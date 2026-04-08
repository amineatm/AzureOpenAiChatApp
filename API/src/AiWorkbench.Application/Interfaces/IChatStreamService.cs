using System.Runtime.CompilerServices;

namespace AiWorkbench.Application.Interfaces
{
    public interface IChatStreamService
    {
        IAsyncEnumerable<string> StreamAsync(string text, CancellationToken ct = default);
    }
}
