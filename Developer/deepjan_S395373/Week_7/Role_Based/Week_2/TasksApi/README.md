# Tasks API — Developer Week 2

ASP.NET Core Web API using Entity Framework Core and SQLite.

## Endpoints

- GET /api/tasks — list tasks
- GET /api/tasks/{id} — get one task
- POST /api/tasks — create a task
- PUT /api/tasks/{id} — update a task
- DELETE /api/tasks/{id} — delete a task

## Run

    dotnet run --project . -- --urls http://localhost:5050

The SQLite database is created locally and excluded from Git.

## Verification

Manual API smoke test: create, read, update, delete, and confirm deletion.
The dependency audit reported no vulnerable packages from the current sources.

## Architecture

HTTP request → ASP.NET Core endpoint → EF Core DbContext → SQLite.

This is a local coursework prototype using EnsureCreated.
EF Core migrations and deeper architecture will be studied later.
