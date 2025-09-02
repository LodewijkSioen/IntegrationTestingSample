using Acme.BigProject.Api.Domain;
using Acme.BigProject.Api.Tests.Builders;
using Alba;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.BigProject.Api.Tests.Integration;

public class BDDStyle
{
    [Test]
    public async Task Products_ShouldBeRetrievableByProductName()
    {
        //Arrange
        await using var context = new AcmeProductContext();

        var product = await context.GivenProductWithNameExists("productName");
        
        //Act
        await context.ShouldGetByName(product, 
        
            //Assert
            res => context.VerifyOk(res));
    }
}



public class AcmeProductContext : IAsyncDisposable
{
    public async Task<Product> GivenProductWithNameExists(string productname)
    {
        return await UsingDatabase(async db => await new ProductBuilder(productname).Persist(db));
    }

    public async Task ShouldGetByName(Product product, Func<IScenarioResult, ValueTask> assertion)
    {
        var result = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url($"/products/{product.UrlName}");
        });

        await assertion(result);
    }

    public async ValueTask VerifyOk(IScenarioResult result)
    {
        Assert.That(result.Context.Response.StatusCode, Is.EqualTo(200));
        await VerifyJson(await result.ReadAsTextAsync());
    }

    public async ValueTask DisposeAsync()
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