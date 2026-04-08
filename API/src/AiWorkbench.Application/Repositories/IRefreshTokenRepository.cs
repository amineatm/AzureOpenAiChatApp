namespace AiWorkbench.Application.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken ct = default);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
        Task UpdateAsync(RefreshToken token, CancellationToken ct = default);
        Task<List<RefreshToken>> GetAllForUserAsync(Guid userId, CancellationToken ct = default);
        Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default);
    }

}
