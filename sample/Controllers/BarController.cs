namespace sample.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.Logging;
    using sample.AuthN;

    [ApiController]
    [Route("api")]
    [Authorize]
    public class BarController : ControllerBase
    {
        private readonly ILogger<BarController> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public BarController(ILogger<BarController> logger, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        [HttpGet]
        [Route("enterbar/{id}")]
        [Authorize(nameof(SamplePolicies.CanDrink))]
        public async Task<ActionResult<bool>> EnterBar([FromRoute, BindRequired] int id)
        {
            var targetBar = Bar.Bars.FirstOrDefault(x => x.Id == id);
            if (targetBar == null)
            {
                return this.NotFound($"Bar {id} not found");
            }

            var authResult = this.authorizationService.AuthorizeAsync(
                this.httpContextAccessor.HttpContext.User,
                targetBar,
                nameof(SamplePolicies.CanEnterBar));

            if (!authResult.IsCompletedSuccessfully)
            {
                return this.Unauthorized($"Not allowed to enter bar {id}");
            }

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
