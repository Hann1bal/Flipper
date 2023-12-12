using Flipper.Models;
using Flipper.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Flipper;

public class FlipperContext : DbContext
{
    public FlipperContext(DbContextOptions option) : base(option)
    {
        DbPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "flipper2.db");
        Console.WriteLine(DbPath);
        Database.EnsureCreated();
    }

    public DbSet<Cards> Cards { get; set; }
    public DbSet<Uniq> Uniqs { get; set; }
    public DbSet<Gem> Gem { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Currency> Currency { get; set; }
    public DbSet<Description> Descriptions { get; set; }
    public string DbPath { get; }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
    }
}