using Microsoft.AspNetCore.Mvc;

namespace SmartDonationSystem.API.Modules.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = AppRoles.Admin)]
    public class DashboardManagementController : ControllerBase
    {
    }
}
