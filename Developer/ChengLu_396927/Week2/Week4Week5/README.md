# Northstar Order Operations

An English-language learning project connecting a Next.js App Router portal to an ASP.NET Core 8 API, a durable Temporal email workflow, structured Seq logging, and optional Exceptionless error reporting.

## Project layout

- `src/app`: responsive portal with a Telerik KendoReact Grid and client-side order form validation.
- `backend/Week4Week5.Api`: order CRUD API, Problem Details exception handling, health endpoint, Serilog/Seq, and Temporal workflow kickoff.
- `backend/Week4Week5.Worker`: Temporal worker and MailKit SMTP activity.
- `docker-compose.yml`: Seq, Temporal, Mailpit, API, worker, and portal services.

The API currently uses an in-memory store with sample records. Data resets when the API restarts; use a database and transactional outbox for production.

## Run locally without Docker

Start the API in one terminal:

```sh
dotnet run --project backend/Week4Week5.Api --urls http://localhost:5206
```

Start the portal in another terminal:

```sh
npm install
npm run dev
```

Open `http://localhost:3000`. Without Temporal, order CRUD still works and the portal reports that the confirmation workflow could not start. To run the complete workflow locally, provide Temporal at `localhost:7233`, Mailpit at `localhost:1025`, and Seq at `localhost:5341`, then start the worker:

```sh
dotnet run --project backend/Week4Week5.Worker
```

## Run the complete stack with Docker

Install Docker Desktop, then run:

```sh
docker compose up --build
```

- Portal: `http://localhost:3000`
- API health: `http://localhost:5206/health`
- Seq: `http://localhost:5341`
- Temporal Web UI: `http://localhost:8233`
- Mailpit inbox: `http://localhost:8025`

Create an order in the portal. The API starts a Temporal workflow, the worker sends the confirmation email to Mailpit, and API/worker structured logs are sent to Seq. Temporal retries failed activities up to five attempts.

To enable Exceptionless, set `EXCEPTIONLESS_API_KEY` in the shell before starting Compose. Without the key, the API still returns Problem Details for unhandled exceptions and logs them through Serilog.

Stop the stack with `docker compose down`. Persistent Seq data is stored in the `seq-data` volume.

## Checks

```sh
npm run lint
npm run build
dotnet build Week4Week5.sln
```

Telerik KendoReact is a commercial component suite. Telerik requires a valid license key for both trial and production use; without one, the Grid displays Telerik's license notice. Obtain and activate a key through Telerik's official licensing instructions.
