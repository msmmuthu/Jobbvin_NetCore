using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobbvin.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilityController : ControllerBase
    {
        private readonly ILogger<UtilityController> _logger;
        public AppDb Db { get; }

        private readonly IConfiguration _configuration;

        public UtilityController(AppDb db, ILogger<UtilityController> logger, IConfiguration configuration)
        {
            Db = db;
            _logger = logger;
            _configuration = configuration;
        }

        // GET api/GetMenuItems
        [HttpPost("GetGoogleApiLocationKey")]
        public async Task<IActionResult> GetGoogleApiLocationKey()
        {
            var key = _configuration.GetSection("GoogleApiLocationKey").Value;
            return new JsonResult(new {key = key });
        }
    }
}
