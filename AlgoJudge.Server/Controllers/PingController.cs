using Microsoft.AspNetCore.Mvc;

namespace AlgoJudge.Server.Controllers
{
    [ApiController]
    [Route("ping")]
    public class PingController : ControllerBase
    {
        private readonly ILogger<PingController> _logger;

        public PingController(ILogger<PingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("ping")]
        public String Ping()
        {
            return "pong";
        }
    }
}
