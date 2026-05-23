using Microsoft.EntityFrameworkCore;
using UserService.Application.UseCases;
using UserService.Domain.Repositories;
using UserService.Infrastructure.Persistence;
using UserService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Banco de Dados
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Injeção de dependência
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<GetUserUseCase>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica migrations automaticamente na inicialização
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

// Endpoints
app.MapGet("/users", async (GetUserUseCase uc, CancellationToken ct) =>
    Results.Ok(await uc.GetAllAsync(ct)));

app.MapGet("/users/{id:guid}", async (Guid id, GetUserUseCase uc, CancellationToken ct) =>
    await uc.ExecuteAsync(id, ct) is { } user
        ? Results.Ok(user)
        : Results.NotFound());

app.MapPost("/users", async (
    UserService.Application.DTOs.CreateUserRequest req,
    CreateUserUseCase uc,
    CancellationToken ct) =>
{
    var user = await uc.ExecuteAsync(req, ct);
    return Results.Created($"/users/{user.Id}", user);
});

app.Run();
