using Castle.Core.Configuration;
using Hackathon.Database.SQLite;
using Microsoft.EntityFrameworkCore;

namespace Hackathon;

public class AddDbContextFactory : IDbContextFactory<ApplicationContext>
{
    private DbContextOptions<ApplicationContext> _options;

    public AddDbContextFactory(DbContextOptions<ApplicationContext> options)
    {
        _options = options;
    }

    public ApplicationContext CreateDbContext()
    {
        return new ApplicationContext(_options);
    }
}