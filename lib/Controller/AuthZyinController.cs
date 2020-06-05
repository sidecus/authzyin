namespace AuthZyin.Controller
{
    using System;
    using System.Threading.Tasks;
    using AuthZyin.Authorization;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Automatically adds api for the client to retrieve authorization context.
    /// It can be retrieved with "GET /authzyin/context".
    /// For security purpose, anonymouse access is not allowed to this api.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("authzyin")]
    public class AuthZyinController : ControllerBase
    {
        /// <summary>
        /// Authorization context
        /// </summary>
        private readonly IAuthZyinContext authZyinContext;

        /// <summary>
        /// Initializes a new instance of AuthZyinController
        /// </summary>
        /// <param name="context">authorization context</param>
        public AuthZyinController(IAuthZyinContext context)
        {
            this.authZyinContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Built in method to return required client data for authorization purpose
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("context")]
        public async Task<IActionResult> GetPolicies()
        {
            var data = await Task.FromResult(this.authZyinContext.ClientContext);
            return this.Ok(data);
        }
    }
}