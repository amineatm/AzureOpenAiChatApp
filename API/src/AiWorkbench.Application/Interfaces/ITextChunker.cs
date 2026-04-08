namespace AiWorkbench.Application.Interfaces
{
    public interface ITextChunker
    {
        List<string> Chunk(string text, int maxWords = 200);
    }
}
