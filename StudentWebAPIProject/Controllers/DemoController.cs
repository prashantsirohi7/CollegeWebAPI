using Microsoft.AspNetCore.Mvc;

namespace StudentWebAPIProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DemoController : Controller
    {
        private readonly ILogger<DemoController> _logger;
        public DemoController(ILogger<DemoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Logging")]
        public IActionResult Index()
        {
            _logger.LogInformation("This is information level log");
            _logger.LogTrace("This is trace level log");
            _logger.LogDebug("This is debug level log");
            _logger.LogCritical("This is critical level log");
            _logger.LogError("This is error level log");
            _logger.LogWarning("This is warning level log");
            return Ok();
        }
    }
}
