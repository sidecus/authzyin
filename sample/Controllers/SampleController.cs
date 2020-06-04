namespace sample.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("api")]
    [Authorize]
    public class SampleController : ControllerBase
    {
        public class AuthNResult
        {
            public string ForRole { get; set; }
            public string Message { get; set; }
        }

        private readonly ILogger<SampleController> logger;

        public SampleController(ILogger<SampleController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Route("user")]
        public async Task<ActionResult<AuthNResult>> GetForUser()
        {
            return await Task.FromResult(new AuthNResult()
            {
                ForRole = "User",
                Message = "this is for user role",
            });
        }

        [HttpGet]
        [Route("admin")]
        public async Task<ActionResult<AuthNResult>> GetForAdmin()
        {
            return await Task.FromResult(new AuthNResult()
            {
                ForRole = "Admin",
                Message = "this is for admin role",
            });
        }
    }
}
