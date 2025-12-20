using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartDonationSystem.Core.Auth.DTOs;
using SmartDonationSystem.Core.Auth.Interfaces;
using SmartDonationSystem.Core.Auth.Models;
using SmartDonationSystem.Core.DTOs;
using SmartDonationSystem.DataAccess;
using SmartDonationSystem.Shared.Enums;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Services.Identity;

public class AuthServices : IAuthServices
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserServices _userServices;

    public AuthServices(
                        IConfiguration configuration,
                        IHttpContextAccessor httpContextAccessor,
                        UserManager<ApplicationUser> userManager,
                        SignInManager<ApplicationUser> signInManager,
                        ApplicationDbContext applicationDbContext,
                        IUserServices userServices)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _signInManager = signInManager;
        _applicationDbContext = applicationDbContext;
        _userServices = userServices;
    }

    public async Task<Result<ApplicationUser>> RegisterAsync(RegisterRequestDto request)
    {
        ApplicationUser user;
        // if (request.InputType == RegisterInputType.Manual)
        user = MapManualData(request);
        // else
        //     user = ExtractDataFromSource(request);

        return await RegisterAsync(user, request.Role);
    }
    public async Task<Result<LoginOrRotateTokenResponseDto>> LoginAsync(LoginRequestDto loginRequestDto)
    {
        ApplicationUser? user = await _userManager.Users
                            .Include(u => u.RefreshTokens)
                            .FirstOrDefaultAsync(u => u.IdentityNumber.Equals(loginRequestDto.IdentityNumber));
        if (user == null)
            return Result<LoginOrRotateTokenResponseDto>.BadRequest("Invalid login attempt!");

        SignInResult result = await _signInManager.PasswordSignInAsync(user, loginRequestDto.Password, false, false);
        if (!result.Succeeded)
            return Result<LoginOrRotateTokenResponseDto>.BadRequest("Invalid login attempt!");

        //Track IPAddress in UserLoginHistory table
        await _userServices.SaveLoginAttemptAsync(loginRequestDto.IdentityNumber);

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
        if (!string.IsNullOrEmpty(RefreshTokenObj.Token))
            AppendRefreshTokenInCookies(RefreshTokenObj.Token, RefreshTokenObj.expiryDate);

        //Get user Roles
        var roles = await _userManager.GetRolesAsync(user);

        return Result<LoginOrRotateTokenResponseDto>.Ok(new LoginOrRotateTokenResponseDto()
        {
            Username = user.UserName,
            Token = await CreateJwtWebTokenAsync(user),
            Roles = roles.ToList(),
        });
    }
    private ApplicationUser MapManualData(RegisterRequestDto request)
    {
        return new ApplicationUser
        {
            IdentityNumber = request.IdentityNumber,
            UserName = request.UserName,
            BirthDate = request.BirthDate,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
        };
    }
    // private ApplicationUser ExtractDataFromSource(RegisterRequestDto request)
    // {
    //     // مثال: OCR / API / National ID Reader
    //     return new ApplicationUser
    //     {
    //         IdentityNumber = request.ExtractedIdentityNumber,
    //         UserName = request.ExtractedName,
    //         BirthDate = request.ExtractedBirthDate,
    //         PhoneNumber = request.PhoneNumber,
    //         Address = request.Address,
    //     };
    // }
    private async Task<Result<ApplicationUser>> RegisterAsync(ApplicationUser user, UserRole role)
    {
        if (user.IdentityNumber == null || user.UserName == null || user.BirthDate == null)
            return Result<ApplicationUser>.BadRequest("Identity Number, User Name, and Birth Date are required.");

        ApplicationUser? existingUser = await _applicationDbContext.ApplicationUsers
                                        .FirstOrDefaultAsync(u => u.IdentityNumber == user.IdentityNumber.Trim());
        if (existingUser != null) return Result<ApplicationUser>.BadRequest("A user with this Identity Number already exists.");

        //Upload profile picture on cloudinary

        ApplicationUser applicationUser = new ApplicationUser()
        {
            IdentityNumber = user.IdentityNumber,
            UserName = user.UserName,
            BirthDate = user.BirthDate,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            PictureUrl = "" //from cloudinary
        };

        IdentityResult createResult = await _userManager.CreateAsync(applicationUser);
        IdentityResult roleResult = await _userManager.AddToRoleAsync(applicationUser, Enum.GetName(role));
        if (!createResult.Succeeded || !roleResult.Succeeded)
        {
            var errors = createResult.Errors.Concat(roleResult.Errors)
                                    .Select(e => e.Description).ToList();
            string errorMessage = string.Join("; ", errors);

            return Result<ApplicationUser>.BadRequest(errorMessage);
        }
        return Result<ApplicationUser>.Created(applicationUser);
    }
    public async Task<Result<LoginOrRotateTokenResponseDto>> RotateRefreshTokenAsync(string token)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));
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

        //Delete old RefreshTtoken and save the new refresh token into cookies=>(Append)
        AppendRefreshTokenInCookies(newRefreshTokenObj.Token, newRefreshTokenObj.expiryDate);

        //get roles from db for that user
        var roles = await _userManager.GetRolesAsync(user);
        return Result<LoginOrRotateTokenResponseDto>.Ok(new LoginOrRotateTokenResponseDto
        {
            Username = user.UserName,
            Token = await CreateJwtWebTokenAsync(user),
            Roles = roles.ToList(),
        }, "Token Rotated successfully!");
    }


    //Token Aggregate
    private async Task<string> CreateJwtWebTokenAsync(ApplicationUser user)
    {
        //Authentication Claims
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id),
            new Claim(ClaimTypes.Name,user.UserName),
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
}
