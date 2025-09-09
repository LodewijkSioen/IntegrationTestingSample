using System.IdentityModel.Tokens.Jwt;
using Acme.BigProject.Api.Domain;
using Acme.BigProject.Api.Services;
using Acme.BigProject.Api.Tests.Mocks;
using Alba;
using Alba.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Respawn;

namespace Acme.BigProject.Api.Tests.Integration;

[SetUpFixture]
public class SystemUnderTest
{
    public static IAlbaHost Host { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task Init()
    {
        Host = await BasicHost();
    }

    #region Basic_Host

    async Task<IAlbaHost> BasicHost()
    {
        return await AlbaHost.For<Program>();
    }

    #endregion

    #region Authenticated_Host

    async Task<IAlbaHost> WithAuthentication()
    {
        var jwtStub = new JwtSecurityStub()
            .With(JwtRegisteredClaimNames.Name, "Willy E Coyote");
        return await AlbaHost.For<Program>(jwtStub);
    }

    #endregion

    #region Dependencies_Host

    async Task<IAlbaHost> WithDependencies()
    {
        var jwtStub = new JwtSecurityStub()
            .With(JwtRegisteredClaimNames.Name, "Willy E Coyote");
        return await AlbaHost.For<Program>(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.Replace(new(typeof(ISendGridService), typeof(MockSendGridService), ServiceLifetime.Transient));
            });
        }, jwtStub);
    }

    #endregion

    #region Database_Host

    async Task<IAlbaHost> WithDatabase()
    {
        var jwtStub = new JwtSecurityStub()
            .With(JwtRegisteredClaimNames.Name, "Willy E Coyote");
        var host = await AlbaHost.For<Program>(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Replace(new(typeof(ISendGridService), typeof(MockSendGridService),
                        ServiceLifetime.Transient));
                });
            },
            jwtStub,
            ConfigurationOverride.Create([
                new("ConnectionStrings:AcmeDatabase", ConnectionString)
            ]));

        await BuildDatabase(host.Services);
        return host;
    }

    #endregion

    [OneTimeTearDown]
    public async Task Teardown()
    {
        await Host.DisposeAsync();
    }

    private async Task BuildDatabase(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AcmeDbContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();

        #region respawn
        _seed = await Respawner.CreateAsync(ConnectionString, new()
        {
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
        #endregion
    }

    public static async Task Reset()
    {
        await _seed.ResetAsync(ConnectionString);
    }

    private const string ConnectionString = "Server=.;Database=acme-bigproject;User Id=sa;Password=yourStrong(!)Password;TrustServerCertificate=True;";

    private static Respawner _seed = null!;
}

/// <summary>
/// Base fixture that will reset all mocks that are registered in the ServiceCollection
/// </summary>
public abstract class DependencyFixture
{
    [TearDown]
    public void Teardown()
    {
        MockSendGridService.Mock.Reset();
    }
}

/// <summary>
/// Base fixture for tests that will mutate the database
/// </summary>
public abstract class DatabaseFixture : DependencyFixture
{
    [SetUp]
    public async Task Setup()
    {
        await SystemUnderTest.Reset();
    }

    protected async Task<T> UsingDatabase<T>(Func<AcmeDbContext, Task<T>> operation)
    {
        await using var scope = SystemUnderTest.Host.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AcmeDbContext>();

        var result = await operation(dbContext);

        await dbContext.SaveChangesAsync();

        return result;
    }
}

