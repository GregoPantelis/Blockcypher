using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;

public sealed class UserAuthDbContext : CustomContext
{
    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserAuthModel> UserAuth { get; set; }
    
    public UserAuthDbContext(DbContextOptions<UserAuthDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(UserAuthDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}