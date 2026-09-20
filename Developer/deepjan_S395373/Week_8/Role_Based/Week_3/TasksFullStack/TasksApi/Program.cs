using Microsoft.EntityFrameworkCore;
using TasksApi.Data;
using TasksApi.Models;

var builder = WebApplication.CreateBuilder(args);

string dataDirectory = Environment.GetEnvironmentVariable("DATA_DIR")
    ?? AppContext.BaseDirectory;

Directory.CreateDirectory(dataDirectory);
string databasePath = Path.Combine(dataDirectory, "tasks.db");

builder.Services.AddDbContext<TasksDbContext>(options =>
    options.UseSqlite($"Data Source={databasePath}"));

builder.Services.AddControllersWithViews();

builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalReact", policy =>
        policy.WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TasksDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors("LocalReact");
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "mvc-learning",
    pattern: "mvc/{action=Index}",
    defaults: new { controller = "Learning" });

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    application = "Week 8 Tasks Full Stack"
}));

app.MapGet("/api/tasks", async (TasksDbContext db) =>
    await db.Tasks
        .AsNoTracking()
        .OrderBy(t => t.Id)
        .ToListAsync());

app.MapGet("/api/tasks/{id:int}", async Task<IResult>
    (int id, TasksDbContext db) =>
{
    var task = await db.Tasks.FindAsync(id);

    return task is null
        ? Results.NotFound()
        : Results.Ok(task);
});

app.MapPost("/api/tasks", async Task<IResult>
    (TaskRequest request, TasksDbContext db) =>
{
    string? title = request.Title?.Trim();

    if (string.IsNullOrEmpty(title) || title.Length > 200)
    {
        return Results.BadRequest(new
        {
            error = "Title must be between 1 and 200 characters."
        });
    }

    var task = new TaskItem
    {
        Title = title,
        IsCompleted = request.IsCompleted
    };

    db.Tasks.Add(task);
    await db.SaveChangesAsync();

    return Results.Created($"/api/tasks/{task.Id}", task);
});

app.MapPut("/api/tasks/{id:int}", async Task<IResult>
    (int id, TaskRequest request, TasksDbContext db) =>
{
    string? title = request.Title?.Trim();

    if (string.IsNullOrEmpty(title) || title.Length > 200)
    {
        return Results.BadRequest(new
        {
            error = "Title must be between 1 and 200 characters."
        });
    }

    var task = await db.Tasks.FindAsync(id);

    if (task is null)
        return Results.NotFound();

    task.Title = title;
    task.IsCompleted = request.IsCompleted;

    await db.SaveChangesAsync();
    return Results.Ok(task);
});

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

app.MapFallbackToFile("index.html");

app.Run();

public record TaskRequest(string? Title, bool IsCompleted);
