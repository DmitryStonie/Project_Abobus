using Hackathon;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagerWebApp.Database;

public sealed class HrManagerApplicationContext : DbContext
{
    public DbSet<Junior> Juniors => Set<Junior>();
    public DbSet<TeamLead> Teamleads => Set<TeamLead>();
    public DbSet<Wishlist> WishLists => Set<Wishlist>();
    public DbSet<Wish> Wishes => Set<Wish>();
    public DbSet<Hackathon.Hackathon> Hackathons => Set<Hackathon.Hackathon>();
    public DbSet<Team> Teams => Set<Team>();

    public HrManagerApplicationContext() : base()
    {
        Database.EnsureCreated();
    }

    public HrManagerApplicationContext(DbContextOptions<HrManagerApplicationContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.LogTo(Console.WriteLine, LogLevel.Error);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new JuniorConfiguration());
        modelBuilder.ApplyConfiguration(new TeamleadConfiguration());
        modelBuilder.ApplyConfiguration(new HackathonConfiguration());
        modelBuilder.ApplyConfiguration(new WishlistConfiguration());
        modelBuilder.ApplyConfiguration(new WishConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
    }
}

public class HackathonConfiguration : IEntityTypeConfiguration<Hackathon.Hackathon>
{
    public void Configure(EntityTypeBuilder<Hackathon.Hackathon> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.HarmonicMean);
        builder.Ignore(h => h.Teams);
    }
}

public class JuniorConfiguration : IEntityTypeConfiguration<Junior>
{
    public void Configure(EntityTypeBuilder<Junior> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name);
        builder.Property(e => e.JuniorId);
        builder.Ignore(e => e.Wishlist);
        builder.Ignore(e => e.TeamLeadId);
    }
}
public class TeamleadConfiguration : IEntityTypeConfiguration<TeamLead>
{
    public void Configure(EntityTypeBuilder<TeamLead> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name);
        builder.Property(e => e.TeamLeadId);
        builder.Ignore(e => e.Wishlist);
        builder.Ignore(e => e.JuniorId);

    }
}

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.HackathonId);
        builder.Property(w => w.OwnerId);
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