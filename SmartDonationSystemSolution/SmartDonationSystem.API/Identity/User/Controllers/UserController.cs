using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDonationSystem.Core.Auth.Interfaces;

namespace SmartDonationSystem.API.Identity.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly IAuthOcrService _authOcrService;

        public UserController(IAuthServices authServices, IAuthOcrService authOcrService)
        {
            _authServices = authServices;
            _authOcrService = authOcrService;
        }

    }
}
