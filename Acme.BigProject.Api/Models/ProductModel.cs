using Acme.BigProject.Api.Domain;

namespace Acme.BigProject.Api.Models;

public class ProductModel
{
#pragma warning disable CS8618, CS9264
    //Only needed for deserialization
    public ProductModel(){}
#pragma warning restore CS8618, CS9264
    public ProductModel(Product product)
    {
        Id = product.Id;
        Name = product.Name;
        Description = product.Description;
        Price = product.Price;
        //Tags = product.Tags;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; } 
    public int Price { get; set; }
    //public string[] Tags { get; set; }
}