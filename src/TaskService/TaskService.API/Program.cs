using Microsoft.EntityFrameworkCore;
using TaskService.Application.Abstractions;
using TaskService.Application.Commands;
using TaskService.Application.DTOs;
using TaskService.Application.Queries;
using TaskService.Domain.Exceptions;
using TaskService.Domain.Repositories;
using TaskService.Infrastructure.Persistence;
using TaskService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// Repositório
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

// Command handlers
builder.Services.AddScoped<ICommandHandler<CreateTaskCommand, TaskDto>, CreateTaskHandler>();
builder.Services.AddScoped<ICommandHandler<CompleteTaskCommand, TaskDto>, CompleteTaskHandler>();
builder.Services.AddScoped<ICommandHandler<DeleteTaskCommand, bool>, DeleteTaskHandler>();

// Query handlers
builder.Services.AddScoped<IQueryHandler<GetTaskByIdQuery, TaskDto?>, GetTaskByIdHandler>();
builder.Services.AddScoped<IQueryHandler<GetTasksByUserQuery, IEnumerable<TaskDto>>, GetTasksByUserHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

// Commands
app.MapPost("/tasks", async (
    CreateTaskCommand cmd,
    ICommandHandler<CreateTaskCommand, TaskDto> handler,
    CancellationToken ct) =>
{
    var result = await handler.HandleAsync(cmd, ct);
    return Results.Created($"/tasks/{result.Id}", result);
});

app.MapPatch("/tasks/{id:guid}/complete", async (
    Guid id,
    ICommandHandler<CompleteTaskCommand, TaskDto> handler,
    CancellationToken ct) =>
{
    try
    {
        var result = await handler.HandleAsync(new CompleteTaskCommand(id), ct);
        return Results.Ok(result);
    }
    catch (NotFoundException ex) { return Results.NotFound(ex.Message); }
});

app.MapDelete("/tasks/{id:guid}", async (
    Guid id,
    ICommandHandler<DeleteTaskCommand, bool> handler,
    CancellationToken ct) =>
{
    try
    {
        await handler.HandleAsync(new DeleteTaskCommand(id), ct);
        return Results.NoContent();
    }
    catch (NotFoundException ex) { return Results.NotFound(ex.Message); }
});

// Queries
app.MapGet("/tasks/{id:guid}", async (
    Guid id,
    IQueryHandler<GetTaskByIdQuery, TaskDto?> handler,
    CancellationToken ct) =>
    await handler.HandleAsync(new GetTaskByIdQuery(id), ct) is { } task
        ? Results.Ok(task)
        : Results.NotFound());

app.MapGet("/tasks/user/{userId:guid}", async (
    Guid userId,
    IQueryHandler<GetTasksByUserQuery, IEnumerable<TaskDto>> handler,
    CancellationToken ct) =>
    Results.Ok(await handler.HandleAsync(new GetTasksByUserQuery(userId), ct)));

app.Run();
