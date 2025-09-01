namespace Acme.BigProject.Api.Domain;

public class Product
{
    public Product(string name)
    {
        Name = name;
        UrlName = name.ToLower().Replace(" ", "-");
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; set; }
    public string UrlName { get; private set; }
    public int Price { get; set; }
    public string[] Tags { get; set; }
}