using Microsoft.AspNetCore.Http;
using SmartDonationSystem.Core.Auth.Interfaces;
using SmartDonationSystem.DataAccess;

namespace SmartDonationSystem.Services.Identity;

public class UserServices : IUserServices
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserServices(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _applicationDbContext = applicationDbContext;
        _httpContextAccessor = httpContextAccessor;
    }

}
