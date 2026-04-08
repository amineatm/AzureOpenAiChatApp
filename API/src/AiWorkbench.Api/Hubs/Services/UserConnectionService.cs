using AiWorkbench.Api.Services;
using System.Collections.Concurrent;

namespace AiWorkbench.Api.Hubs.Services
{
    public class UserConnectionService : IUserConnectionService
    {
        private readonly ConcurrentDictionary<Guid, HashSet<string>> connections = new();

        public void AddConnection(Guid userId, string connectionId)
        {
            connections.AddOrUpdate(
                userId,
                _ => [connectionId],
                (_, set) =>
                {
                    set.Add(connectionId);
                    return set;
                });
        }

        public void RemoveConnection(Guid userId, string connectionId)
        {
            if (connections.TryGetValue(userId, out var set))
            {
                set.Remove(connectionId);
                if (set.Count == 0)
                    connections.TryRemove(userId, out _);
            }
        }

        public IReadOnlyList<string> GetConnections(Guid userId)
        {
            return connections.TryGetValue(userId, out var set) ? set.ToList() : [];
        }
    }
}
