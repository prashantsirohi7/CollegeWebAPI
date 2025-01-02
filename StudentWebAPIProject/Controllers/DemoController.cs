using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace StudentWebAPIProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowAll")]    //overriding the global policy
    public class DemoController : Controller
    {
        private readonly ILogger<DemoController> _logger;
        public DemoController(ILogger<DemoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Logging")]
        [DisableCors]   //disabling the all cors policy for this method
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
