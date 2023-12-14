using Flipper.Models;
using Flipper.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Flipper;

public class FlipperContext : DbContext
{
    public FlipperContext(DbContextOptions option) : base(option)
    {
        
    }
    public static string GetDatabaseConnectionString()
    {
        return new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Port = 5432,
            Database = "flipperdb",
            Username = "postgres",
            Password = "4847",
            Timeout = 300
        }.ConnectionString;
    }
    public DbSet<Cards> Cards { get; set; }
    public DbSet<Uniq> Uniqs { get; set; }
    public DbSet<Character> Character { get; set; }
    public DbSet<Gem> Gem { get; set; }
    public DbSet<Account?> Accounts { get; set; }
    public DbSet<Currency> Currency { get; set; }
    public DbSet<Description> Descriptions { get; set; }
    public string DbPath { get; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}