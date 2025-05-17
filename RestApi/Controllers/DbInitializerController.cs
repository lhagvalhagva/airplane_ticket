using DataAccess;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    /// <summary>
    /// Өгөгдлийн сангийн анхны өгөгдөл үүсгэх, байгуулах контроллер.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DbInitializerController : ControllerBase
    {
        private readonly AirportDbContext _dbContext;

        /// <summary>
        /// DbInitializerController-ийн байгуулагч.
        /// </summary>
        /// <param name="dbContext">Өгөгдлийн сангийн контекст</param>
        public DbInitializerController(AirportDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Өгөгдлийн санг анхны өгөгдөлтөй үүсгэх.
        /// Энэ үйлдэл нь өгөгдлийн санг шинээр үүсгэх, эсвэл хуучныг арилгаж шинээр үүсгэх, тэгээд жишээ өгөгдлүүдийг оруулна.
        /// </summary>
        /// <returns>Үйлдлийн амжилттай, эсвэл амжилтгүй хариу</returns>
        /// <response code="200">Өгөгдлийн сан амжилттай үүсгэгдсэн</response>
        /// <response code="500">Өгөгдлийн сан үүсгэхэд алдаа гарсан</response>
        [HttpPost("initialize")]
        public async Task<IActionResult> InitializeDatabase()
        {
            try
            {
                await DbInitializer.InitializeAsync(_dbContext);
                return Ok("Database initialized successfully with seed data.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error initializing database: {ex.Message}");
            }
        }
    }
} 