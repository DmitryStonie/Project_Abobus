using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hackathon.Database.SQLite;

public sealed class ApplicationContext : DbContext
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Junior> Juniors => Set<Junior>();
    public DbSet<TeamLead> Teamleads => Set<TeamLead>();
    public DbSet<Wishlist> WishLists => Set<Wishlist>();
    public DbSet<Wish> Wishes => Set<Wish>();
    public DbSet<Hackathon> Hackathons => Set<Hackathon>();
    public DbSet<Team> Teams => Set<Team>();

    public ApplicationContext() : base()
    {
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        Database.Migrate(); 
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Error);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new HackathonConfiguration());
        modelBuilder.ApplyConfiguration(new WishlistConfiguration());
        modelBuilder.ApplyConfiguration(new WishConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
    }
}

public class HackathonConfiguration : IEntityTypeConfiguration<Hackathon>
{
    public void Configure(EntityTypeBuilder<Hackathon> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.HarmonicMean);
        builder.HasMany<Team>()
            .WithOne()
            .HasForeignKey(t => t.HackathonId);
        builder.HasMany<Wishlist>()
            .WithOne()
            .HasForeignKey(w => w.HackathonId);
        builder.Ignore(h => h.Teams);
    }
}

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name);
        builder.Property(e => e.JuniorId);
        builder.Property(e => e.TeamLeadId);
        builder.HasMany<Wishlist>()
            .WithOne()
            .HasForeignKey(w => w.OwnerId);
        builder.HasMany<Wish>()
            .WithOne()
            .HasForeignKey(w => w.OwnerId);
        builder.HasMany<Wish>()
            .WithOne()
            .HasForeignKey(w => w.PartnerId);
        builder.HasMany<Team>()
            .WithOne()
            .HasForeignKey(t => t.JuniorId);
        builder.HasMany<Team>()
            .WithOne()
            .HasForeignKey(t => t.TeamLeadId);
        builder.Ignore(e => e.Wishlist);
        builder.UseTphMappingStrategy();
    }
}

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.HackathonId);
        builder.Property(w => w.OwnerId);
        builder.HasMany<Wish>()
            .WithOne()
            .HasForeignKey(w => w.WishlistId);
        builder.Ignore(w => w.Wishes);
    }
}

public class WishConfiguration : IEntityTypeConfiguration<Wish>
{
    public void Configure(EntityTypeBuilder<Wish> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Score);
        builder.Property(w => w.WishlistId);
        builder.Property(w => w.OwnerId);
        builder.Property(w => w.PartnerId);
    }
}

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.HackathonId);
        builder.Property(t => t.JuniorId);
        builder.Property(t => t.TeamLeadId);
        builder.Ignore(t => t.TeamLead);
        builder.Ignore(t => t.Junior);
    }
}