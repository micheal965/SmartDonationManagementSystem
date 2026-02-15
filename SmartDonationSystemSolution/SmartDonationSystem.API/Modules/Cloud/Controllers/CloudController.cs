using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartDonationSystem.API.Modules.Cloud.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CloudController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet("signature")]
        [AllowAnonymous]
        public IActionResult GetSignature(string folderName = "General")
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var parameters = new Dictionary<string, object>{
            { "folder",folderName },
            { "timestamp", timestamp },
            };


            var apiSecret = _config["CloudinarySettings:ApiSecret"];
            var cloudName = _config["CloudinarySettings:CloudName"];
            var apiKey = _config["CloudinarySettings:ApiKey"];

            var cloudinary = new CloudinaryDotNet.Cloudinary(new CloudinaryDotNet.Account(cloudName, apiKey, apiSecret));
            var signature = cloudinary.Api.SignParameters(parameters);

            return Ok(new
            {
                signature,
                timestamp,
                apiKey,
                cloudName,
                folder = folderName
            });
        }
    }
}
