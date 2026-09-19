using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;

namespace ICMarkets.Blockcypher.Tests.Shared.Fixtures;

public static class DatabaseSeeder
{
    public static void SeedUser(UserAuthDbContext context)
    {
        var user = GlobalUsings.UserModel;

        context.Users.Add(user);
        context.SaveChanges();

        var userAuth = GlobalUsings.UserAuthModel;
        userAuth.UserId = user.Id;

        context.UserAuth.Add(userAuth);
        context.SaveChanges();
    }
}