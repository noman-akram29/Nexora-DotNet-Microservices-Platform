using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CustomerService.Data;

public sealed class CustomerDbContextFactory
    : IDesignTimeDbContextFactory<CustomerDbContext>
{
    public CustomerDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Default")
            ?? "Host=localhost;Port=5432;Database=customer_db;Username=customer_service;Password=design_time_only";

        var options = new DbContextOptionsBuilder<CustomerDbContext>();
        options.UseNpgsql(connectionString);

        return new CustomerDbContext(options.Options);
    }
}