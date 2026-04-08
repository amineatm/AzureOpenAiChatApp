namespace AiWorkbench.Api.Services
{
    public interface IUserConnectionService
    {
        void AddConnection(Guid userId, string connectionId);
        void RemoveConnection(Guid userId, string connectionId);
        IReadOnlyList<string> GetConnections(Guid userId);
    }
}
