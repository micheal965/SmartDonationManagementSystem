using Microsoft.EntityFrameworkCore;
using SmartDonationSystem.DataAccess;

namespace SmartDonationSystem.Services.Modules.FileExtractionModule
{
    public class FileExtractionJob
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly FileExtractionOrchestrator _fileExtractor;

        public FileExtractionJob(ApplicationDbContext dbContext, FileExtractionOrchestrator fileExtractor)
        {
            _dbContext = dbContext;
            _fileExtractor = fileExtractor;
        }

        public async Task ExtractAndSaveTextAsync(int postId)
        {
            var post = await _dbContext.Posts.Include(p => p.PostAttachments).FirstOrDefaultAsync(p => p.Id == postId);
            if (post == null || post.PostAttachments == null || !post.PostAttachments.Any())
                return;

            foreach (var attachment in post.PostAttachments)
            {

                // Extract text from each file URL
                string extractedText = await _fileExtractor.ExtractTextFromUrlAsync(attachment.AttachmentUrl);
                // Append to post content
                post.AiSummary += "\n" + extractedText;
            }
            await _dbContext.SaveChangesAsync();
        }
    }
}
