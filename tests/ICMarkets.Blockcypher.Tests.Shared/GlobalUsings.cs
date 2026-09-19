global using Xunit;
using System.Security.Cryptography;
using ICMarkets.Blockcypher.Api.Contracts.Requests;
using ICMarkets.Blockcypher.Application.Configuration.Auth;
using ICMarkets.Blockcypher.Application.DataObjects.Constants;
using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Infrastructure.Configuration.Apis;
using ICMarkets.Blockcypher.Infrastructure.ExternalApis.Contracts.Blockcypher.Responses;
using ICMarkets.Blockcypher.Infrastructure.Persistance.DbContexts;
using ICMarkets.Blockcypher.Infrastructure.Persistance.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

public static class GlobalUsings
{
    private static readonly string _tUsername = "blockcypher";
    private static readonly string _tpassword = "blockcypher2027!!";
    private static readonly string _tpasswordHash = GetPasswordHash();
    
    public static string Issuer = ConfigurationConstants.JwtTokenIssuers.BlockcypherIssuer;
    public static string Audience = "TestAudience";
    public static int ExpiryMinutes = 60;

    public static IOptions<AuthConfigOptions> AuthConfigOptions = Options.Create(new AuthConfigOptions
    {
        TokenType = AuthToken.Jwt.ToString(),
        JwtAuthConfigs =
        [
            new JwtAuthConfig
            {
                Issuer = Issuer,
                Audience = Audience,
                Signature = Convert.ToBase64String(
                    RandomNumberGenerator.GetBytes(32)),
                ExpiryMinutes = ExpiryMinutes
            }
        ]
    });

    public static IOptions<ExternalApiOptions> CreateExternalApiOptions()
    {
        return Options.Create(new ExternalApiOptions
        {
            Endpoints =
            [
                new ExternalApi()
                {
                    Name = "Blockcypher",
                    BaseUrl = "https://api.blockcypher.com",
                    ApiVersion = "v1",
                    Timeout = 20
                }
            ]
        });
    }

    public static BlockchainResponse CreateBlockchainResponse()
    {
        return new BlockchainResponse
        {
            Name = "BTC.main",
            Height = 4997970,
            Hash = "00000000008ea93063b8d2d3d6d2b50e75c42cbd1a734314ba692656d933771e",
            Time = DateTimeOffset.Parse("2026-09-18T21:01:38.032105+03:00"),
            LatestUrl = "https://api.blockcypher.com/v1/btc/test3/blocks/00000000008ea93063b8d2d3d6d2b50e75c42cbd1a734314ba692656d933771e",
            PreviousHash = "00000000005f2e55f682e16f64f43d3c2a0d7d9598de98b48289d2e68622bb9f",
            PreviousUrl = "https://api.blockcypher.com/v1/btc/test3/blocks/00000000005f2e55f682e16f64f43d3c2a0d7d9598de98b48289d2e68622bb9f",
            PeerCount = 146,
            UnconfirmedCount = 0,
            HighFeePerKb = 11146,
            MediumFeePerKb = 6347,
            LowFeePerKb = 2707,
            LastForkHeight = 4997086,
            LastForkHash = "0000000000eccda38eef88c5bbb88c53f953b650979166d43fd683db38ae8408"
        };
    }
    
    public static BlockchainDataFilter CreateBlockchainDataFilter()
    {
        return new BlockchainDataFilter
        {
            Coin = Coin.BTC,
            Chain = Chain.Main
        };
    }
    
    public static UserModel CreateUser()
    {
        return new UserModel
        {
            Id = 1,
            Username = "testuser",
            IsActive = true
        };
    }
    
    public static UserAuthData CreateUserAuth()
    {
        return new UserAuthData
        {
            UserId =  1,
            Username = "testuser",
            IsActive = true,
            PasswordHash = "somepasshash",
            CreatedAt = DateTime.Now,
            UtcCreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.Now,
        };
    }

    public static BlockcypherDbContext CreateBlockcypherDbContext()
    {
        var options =
            new DbContextOptionsBuilder<BlockcypherDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new BlockcypherDbContext(options);
    }

    public static BlockchainModel CreateBlockchainModel(
        string coin = "btc",
        string chain = "main")
    {
        return new BlockchainModel
        {
            Coin = coin,
            Chain = chain,
            BlockchainData = "{}",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UtcCreatedAt = DateTime.UtcNow
        };
    }
    
    public static UserAuthDbContext CreateUserAuthDbContext()
    {
        var options =
            new DbContextOptionsBuilder<UserAuthDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new UserAuthDbContext(options);
    }

    public static UserModel CreateUser(
        string username,
        bool isActive = true)
    {
        return new UserModel
        {
            Username = username,
            IsActive = isActive,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            UtcCreatedAt = DateTime.UtcNow
        };
    }
    
    public static LoginRequest CreateLoginRequest()
    {
        return new LoginRequest
        {
            Username = "testuser",
            Password = "TestPassword123!"
        };
    }
    
    public static UserTokenData UserTokenData = new UserTokenData()
    {
        Username = "someUserName",
        Token = "someToken",
        CreatedAt = DateTime.Now,
        CreatedAtUtc = DateTime.UtcNow,
        ExpiresAt = DateTime.Now.AddMinutes(20),
        ExpiresAtUtc = DateTime.UtcNow.AddMinutes(20),
    };
    
    public static BlockchainData CreateBlockchainData()
    {
        return new BlockchainData
        {
            Chain = Chain.Main.ToString(),
            Coin = Coin.BTC.ToString(),
            CreatedAt = DateTime.Now,
            UtcCreatedAt = DateTime.UtcNow,
            BChainData = JObject.Parse(@"{
                ""Name"": ""BTC.main"",
                ""Height"": 4997970,
                ""Hash"": ""00000000008ea93063b8d2d3d6d2b50e75c42cbd1a734314ba692656d933771e"",
                ""Time"": ""2026-09-18T21:01:38.032105+03:00"",
                ""LatestUrl"": ""https://api.blockcypher.com/v1/btc/test3/blocks/00000000008ea93063b8d2d3d6d2b50e75c42cbd1a734314ba692656d933771e"",
                ""PreviousHash"": ""00000000005f2e55f682e16f64f43d3c2a0d7d9598de98b48289d2e68622bb9f"",
                ""PreviousUrl"": ""https://api.blockcypher.com/v1/btc/test3/blocks/00000000005f2e55f682e16f64f43d3c2a0d7d9598de98b48289d2e68622bb9f"",
                ""PeerCount"": 146,
                ""UnconfirmedCount"": 0,
                ""HighFeePerKb"": 11146,
                ""MediumFeePerKb"": 6347,
                ""LowFeePerKb"": 2707,
                ""LastForkHeight"": 4997086,
                ""LastForkHash"": ""0000000000eccda38eef88c5bbb88c53f953b650979166d43fd683db38ae8408"",
                ""createdAt"": ""2026-09-19T01:39:29.6517564"",
                ""utcCreatedAt"": ""2026-09-18T22:39:29.6519936""
            }")
        };
    }
    
    public static BlockchainSnaphotRequest CreateSnapshotRequest()
    {
        return new BlockchainSnaphotRequest
        {
            Coin = "btc",
            Chain = "main"
        };
    }

    public static BlockchainHistoryRequest CreateHistoryRequest()
    {
        return new BlockchainHistoryRequest
        {
            Coin = "btc",
            Chain = "main"
        };
    }
    
    public static UserAuthData UserAuthData = new UserAuthData()
    {
        UserId = 1,
        Username = "someUserName",
        IsActive = true,
        PasswordHash = "somePasswordHash",
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now,
        UtcCreatedAt = DateTime.UtcNow,
    };
    
    public static UserAuthData IncativeUserAuthData = new UserAuthData()
    {
        UserId = 1,
        Username = "someUserName",
        IsActive = false,
        PasswordHash = "somePasswordHash",
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now,
        UtcCreatedAt = DateTime.UtcNow,
    };
    
    public static UserDataFilter UserDataFilter = new UserDataFilter()
    {
        Username = "someUserName",
        InputPassword = "somePassword",
    };

    public static AuthConfigOptions authConfig = new AuthConfigOptions
    {
        TokenType = "JWT",
        JwtAuthConfigs = new List<JwtAuthConfig>
        {
            new()
            {
                Issuer = ConfigurationConstants.JwtTokenIssuers.BlockcypherIssuer,
                Audience = "TestAudience",
                Signature = "test-signature",
                ExpiryMinutes = 60
            }
        }
    };
    
    public static BlockchainDataFilter CreateFilter()
    {
        return new BlockchainDataFilter
        {
            Coin = Coin.BTC,
            Chain = Chain.Main
        };
    }
    
    public static List<BlockchainData> CreateBlockchainDataList()
    {
        return Enumerable.Range(0, 3)
            .Select(_ => CreateBlockchainData())
            .ToList();
    }

    public static LoginRequest ValidLoginRequest = new LoginRequest()
    {
        Username = _tUsername,
        Password = _tpassword,
    };
    
    public static LoginRequest InvalidCredsLoginRequest = new LoginRequest()
    {
        Username = "someInvalidUsername",
        Password = "someInvalidPassword",
    };
    
    public static LoginRequest InvalidLoginRequest = new LoginRequest()
    {
        Username = "",
        Password = "p",
    };
    
    public static UserModel UserModel = new UserModel
    {
        Username = _tUsername,
        IsActive = true,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now,
        UtcCreatedAt = DateTime.UtcNow
    };

    public static UserAuthModel UserAuthModel = new UserAuthModel()
    {
        PasswordHash = _tpasswordHash,
        CreatedAt = DateTime.Now,
        UpdatedAt = DateTime.Now,
        UtcCreatedAt = DateTime.UtcNow
    };

    public static BlockchainSnaphotRequest ValidBlockchainSnaphotRequest = new BlockchainSnaphotRequest()
    {
        Chain = Chain.Main.ToString(),
        Coin = Coin.BTC.ToString()
    };
    
    private static string GetPasswordHash()
    {
        var passwordHasher = new PasswordHasher<UserModel>();
        return passwordHasher.HashPassword(UserModel, _tpassword);
    }
}