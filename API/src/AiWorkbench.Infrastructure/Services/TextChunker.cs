using AiWorkbench.Application.Interfaces;

namespace AiWorkbench.Infrastructure.Services
{
    public class TextChunker : ITextChunker
    {
        public List<string> Chunk(string text, int maxWords = 200)
        {
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var chunks = new List<string>();
            var current = new List<string>();

            foreach (var w in words)
            {
                current.Add(w);
                if (current.Count >= maxWords)
                {
                    chunks.Add(string.Join(" ", current));
                    current.Clear();
                }
            }

            if (current.Count > 0)
                chunks.Add(string.Join(" ", current));

            return chunks;
        }
    }
}
