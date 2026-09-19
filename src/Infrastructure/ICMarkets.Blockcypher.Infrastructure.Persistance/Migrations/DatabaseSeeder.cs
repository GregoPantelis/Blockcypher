using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace ICMarkets.Blockcypher.Infrastructure.Persistance.Migrations;

public class DatabaseSeeder
{
    public static void Seed(UserAuthDbContext context, IConfiguration configuration)
    {
        string username = configuration["InitialUser:Username"]
            ?? throw new InvalidOperationException("Initial user username is not configured.");

        string password = configuration["InitialUser:Password"]
            ?? throw new InvalidOperationException("Initial user password is not configured.");
        
        if (context.Users.Any(x => x.Username == username.Trim()))
            return;

        // Create User
        UserModel user = new UserModel
        {
            Username = username.Trim(),
            IsActive = true,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UtcCreatedAt = DateTime.UtcNow
        };
        
        context.Users.Add(user);
        context.SaveChanges();
        
        UserAuthModel userAuth = new UserAuthModel()
        {
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UtcCreatedAt = DateTime.UtcNow
        };
        
        userAuth.PasswordHash = GetPasswordHash(user, password.Trim());
        userAuth.UserId = user.Id;

        context.UserAuth.Add(userAuth);
        context.SaveChanges();
    }

    private static string GetPasswordHash<T>(T model, string pass) 
        where T : class, new()
    {
        var passwordHasher = new PasswordHasher<T>();
        return passwordHasher.HashPassword(model, pass);
    }
}