using Microsoft.AspNetCore.Http;
using SmartDonationSystem.Core.Auth.DTOs;
using SmartDonationSystem.Shared.Responses;

namespace SmartDonationSystem.Core.Auth.Interfaces;

public interface IAuthOcrService
{
    // Task<Result<ExtractedIdentityDto>> ExtractAsync(IFormFile image);
    Task<ExtractedIdentityDto> ExtractAsync(IFormFile image);
}
