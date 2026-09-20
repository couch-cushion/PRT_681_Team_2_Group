# Week 8 — Full Stack Integration & Deployment

PRT681 / PRT585 Software Engineering: Practice — Developer Week 3

## Architecture

Browser -> ASP.NET Core container -> React static frontend + REST API -> EF Core -> SQLite

The React production build is copied into ASP.NET Core `wwwroot` during the Docker build. This lets one container serve both the frontend and API from the same origin.

## Features

- React task list
- Add, complete/uncomplete and delete tasks
- ASP.NET Core REST API
- EF Core with SQLite
- `/health` endpoint
- ASP.NET Core MVC controller + Razor view at `/mvc`
- Multi-stage Docker build
- Render/Azure-compatible dynamic port binding

## Local development without Docker

Run the API:

```powershell
cd TasksApi
dotnet run --urls http://localhost:5050
```

In another terminal, run React:

```powershell
cd TasksWeb
npm install
npm run dev
```

Vite proxies `/api` and `/health` to `http://localhost:5050`.

## Local Docker run

From this folder:

```powershell
docker build -t week8-tasks .
docker run --rm -p 10000:10000 week8-tasks
```

Open:

- App: `http://localhost:10000`
- MVC learning page: `http://localhost:10000/mvc`
- Health: `http://localhost:10000/health`
- API: `http://localhost:10000/api/tasks`

## Deploy to Render

1. Push this folder to GitHub.
2. In Render select **New -> Web Service** and connect the repository.
3. Choose the branch containing this work.
4. Select **Docker** as the runtime/language.
5. Set Dockerfile Path to:
   `Developer/deepjan_S395373/Week_8/Role_Based/Week_3/TasksFullStack/Dockerfile`
6. Create the web service.
7. After deployment, verify `/`, `/health`, and `/api/tasks`.

Render supplies a `PORT` environment variable; the Docker command binds ASP.NET Core to that port.

### SQLite note

The database file is created inside the running container. On hosts with ephemeral filesystems, data may be reset after a redeploy/restart. This is acceptable for a coursework deployment demonstration. A production system should use persistent storage or a managed database.

## Azure option

The same Dockerfile can be published to a container registry and run in Azure App Service for Linux as a custom container. Verify the app binds to the platform-provided port and configure persistent storage/database services for production use.

## Deployment verification checklist

- [ ] Docker image builds successfully
- [ ] Container starts without errors
- [ ] `/health` returns HTTP 200
- [ ] React page loads
- [ ] MVC page loads at `/mvc`
- [ ] GET `/api/tasks` works
- [ ] Create a task
- [ ] Toggle completion
- [ ] Delete a task
- [ ] Refresh page and confirm current container persistence
- [ ] Record deployment URL and Git commit in Week 8 notes

## Week 8 learning evidence

This practical demonstrates full-stack integration, Docker multi-stage builds, container port binding, environment-based configuration, cloud deployment workflow, health checking, and deployment documentation.
