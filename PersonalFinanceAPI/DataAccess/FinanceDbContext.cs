using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Entities;

namespace PersonalFinanceAPI.DataAccess;

public class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .Property(account => account.OpeningBalance).HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(transaction => transaction.Amount).HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .Property(transaction => transaction.Type).HasConversion<string>();
    }
}