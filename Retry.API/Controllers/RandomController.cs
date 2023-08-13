using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Retry.API.Services.Interfaces;

namespace Retry.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandomController : ControllerBase
    {
        private readonly IRandomNumberService _randomNumberService;
        private readonly ILogger<RandomController> _logger;

        public RandomController(IRandomNumberService randomNumberService, ILogger<RandomController> logger)
        {
            _randomNumberService = randomNumberService;
            _logger = logger;
        }

        [HttpGet]
        public async ValueTask<IActionResult> Get()
        {
            var number = await _randomNumberService.GetRandomNumberWithRetryAsync();
            _logger.LogInformation($"Random number: {number} ");
            return Ok(number);
        }
    }
}
