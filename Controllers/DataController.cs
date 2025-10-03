/*
    File: DataController.cs
    Summary: Provides endpoints for managing and reseeding in-memory user data. 
    Includes a reset endpoint to reseed the database with sample users for demonstration/testing.
*/
using Microsoft.AspNetCore.Mvc;
using TechHiveAPI.Services;

namespace TechHiveAPI.Controllers;

/// <summary>
/// Controller for data management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DataController : ControllerBase
{
    private readonly DataSeedService _dataSeedService;
    private readonly ILogger<DataController> _logger;

    public DataController(DataSeedService dataSeedService, ILogger<DataController> logger)
    {
        _dataSeedService = dataSeedService ?? throw new ArgumentNullException(nameof(dataSeedService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Seeds the database with sample data.
    /// </summary>
    /// <returns>A success message.</returns>
    /// <response code="200">If the data was successfully seeded.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost("seed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedData()
    {
        _logger.LogInformation("Seeding database with sample data");
        await _dataSeedService.SeedDataAsync();
        return Ok(new { message = "Database seeded successfully with 25 sample users." });
    }
}
