namespace SmartDonationSystem.Services.Modules.AI.SummarizationModule
{
    public class ChunkingService
    {
        private readonly int _chunkSize;

        public ChunkingService(int chunkSize = 1000)
        {
            _chunkSize = chunkSize;
        }

        public List<string> ChunkText(string text)
        {
            var chunks = new List<string>();
            var words = text.Split(' ');
            for (int i = 0; i < words.Length; i += _chunkSize)
            {
                var chunk = string.Join(" ", words.Skip(i).Take(_chunkSize));
                chunks.Add(chunk);
            }
            return chunks;
        }
    }
}
