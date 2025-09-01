using Microsoft.EntityFrameworkCore;

namespace Acme.BigProject.Api.Domain;

public class AcmeDbContext(DbContextOptions<AcmeDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}