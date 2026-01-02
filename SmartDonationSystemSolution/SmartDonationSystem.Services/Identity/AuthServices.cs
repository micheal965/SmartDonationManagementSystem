using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartDonationSystem.Core.Auth.DTOs;
using SmartDonationSystem.Core.Auth.Interfaces;
using SmartDonationSystem.Core.Auth.Models;
using SmartDonationSystem.Core.Cloud;
using SmartDonationSystem.Core.DTOs;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Identity;

public class AuthServices : IAuthServices
{
    private static readonly HashSet<string> BlacklistedTokens = new();
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ICloudinaryServices _cloudinaryServices;

    public AuthServices(IConfiguration configuration,
                        IHttpContextAccessor httpContextAccessor,
                        UserManager<ApplicationUser> userManager,
                        RoleManager<IdentityRole> roleManager,
                        SignInManager<ApplicationUser> signInManager,
                        ApplicationDbContext applicationDbContext,
                        ICloudinaryServices cloudinaryServices)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _applicationDbContext = applicationDbContext;
        _cloudinaryServices = cloudinaryServices;
    }
    public async Task<Result<LoginOrRotateTokenResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        ApplicationUser? user = await _userManager.Users
                            .Include(u => u.RefreshTokens)
                            .FirstOrDefaultAsync(u => u.IdentityNumber.Equals(loginRequestDto.IdentityNumber));
        if (user == null)
            return Result<LoginOrRotateTokenResponseDto>.Unauthorized("Invalid login attempt!");

        SignInResult checkPasswordResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequestDto.Password, false);
        if (!checkPasswordResult.Succeeded)
            return Result<LoginOrRotateTokenResponseDto>.Unauthorized("Invalid login attempt!");

        //Track IPAddress in UserLoginHistory table
        await SaveLoginAttemptAsync(loginRequestDto.IdentityNumber);

        //Check for RefreshToken
        var RefreshTokenObj = new RefreshToken();
        if (user.RefreshTokens.Any(t => t.isActive))
            RefreshTokenObj = user.RefreshTokens.FirstOrDefault(t => t.isActive);
        else
        {
            //if there is no active RefreshToken for that user so generate new one 
            RefreshTokenObj = GenerateRefreshTokenObject();
            user.RefreshTokens.Add(RefreshTokenObj);
            await _userManager.UpdateAsync(user);
        }

        //set refresh token if not empty in the cookies 
        if (!string.IsNullOrEmpty(RefreshTokenObj?.Token))
            AppendRefreshTokenInCookies(RefreshTokenObj.Token, RefreshTokenObj.expiryDate);

        //Get user Roles
        var roles = await _userManager.GetRolesAsync(user);

        return Result<LoginOrRotateTokenResponseDto>.Ok(new LoginOrRotateTokenResponseDto()
        {
            Token = await CreateJwtWebTokenAsync(user),
        });
    }
    public async Task<Result<RegisterResultDto>> RegisterAsync(RegisterRequestDto requestDto)
    {
        ApplicationUser? existingUser = await _applicationDbContext.ApplicationUsers
                                        .FirstOrDefaultAsync(u => u.IdentityNumber == requestDto.IdentityNumber.Trim());
        if (existingUser != null) return Result<RegisterResultDto>.BadRequest("A user with this Identity Number already exists.");

        //Upload profile picture on cloudinary
        var uploadResult = await _cloudinaryServices.UploadImageAsync(requestDto.ProfilePicture);

        ApplicationUser applicationUser = new ApplicationUser()
        {
            IdentityNumber = requestDto.IdentityNumber,
            FullName = requestDto.FullName,
            UserName = Guid.NewGuid().ToString(),
            BirthDate = requestDto.BirthDate,
            PhoneNumber = requestDto.PhoneNumber,
            Address = requestDto.Address,
            PictureUrl = uploadResult.isSucceded ? uploadResult.url : null,
        };

        //Check if the role exists
        if (!await _roleManager.RoleExistsAsync(requestDto.Role) ||
            requestDto.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase)) return Result<RegisterResultDto>.BadRequest("Invalid role");

        //Create transaction to ensure the user and its role created or not at all
        using var transaction = await _applicationDbContext.Database.BeginTransactionAsync();

        IdentityResult createResult = await _userManager.CreateAsync(applicationUser, requestDto.Password);
        if (!createResult.Succeeded)
        {
            await transaction.RollbackAsync();
            return Result<RegisterResultDto>.BadRequest("Registration failed", createResult.Errors.Select(e => e.Description).ToList());
        }
        IdentityResult roleResult = await _userManager.AddToRoleAsync(applicationUser, requestDto.Role);
        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(applicationUser);
            await transaction.RollbackAsync();

            return Result<RegisterResultDto>.BadRequest("Registration failed", roleResult.Errors.Select(e => e.Description).ToList());
        }
        await transaction.CommitAsync();
        return Result<RegisterResultDto>.Created(applicationUser.Adapt<RegisterResultDto>());
    }
    public async Task<Result<LoginOrRotateTokenResponseDto>> RotateRefreshTokenAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Result<LoginOrRotateTokenResponseDto>.BadRequest("Token is required");

        var user = await _userManager.Users.Include(u => u.RefreshTokens)
                                           .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
        if (user == null)
            return Result<LoginOrRotateTokenResponseDto>.BadRequest("Invalid Token");

        var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);
        if (!refreshToken.isActive)
            return Result<LoginOrRotateTokenResponseDto>.BadRequest("Invalid Token");

        //revoke that token and generate new one
        refreshToken.revokedOn = DateTime.UtcNow;

        var newRefreshTokenObj = GenerateRefreshTokenObject();
        user.RefreshTokens.Add(newRefreshTokenObj);
        await _userManager.UpdateAsync(user);

        //Delete old RefreshToken and save the new refresh token into cookies=>(Append)
        AppendRefreshTokenInCookies(newRefreshTokenObj.Token, newRefreshTokenObj.expiryDate);

        //get roles from db for that user
        var roles = await _userManager.GetRolesAsync(user);
        return Result<LoginOrRotateTokenResponseDto>.Ok(new LoginOrRotateTokenResponseDto
        {
            Token = await CreateJwtWebTokenAsync(user),
        }, "Token Rotated successfully!");
    }

    //Logout Services
    public async Task AddTokenBlacklistAsync(string token)
    {
        await Task.Delay(100);  // Simulate async I/O operation to add token to blacklist
        BlacklistedTokens.Add(token);
        var refreshTokenFromCookies = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];
        DeleteRefreshTokenFromCookies();

        //Revoke RefreshToken in the Database
        var refreshTokenObj = await _applicationDbContext.RefreshTokens
                                    .FirstOrDefaultAsync(rf => rf.Token == refreshTokenFromCookies);
        if (refreshTokenObj == null || refreshTokenObj.isExpired) return;

        refreshTokenObj.revokedOn = DateTime.UtcNow;
        _applicationDbContext.RefreshTokens.Update(refreshTokenObj);
        await _applicationDbContext.SaveChangesAsync();
    }
    public async Task<bool> IsTokenBlacklistedAsync(string token)
    {
        await Task.Delay(100); // Simulate a delay
        return BlacklistedTokens.Contains(token);
    }

    //Track IPAddress
    public async Task SaveLoginAttemptAsync(string IdentityNumber)
    {
        var user = await _applicationDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.IdentityNumber.Equals(IdentityNumber));
        if (user != null)
        {
            var ipAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString();

            // Check if the app is behind a proxy (e.g., Nginx, Cloudflare)
            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                //X-Forwarded-For: 203.0.113.45, 70.41.3.18, 150.172.238.178
                ipAddress = _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
            }

            ipAddress = ipAddress == "::1" ? "127.0.0.1" : ipAddress; // Convert ::1 to 127.0.0.1 if local
            await _applicationDbContext.UserLoginsHistory.AddAsync(new UserLoginHistory()
            {
                ApplicationUserId = user.Id,
                IpAddress = ipAddress ?? "",
                LoginTime = DateTime.UtcNow,
            });
            await _applicationDbContext.SaveChangesAsync();
        }
    }
    public async Task<Result<IReadOnlyList<UserLoginsHistoryResponseDto>>> GetLoginHistoryAsync(string userId)
    {
        IReadOnlyList<UserLoginHistory> userLoginsHistory = await _applicationDbContext.UserLoginsHistory
                                                                    .Where(lg => lg.ApplicationUserId == userId)
                                                                    .OrderByDescending(l => l.LoginTime).ToListAsync();
        return Result<IReadOnlyList<UserLoginsHistoryResponseDto>>.Ok(userLoginsHistory.Adapt<IReadOnlyList<UserLoginsHistoryResponseDto>>());
    }

    //Token Aggregate
    private async Task<string> CreateJwtWebTokenAsync(ApplicationUser user)
    {
        //Authentication Claims
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id),
            new Claim(ClaimTypes.Name,user.FullName),
            new Claim("NationalId",user.IdentityNumber),
        };

        //RoleClaims 
        var userRoles = await _userManager.GetRolesAsync(user);
        if (userRoles != null)
            foreach (var role in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(_configuration["JwtSettings:ExpiryMinutes"])),
            signingCredentials: cred
            );

        //write token and return
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private RefreshToken GenerateRefreshTokenObject()
    {
        var randomBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return new RefreshToken()
        {
            Token = Convert.ToBase64String(randomBytes),
            createdOn = DateTime.UtcNow,
            expiryDate = DateTime.UtcNow.AddDays(7),
        };
    }
    private void AppendRefreshTokenInCookies(string token, DateTime expires)
    {
        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,//csrf
            Secure = true,
            Expires = expires,
            SameSite = SameSiteMode.Strict,
        };
        _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", token, cookieOptions);
    }
    private void DeleteRefreshTokenFromCookies()
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Delete("refreshToken");
    }
}
