namespace Retry.API.Services.Interfaces
{
    public interface IRandomNumberService
    {
        Task<int> GetRandomNumberAsync();
        Task<int> GetRandomNumberWithRetryAsync(int maxAttempt = 3);
    }
}
