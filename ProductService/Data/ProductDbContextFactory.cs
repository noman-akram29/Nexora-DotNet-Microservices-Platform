using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProductService.Data;

public sealed class ProductDbContextFactory
    : IDesignTimeDbContextFactory<ProductDbContext>
{
    public ProductDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=product_db;Username=product_service;Password=design_time_only";

        var options = new DbContextOptionsBuilder<ProductDbContext>();
        options.UseNpgsql(connectionString);

        return new ProductDbContext(options.Options);
    }
}