namespace ApiGateway.RateLimit;

public sealed class RateLimitMiddleware(RequestDelegate next, RedisRateLimiter limiter)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        // Chave por IP - em produção usaria o userId do JWT
        var clientKey = ctx.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        var (allowed, remaining) = await limiter.CheckAsync(clientKey);

        ctx.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();

        if (!allowed)
        {
            ctx.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await ctx.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit excedido. Tente em 1 minuto."
            });
            return;
        }

        await next(ctx);
    }
}
