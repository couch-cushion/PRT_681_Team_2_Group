using Microsoft.EntityFrameworkCore;
using TasksApi.Data;
using TasksApi.Models;

var builder = WebApplication.CreateBuilder(args);

string databasePath = Path.Combine(
    AppContext.BaseDirectory,
    "tasks.db"
);

builder.Services.AddDbContext<TasksDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));

// Allow our local React development server to call the API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("React", policy =>
        policy.WithOrigins(
            "http://localhost:5173",
            "http://127.0.0.1:5173")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

// Create the database for this small coursework application.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TasksDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors("React");

app.MapGet("/", () => "Tasks API is running.");

// READ ALL
app.MapGet("/api/tasks", async (TasksDbContext db) =>
    await db.Tasks
        .AsNoTracking()
        .OrderBy(t => t.Id)
        .ToListAsync());

// READ ONE
app.MapGet("/api/tasks/{id:int}", async Task<IResult>
    (int id, TasksDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);

    return task is null
        ? Results.NotFound()
        : Results.Ok(task);
});

// CREATE
app.MapPost("/api/tasks", async Task<IResult>
    (TaskRequest request, TasksDbContext db) =>
{
    string? title = request.Title?.Trim();

    if (string.IsNullOrEmpty(title) || title.Length > 200)
        return Results.BadRequest(new
        {
            error = "Title must be between 1 and 200 characters."
        });

    var task = new TaskItem
    {
        Title = title,
        IsCompleted = request.IsCompleted
    };

    db.Tasks.Add(task);
    await db.SaveChangesAsync();

    return Results.Created($"/api/tasks/{task.Id}", task);
});

// UPDATE
app.MapPut("/api/tasks/{id:int}", async Task<IResult>
    (int id, TaskRequest request, TasksDbContext db) =>
{
    string? title = request.Title?.Trim();

    if (string.IsNullOrEmpty(title) || title.Length > 200)
        return Results.BadRequest(new
        {
            error = "Title must be between 1 and 200 characters."
        });

    var task = await db.Tasks.FindAsync(id);

    if (task is null)
        return Results.NotFound();

    task.Title = title;
    task.IsCompleted = request.IsCompleted;

    await db.SaveChangesAsync();

    return Results.Ok(task);
});

// DELETE
app.MapDelete("/api/tasks/{id:int}", async Task<IResult>
    (int id, TasksDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);

    if (task is null)
        return Results.NotFound();

    db.Tasks.Remove(task);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

public record TaskRequest(string? Title, bool IsCompleted);