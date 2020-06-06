namespace sample.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using sample.AuthN;

    [ApiController]
    [Route("api")]
    [Authorize]
    public class BarController : ControllerBase
    {
        private readonly ILogger<BarController> logger;

        public BarController(ILogger<BarController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("enterbar")]
        [Authorize(nameof(SamplePolicies.CanEnterBar))]
        public async Task<ActionResult<bool>> EnterBar()
        {
            // Sucessfully entered the bar
            return await Task.FromResult(true);
        }

        [HttpGet]
        [Route("buydrink")]
        [Authorize(nameof(SamplePolicies.CanBuyDrink))]
        public async Task<ActionResult<bool>> BuyDrink()
        {
            // Sucessfully entered the bar
            return await Task.FromResult(true);
        }
    }
}
