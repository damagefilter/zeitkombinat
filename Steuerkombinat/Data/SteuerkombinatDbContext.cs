using Microsoft.EntityFrameworkCore;
using Steuerkombinat.Models;

namespace Steuerkombinat.Data;

public class SteuerkombinatDbContext : DbContext {
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<TaxConfig> TaxConfigs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=steuerkombinat.db");
    }
}
