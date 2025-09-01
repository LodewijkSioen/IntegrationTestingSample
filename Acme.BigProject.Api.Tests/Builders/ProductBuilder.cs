using Acme.BigProject.Api.Domain;

namespace Acme.BigProject.Api.Tests.Builders;

public record ProductBuilder(string? Name)
{
    public async Task<Product> Persist(AcmeDbContext dbContext)
    {
        var product = new Product(Name ?? "testproduct")
        {
            Description = "Some description of the product",
            Price = 42,
            Tags = ["tag 1", "tag 2"]
        };
        var entry = await dbContext.Products.AddAsync(product);

        return entry.Entity;
    }
}