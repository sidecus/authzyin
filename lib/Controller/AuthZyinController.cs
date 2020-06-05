namespace AuthZyin.Controller
{
    using System;
    using System.Threading.Tasks;
    using AuthZyin.Authorization;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Auto injected controller to handle policy serialization
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("authzyin")]
    public class AuthZyinController : ControllerBase
    {
        /// <summary>
        /// client data manager via DI
        /// </summary>
        private readonly IAuthZyinDataManager clientDataManager;

        /// <summary>
        /// Initializes a new instance of AuthZyinController
        /// </summary>
        /// <param name="options">authorization options</param>
        public AuthZyinController(IAuthZyinDataManager clientDataManager)
        {
            this.clientDataManager = clientDataManager ?? throw new ArgumentNullException(nameof(clientDataManager));
        }

        /// <summary>
        /// Built in method to return required client data for authorization purpose
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("policies")]
        public async Task<IActionResult> GetPolicies()
        {
            var data = await Task.FromResult(this.clientDataManager.ClientData);
            return this.Ok(data);
        }
    }
}