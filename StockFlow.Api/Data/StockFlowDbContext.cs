using Microsoft.EntityFrameworkCore;
using StockFlow.Models;

namespace StockFlow.Data;

public class StockFlowDbContext : DbContext
{
    public StockFlowDbContext(DbContextOptions<StockFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
}