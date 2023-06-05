using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

var dbContext = new ApplicationDbContext();
await dbContext.Database.EnsureDeletedAsync();
await dbContext.Database.EnsureCreatedAsync();

dbContext.Users.AddRange(new User[]
{
  new() { Email = "test1@mail.com" },
  new() { Email = "test2@mail.com" },
  new() { Email = "test3@mail.com" },
  new() { Email = "test1@gmail.com" },
});

var excludeEmails = new[]
{
  "*@mail.com",
};

var sql = dbContext.Users.Where(x => !excludeEmails.Any(email => email.StartsWith("*")
  ? email.EndsWith("*") ? x.Email.Contains(email) : x.Email.StartsWith(email)
  : email.EndsWith("*") ? x.Email.EndsWith(email) : x.Email == email)).ToQueryString();

Console.WriteLine(sql);
Console.Read();




public class ApplicationDbContext : DbContext
{
  public DbSet<User> Users { get; set; } = null!;

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder
      .UseSqlServer("Data Source = .\\SQLEXPRESS; Database = EfCorePrimitiveCollection; Integrated Security = True; TrustServerCertificate=True")
      .LogTo(Console.WriteLine, LogLevel.Debug)
      .EnableSensitiveDataLogging()
      .EnableDetailedErrors();

    base.OnConfiguring(optionsBuilder);
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>().Property(x => x.Email)/*.IsUnicode(false).HasColumnType("varchar(100)")*/;
      
    base.OnModelCreating(modelBuilder);
  }
}

public class User
{
  public int Id { get; private set; }
  public required string Email { get; set; }
}