using StackExchange.Redis;

namespace ApiGateway.RateLimit;

// Rate limiter baseado em sliding window com Redis
// Estratégia: cada IP tem um contador com TTl - simples e eficaz
public sealed class RedisRateLimiter(IConnectionMultiplexer redis)
{
    private const int MaxRequests = 60; // por janela
    private const int WindowSeconds = 60;

    public async Task<(bool Allowed, int Remaining)> CheckAsync(string clientKey)
    {
        var db = redis.GetDatabase();
        var key = $"rate:{clientKey}";

        var count = await db.StringIncrementAsync(key);

        if (count == 1)
            await db.KeyExpireAsync(key, TimeSpan.FromSeconds(WindowSeconds));

        var allowed = count <= MaxRequests;
        var remainig = Math.Max(0, MaxRequests - (int)count);

        return (allowed, remainig);
    }
}
