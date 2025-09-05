using System.Security.Cryptography.Xml;
using Acme.BigProject.Api.Domain;
using Acme.BigProject.Api.Models;
using Acme.BigProject.Api.Tests.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.BigProject.Api.Tests.Integration;

public class Database : DatabaseFixture
{
    [Test]
    public async Task Product_Does_Not_Exist()
    {
        await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url("/products/non-existing");
            s.StatusCodeShouldBe(404);
        });
    }

    [Test]
    public async Task Product_Exist()
    {
        Guid id;
        string urlName;
        await using (var scope = SystemUnderTest.Host.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AcmeDbContext>();
            var entry = await dbContext.Products.AddAsync(new("Portable Hole")
            {
                Description = "A hole you can take with you",
                Price = 42,
                Tags = ["decoy", "physics"]
            });
            await dbContext.SaveChangesAsync();
            id = entry.Entity.Id;
            urlName = entry.Entity.UrlName;
        }

        var response = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url($"/products/{urlName}");
            s.StatusCodeShouldBe(200);
        });
        var product = await response.ReadAsJsonAsync<ProductModel>();

        Assert.That(product.Id, Is.EqualTo(id));
        Assert.That(product.Name, Is.EqualTo("Portable Hole"));
        Assert.That(product.Description, Is.EqualTo("A hole you can take with you"));
        Assert.That(product.Price, Is.EqualTo(42));
    }

    [Test]
    public async Task Product_Exist_Better()
    {
        var (id, urlName) = await UsingDatabase(async db =>
        {
            var entry = await db.Products.AddAsync(new("Portable Hole")
            {
                Description = "A hole you can take with you",
                Price = 42,
                Tags = ["decoy", "physics"]
            });
            return (entry.Entity.Id, entry.Entity.UrlName);
        });

        var response = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url($"/products/{urlName}");
            s.StatusCodeShouldBe(200);
        });

        var product = await response.ReadAsJsonAsync<ProductModel>();

        Assert.That(product.Id, Is.EqualTo(id));
        Assert.That(product.Name, Is.EqualTo("Portable Hole"));
        Assert.That(product.Description, Is.EqualTo("A hole you can take with you"));
        Assert.That(product.Price, Is.EqualTo(42));
    }

    [Test]
    public async Task Product_Exist_Best()
    {
        var (id, urlName) = await UsingDatabase(async db =>
        {
            var product = await new ProductBuilder("Portable Hole").Persist(db);
            return (product.Id, product.UrlName);
        });

        var response = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url($"/products/{urlName}");
            s.StatusCodeShouldBe(200);
        });

        var product = await response.ReadAsJsonAsync<ProductModel>();

        Assert.That(product.Id, Is.EqualTo(id));
        Assert.That(product.Name, Is.EqualTo("Portable Hole"));
        Assert.That(product.Description, Is.EqualTo("Some description of the product"));
        Assert.That(product.Price, Is.EqualTo(42));
    }

    [Test]
    public async Task Product_Exist_Betterest()
    {
        var urlName = await UsingDatabase(async db =>
        {
            var product = await new ProductBuilder("Portable Hole").Persist(db);
            return product.UrlName;
        });

        var response = await SystemUnderTest.Host.Scenario(s =>
        {
            s.Get.Url($"/products/{urlName}");
            s.StatusCodeShouldBe(200);
        });

        await VerifyJson(response.ReadAsTextAsync());
    }
}