using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace SmartDonationSystem.Core.Cloud;

public interface ICloudinaryServices
{
    Task<List<string>> UploadImagesAsync(List<IFormFile> files);
    Task<string> UploadImageAsync(IFormFile file);
    Task<bool> DeleteImagesAsync(List<string> imagesUrls);
    Task<bool> DeleteImageAsync(string imageUrl);
    Task<List<string>> UploadFilesAsync(List<IFormFile> files, string category);
    Task<string> GetImageAsync(string ImageUrl);
}
