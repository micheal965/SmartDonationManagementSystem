using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using SmartDonationSystem.Core.Cloud;
using SmartDonationSystem.DataAccess;

namespace SmartDonationSystem.Services.Identity
{

    internal class ProfileImageJob
    {
        private readonly IWebHostEnvironment _env;
        private readonly ICloudinaryServices _cloudinaryService;
        private readonly ApplicationDbContext _context;
        public ProfileImageJob(ICloudinaryServices cloudinaryService, ApplicationDbContext context, IWebHostEnvironment env)
        {
            _cloudinaryService = cloudinaryService;
            _context = context;
            _env = env;
        }
        [AutomaticRetry(Attempts = 3)]
        public async Task Handle(string userId, string relativePath)
        {
            var fullPath = Path.Combine(_env.WebRootPath, relativePath);

            if (!File.Exists(fullPath))
                return;

            byte[] fileBytes = await File.ReadAllBytesAsync(fullPath);
            await using var memoryStream = new MemoryStream(fileBytes);

            var file = new FormFile(memoryStream, 0, memoryStream.Length, "file", Path.GetFileName(fullPath))
            {
                Headers = new HeaderDictionary(),
                ContentType = GetContentType(fullPath)
            };

            var result = await _cloudinaryService.UploadImageAsync(file);

            if (!result.isSucceded) return;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return;

            user.PictureUrl = result.url;
            await _context.SaveChangesAsync();

            File.Delete(fullPath);
        }


        public IFormFile GetFormFileFromPath(string fullPath)
        {
            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found", fullPath);

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

            var formFile = new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(fullPath))
            {
                Headers = new HeaderDictionary(),
                ContentType = GetContentType(fullPath)
            };

            return formFile;
        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
                contentType = "application/octet-stream";

            return contentType;
        }

    }
}
