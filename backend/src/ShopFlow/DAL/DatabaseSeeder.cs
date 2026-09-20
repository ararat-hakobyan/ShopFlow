using Microsoft.EntityFrameworkCore;
using ShopFlow.Models;
using ShopFlow.Enums;
using ShopFlow.Services.Interfaces;

namespace ShopFlow.DAL;

public sealed class DatabaseSeeder
{
    private readonly ShopFlowDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly SeedOptions _options;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        ShopFlowDbContext context,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _options = configuration.GetSection(SeedOptions.SectionName).Get<SeedOptions>() ?? new SeedOptions();
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedAdministratorAsync(cancellationToken);
        await SeedCatalogueAsync(cancellationToken);
    }

    private async Task SeedAdministratorAsync(CancellationToken cancellationToken)
    {
        var adminExists = await _context.Users
            .AnyAsync(user => user.Role == UserRole.Admin, cancellationToken);

        if (adminExists)
        {
            return;
        }

        var admin = new User
        {
            Username = _options.AdminUsername,
            Email = _options.AdminEmail,
            Role = UserRole.Admin,
            PasswordHash = _passwordHasher.Hash(_options.AdminPassword)
        };

        _context.Users.Add(admin);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded the first administrator account: {Email}.", admin.Email);
    }

    private async Task SeedCatalogueAsync(CancellationToken cancellationToken)
    {
        if (await _context.Categories.AnyAsync(cancellationToken))
        {
            return;
        }

        var categories = new List<Category>
        {
            new()
            {
                Name = "Footwear",
                Products =
                {
                    new Product
                    {
                        ProductName = "Runner Lightweight Sneakers",
                        ProductVariants =
                        {
                            new ProductVariant { Color = "Black", Size = "42", Price = 24_900m, StockQuantity = 12 },
                            new ProductVariant { Color = "White", Size = "43", Price = 24_900m, StockQuantity = 8 }
                        }
                    }
                }
            },
            new()
            {
                Name = "Outerwear",
                Products =
                {
                    new Product
                    {
                        ProductName = "City Rain Jacket",
                        ProductVariants =
                        {
                            new ProductVariant { Color = "Navy", Size = "M", Price = 39_500m, StockQuantity = 6 },
                            new ProductVariant { Color = "Olive", Size = "L", Price = 39_500m, StockQuantity = 4 }
                        }
                    }
                }
            },
            new()
            {
                Name = "Accessories",
                Products =
                {
                    new Product
                    {
                        ProductName = "Everyday Canvas Backpack",
                        ProductVariants =
                        {
                            new ProductVariant { Color = "Sand", Size = "One size", Price = 18_000m, StockQuantity = 15 }
                        }
                    }
                }
            }
        };

        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Seeded {Count} demo categories.", categories.Count);
    }
}

public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    public string AdminUsername { get; set; } = "admin";

    public string AdminEmail { get; set; } = "admin@shopflow.local";

    public string AdminPassword { get; set; } = "Admin123!";
}
