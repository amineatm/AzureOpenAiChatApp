namespace AiWorkbench.Application.Interfaces
{
    public interface IFileTextExtractor
    {
        string ExtractText(string fileName, byte[] bytes);
    }
}
