namespace sample.Controllers
{
    using System;
    using System.Collections.Generic;
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
    public class PlaceController : ControllerBase
    {
        private readonly ILogger<PlaceController> logger;
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private static readonly IEnumerable<Place> Places = new List<Place>
        {
            new Bar
            {
                Id = 0,
                Name = "DarkRavern Bar",
                AcceptedPaymentMethods = new List<string> { PaymentMethod.Visa, },
            },
            new Bar
            {
                Id = 1,
                Name = "BlueSky Bar",
                AcceptedPaymentMethods = new List<string> { PaymentMethod.Cash, },
            },
            new Bar
            {
                Id = 2,
                Name = "OneWorld Bar",
                AcceptedPaymentMethods = new List<string> { PaymentMethod.MasterCard, },
            },
            new AgeLimitedPlace
            {
                Id = 3,
                Name = "MiddleAgeCrisis Club",
                AcceptedPaymentMethods = new List<string> { PaymentMethod.Visa, },
                MinAge = 40,
                MaxAge = 55,
            },
            new AgeLimitedPlace
            {
                Id = 4,
                Name = "YouthDrawing Club",
                AcceptedPaymentMethods = new List<string> { PaymentMethod.Cash, },
                MinAge = 5,
                MaxAge = 12,
            },
        };


        public PlaceController(ILogger<PlaceController> logger, IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        [HttpGet]
        [Route("places")]
        [Authorize(nameof(Policies.IsCustomer))]
        public async Task<ActionResult<IEnumerable<object>>> GetPlaces()
        {
            // Convert to object to workaround System.Text.Json issue.
            return await Task.FromResult(Places.Select(p => p as object).ToList());
        }

        [HttpGet]
        [Route("enterplace/{id}")]
        [Authorize(nameof(Policies.IsCustomer))]
        public async Task<ActionResult<Place>> EnterPlace([FromRoute, BindRequired] int id)
        {
            var targetPlace = Places.FirstOrDefault(x => x.Id == id);
            if (targetPlace == null)
            {
                return this.NotFound($"Place {id} not found");
            }

            var authResult = await this.authorizationService.AuthorizeAsync(
                this.httpContextAccessor.HttpContext.User,
                targetPlace,
                targetPlace.Policy);

            if (!authResult.Succeeded)
            {
                return this.Forbid();
            }

            // Sucessfully entered the place
            return await Task.FromResult(targetPlace);
        }

        [HttpGet]
        [Route("buydrink")]
        [Authorize(nameof(Policies.CanDrinkAlchohol))]
        public async Task<ActionResult<bool>> BuyDrink()
        {
            // TODO[sidecus] - more details
            // Sucessfully bought drink
            return await Task.FromResult(true);
        }
    }
}
