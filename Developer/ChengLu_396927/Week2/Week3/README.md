# Tasks CRUD Full-Stack App

This project is a small full-stack app built with:

- ASP.NET Core Web API
- Entity Framework Core
- SQLite database
- React + Vite frontend

It includes a beginner-friendly guide in:

- [API_FLOW_BEGINNER_CN.md](API_FLOW_BEGINNER_CN.md)
- [API_FLOW_BEGINNER_EN.md](API_FLOW_BEGINNER_EN.md)

## Architecture

- Frontend: React + Vite
- Backend: ASP.NET Core Web API
- Database: SQLite (`tasks.db`)
- Deployment: Dockerized API + frontend static site

## Run locally

### 1) Start the API

```bash
dotnet run --project hello.csproj --urls http://localhost:5000
```

### 2) Start the frontend

Open a second terminal:

```bash
cd frontend
npm install
npm run dev
```

Then open:

- Frontend: `http://localhost:5173`
- API: `http://localhost:5000`

## Run with Docker

### Build the API image

```bash
docker build -t task-api .
```

### Run the API container locally

```bash
docker run --rm -p 5001:5000 -v "$(pwd)/tasks.db:/app/tasks.db" task-api
```

### Run both services with Docker Compose

```bash
docker compose up --build
```

This starts:

- API: `http://localhost:5001`
- Frontend: `http://localhost:8080`

## Build the frontend for production

```bash
cd frontend
npm install
npm run build
```

The static files are generated in `frontend/dist`.

## Deploy to Render (recommended free-tier option)

1. Push this repository to GitHub.
2. Create a new Web Service on Render.
3. Connect the GitHub repo.
4. Use the included Dockerfile for the API service.
5. Set the service port to `5000`.
6. For the frontend, either:
   - deploy `frontend/dist` as a static site, or
   - serve the frontend through a separate Nginx container.
7. Set `VITE_API_URL` in the frontend build to the deployed API URL.

## Deploy to Azure App Service

1. Push this repository to GitHub.
2. Create a new Azure App Service.
3. Configure it to run the Docker image from this repo.
4. Expose port `5000`.
5. Keep SQLite in a persistent storage path, or move the database to Azure SQL / PostgreSQL for production.
6. Deploy the frontend as a separate static site if needed.

## Notes

- This project uses SQLite for simplicity, so it is best for demos and small projects.
- For production, use a managed database such as PostgreSQL or Azure SQL.
- For a beginner deployment, Render is the easiest option because it is simpler than App Service setup.
