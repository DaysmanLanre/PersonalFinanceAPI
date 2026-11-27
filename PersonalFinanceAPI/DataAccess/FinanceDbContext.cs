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

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Salary" },
            new Category { Id = 2, Name = "Transport" },
            new Category { Id = 3, Name = "Utilities" },
            new Category { Id = 4, Name = "Entertainment" },
            new Category { Id = 5, Name = "Health" },
            new Category { Id = 6, Name = "Rent" },
            new Category { Id = 7, Name = "Food" },
            new Category { Id = 8, Name = "Other" });
    }
}