using System.Text;
using ApiGateway.Auth;
using ApiGateway.RateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Redis
var redisConn = builder.Configuration["Redis:ConnectionString"]!;
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisConn));

builder.Services.AddSingleton<RedisRateLimiter>();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                                           Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<JwtService>();

// YARP — lê config de appsettings.json
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Pipeline (ordem importa!)
app.UseMiddleware<RateLimitMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Endpoint de login — gera JWT (simplificado: sem verificar senha aqui)
app.MapPost("/auth/token", (LoginRequest req, JwtService jwt) =>
{
    // Em produção: validar credenciais no UserService antes de emitir o token
    if (string.IsNullOrWhiteSpace(req.Email))
        return Results.BadRequest("E-mail obrigatório.");

    var token = jwt.Generate(req.UserId, req.Email);

    return Results.Ok(new { token });
}).AllowAnonymous();

// Rotas protegidas passam pelo proxy (YARP) com autenticação obrigatória
app.MapReverseProxy(pipeline =>
{
    pipeline.Use(async (ctx, next) =>
    {
        if (!ctx.User.Identity?.IsAuthenticated ?? true)
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await ctx.Response.WriteAsJsonAsync(new { error = "Token inválido ou ausente." });
            return;
        }
        await next();
    });
});

app.Run();

record LoginRequest(Guid UserId, string Email);
