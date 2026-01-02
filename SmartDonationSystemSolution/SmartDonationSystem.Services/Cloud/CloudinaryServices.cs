using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SmartDonationSystem.Core.Cloud;

namespace SmartDonationSystem.Services.Cloud;

public class CloudinaryServices : ICloudinaryServices
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryServices(IConfiguration configuration)
    {
        var account = new Account(
            configuration["CloudinarySettings:CloudName"],
            configuration["CloudinarySettings:ApiKey"],
            configuration["CloudinarySettings:ApiSecret"]
        );
        _cloudinary = new Cloudinary(account);
    }
    //Upload more than one image
    public async Task<(bool isSucceded, List<string> urls)> UploadImagesAsync(List<IFormFile> files)
    {
        var uploadedUrls = new List<string>();
        foreach (var file in files)
        {
            var uploadedUrl = await UploadImageAsync(file);
            if (uploadedUrl.isSucceded)
                uploadedUrls.Add(uploadedUrl.url);
        }
        return (uploadedUrls.Count == files.Count, uploadedUrls);
    }
    //Upload one image
    public async Task<(bool isSucceded, string url)> UploadImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0) return (false, null);

        //Restrict send only images
        if (!IsValidImageFile(file))
            throw new Exception($"Invalid file: {file.FileName} (Only images are allowed)");

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);

        return (uploadResult.StatusCode == HttpStatusCode.OK, uploadResult.SecureUrl.ToString());
    }
    //Delete more than one image
    public async Task<bool> DeleteImagesAsync(List<string> imagesUrls)
    {
        List<string> publicIds = new List<string>();

        foreach (var imagesUrl in imagesUrls)
            publicIds.Add(GetPublicIdfromUrl(imagesUrl));

        if (publicIds == null || publicIds.Count == 0) return true;

        var deletionParams = new DelResParams
        {
            PublicIds = publicIds,
            Invalidate = true
        };

        var result = await _cloudinary.DeleteResourcesAsync(deletionParams);
        return result.Deleted.Count == imagesUrls.Count; // Check if all images were deleted
    }
    //Delete one image
    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return false;

        var publicId = GetPublicIdfromUrl(imageUrl);
        if (string.IsNullOrEmpty(publicId)) return false;

        var deleteParams = new DeletionParams(publicId);

        var imageDestroyResult = await _cloudinary.DestroyAsync(deleteParams);
        return imageDestroyResult.Result == "ok";
    }
    //Upload more than one files and no validation for image
    public async Task<List<string>> UploadFilesAsync(List<IFormFile> files, string category)
    {
        if (files == null || files.Count == 0) throw new Exception("No files provided");

        var uploadedUrls = new List<string>();

        foreach (var file in files)
        {
            if (file.Length == 0) continue; // Skip empty files

            await using var stream = file.OpenReadStream();
            var uploadParams = new RawUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = $"{category}/{Guid.NewGuid()}"
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            uploadedUrls.Add(uploadResult.SecureUrl.ToString());
        }
        return uploadedUrls;
    }
    //Get image Url
    public async Task<string> GetImageAsync(string ImageUrl)
    {
        try
        {
            var publicId = GetPublicIdfromUrl(ImageUrl);
            var uri = _cloudinary.Api.UrlImgUp.BuildUrl(publicId);
            return uri.ToString();
        }
        catch
        {
            throw new Exception("There is no Image");
        }
    }
    private string GetPublicIdfromUrl(string url)
    {
        Uri uri = new Uri(url);
        return Path.GetFileNameWithoutExtension(uri.AbsolutePath);
    }
    private bool IsValidImageFile(IFormFile file)
    {
        var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/png", "image/gif", "image/bmp", "image/webp" };

        // Check MIME type
        if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
            return false;

        // Check file extension
        var extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension))
            return false;

        return true;
    }
}
