using Retry.API.Extensions;
using Retry.API.Services.Interfaces;

namespace Retry.API.Services.Implements
{
    public class RandomNumberService : IRandomNumberService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RandomNumberService> _logger;
        public RandomNumberService(IHttpClientFactory httpClientFactory, ILogger<RandomNumberService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<int> GetRandomNumberAsync()
        {
            var url = "https://www.random.org/integers/?num=1&min=1&max=10000&col=1&base=10&format=plain&rnd=new";
            var httpClient = _httpClientFactory.CreateClient();
            using var responseMessage = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            responseMessage.EnsureSuccessStatusCode();

            var result = await responseMessage.Content.ReadAsStringAsync();

            return Convert.ToInt32(result);
        }

        public async Task<int> GetRandomNumberWithRetryAsync(int maxAttempt = 3)
        {
            var result = -1;
            try
            {
                _logger.LogInformation($"Start method GetRandomNumberWithRetryAsync ");
                await RetryExtension.DoRetryReturnResultAsync(async () =>
                {
                    result = await GetRandomNumberAsync();
                    _logger.LogInformation($"Result is {result}");
                    if (result % 2 == 0)
                    {
                        _logger.LogWarning($"{result} is even number");
                        throw new ArgumentException($"Invalid number: {result} is even number.");
                    }
                    return result;
                },
                exceptionType: (ex) => true,
                retryInterval: TimeSpan.FromSeconds(10),
                maxAttemptCount: maxAttempt,
                actionWhenException: (iRetry) => _logger.LogWarning($"Method GetRandomNumberWithRetryAsync - Retry {iRetry} times "));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception - GetRandomNumberWithRetryAsync with number {result}  due to {ex.ToString()}");
                throw;
            }

            return result;
        }
    }
}
