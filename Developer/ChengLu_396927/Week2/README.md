# Tasks CRUD Full-Stack App

This project contains two stages of the practice:

1. A C# data-access practice using a local SQLite `Tasks` database.
2. An ASP.NET Core Web API with Entity Framework Core and a React/Vite frontend.

## Backend

Requirements: .NET Core 3.1 SDK or a newer compatible SDK.

```bash
dotnet run --project hello.csproj --urls http://localhost:5000
```

The API is available at `http://localhost:5000/api/tasks` and stores local data in `tasks.db`.

Supported endpoints:

- `GET /api/tasks`
- `GET /api/tasks/{id}`
- `POST /api/tasks`
- `PUT /api/tasks/{id}`
- `DELETE /api/tasks/{id}`

## Frontend

In another terminal:

```bash
cd frontend
npm install
npm run dev
```

Open `http://localhost:5173` in a browser. The Vite development proxy forwards `/api` requests to the backend on port 5000.

## Production frontend build

```bash
cd frontend
npm run build
```
