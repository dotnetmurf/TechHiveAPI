using TechHiveAPI.Data;
using TechHiveAPI.Models;

namespace TechHiveAPI.Services;

/// <summary>
/// Service for seeding the database with sample data.
/// </summary>
public class DataSeedService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DataSeedService> _logger;

    public DataSeedService(AppDbContext context, ILogger<DataSeedService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Seeds the database with 25 sample users.
    /// </summary>
    public async Task SeedDataAsync()
    {
        try
        {
            // Clear existing data
            _context.Users.RemoveRange(_context.Users);
            await _context.SaveChangesAsync();

            var users = new List<User>
            {
                new User { FirstName = "John", LastName = "Doe", Email = "john.doe@techhive.com", Role = "Developer" },
                new User { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@techhive.com", Role = "Manager" },
                new User { FirstName = "Mike", LastName = "Johnson", Email = "mike.johnson@techhive.com", Role = "Designer" },
                new User { FirstName = "Emily", LastName = "Williams", Email = "emily.williams@techhive.com", Role = "QA Engineer" },
                new User { FirstName = "David", LastName = "Brown", Email = "david.brown@techhive.com", Role = "DevOps Engineer" },
                new User { FirstName = "Sarah", LastName = "Davis", Email = "sarah.davis@techhive.com", Role = "Product Manager" },
                new User { FirstName = "Tom", LastName = "Miller", Email = "tom.miller@techhive.com", Role = "Architect" },
                new User { FirstName = "Lisa", LastName = "Wilson", Email = "lisa.wilson@techhive.com", Role = "Team Lead" },
                new User { FirstName = "James", LastName = "Moore", Email = "james.moore@techhive.com", Role = "Developer" },
                new User { FirstName = "Mary", LastName = "Taylor", Email = "mary.taylor@techhive.com", Role = "Designer" },
                new User { FirstName = "Robert", LastName = "Anderson", Email = "robert.anderson@techhive.com", Role = "Developer" },
                new User { FirstName = "Patricia", LastName = "Thomas", Email = "patricia.thomas@techhive.com", Role = "Manager" },
                new User { FirstName = "Michael", LastName = "Jackson", Email = "michael.jackson@techhive.com", Role = "QA Engineer" },
                new User { FirstName = "Linda", LastName = "White", Email = "linda.white@techhive.com", Role = "DevOps Engineer" },
                new User { FirstName = "William", LastName = "Harris", Email = "william.harris@techhive.com", Role = "Developer" },
                new User { FirstName = "Barbara", LastName = "Martin", Email = "barbara.martin@techhive.com", Role = "Product Manager" },
                new User { FirstName = "Richard", LastName = "Thompson", Email = "richard.thompson@techhive.com", Role = "Architect" },
                new User { FirstName = "Susan", LastName = "Garcia", Email = "susan.garcia@techhive.com", Role = "Team Lead" },
                new User { FirstName = "Joseph", LastName = "Martinez", Email = "joseph.martinez@techhive.com", Role = "Developer" },
                new User { FirstName = "Jessica", LastName = "Robinson", Email = "jessica.robinson@techhive.com", Role = "Designer" },
                new User { FirstName = "Charles", LastName = "Clark", Email = "charles.clark@techhive.com", Role = "Developer" },
                new User { FirstName = "Karen", LastName = "Rodriguez", Email = "karen.rodriguez@techhive.com", Role = "QA Engineer" },
                new User { FirstName = "Daniel", LastName = "Lewis", Email = "daniel.lewis@techhive.com", Role = "DevOps Engineer" },
                new User { FirstName = "Nancy", LastName = "Lee", Email = "nancy.lee@techhive.com", Role = "Manager" },
                new User { FirstName = "Paul", LastName = "Walker", Email = "paul.walker@techhive.com", Role = "Developer" }
            };

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeded with {Count} sample users", users.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while seeding data");
            throw;
        }
    }
}
