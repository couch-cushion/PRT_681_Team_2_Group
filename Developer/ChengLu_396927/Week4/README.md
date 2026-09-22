# Week 4 — Advanced Enterprise UI, Next.js & Modern Dev Workflows

This folder builds directly on the Week 3 ASP.NET Core Task API
([../Week2/Week3](../Week2/Week3)). Instead of the React/Vite frontend, this week's
practice is a **Next.js management portal** styled with **Kendo UI for React**, wired
to the same backend.

## What's here

```
Week4/
  portal/            Next.js (App Router) management portal
    src/app/page.tsx        landing page (SSG)
    src/app/about/page.tsx  notes on SSR/SSG, component libraries, AI-assisted coding (SSG)
    src/app/tasks/page.tsx  Task Grid page (SSR — fetches from the API on every request)
    src/app/tasks/TaskManager.tsx  client component: Kendo Grid + Form + Dialog CRUD UI
    src/lib/api.ts          fetch helpers calling the Week 3 ASP.NET Core API
    src/lib/types.ts        TaskItem / TaskInput types matching the API's TaskItem model
```

## Topics covered

- **Next.js App Router, SSR & SSG**: `/tasks` is a Server Component with
  `export const dynamic = "force-dynamic"`, so it calls the API and renders the grid's
  initial HTML on every request (SSR), then hydrates in the browser for interactivity.
  `/` and `/about` fetch no data, so Next.js statically generates them once at build
  time (SSG) — confirmed by `npm run build` output (`○` = static, `ƒ` = SSR).
- **Enterprise component suite**: Kendo UI for React (`@progress/kendo-react-grid`,
  `kendo-react-form`, `kendo-react-dialogs`, `kendo-react-inputs`) provides the data
  grid, validated form, and modal dialogs used for Create/Read/Update/Delete. DevExpress
  (DevExtreme `DataGrid`, `dx-react-form`) is a drop-in commercial alternative with the
  same grid/scheduler/pivot-table style components.
- **Client-side validation against the API contract**: the Kendo `Form` validates
  `title` (required, ≤ 200 chars) and `description` (≤ 1000 chars) client-side, matching
  the constraints enforced by [`TasksController`](../Week2/Week3/Controllers/TasksController.cs)
  and [`TaskItem`](../Week2/Week3/Models/TaskItem.cs) on the server.
- **AI-assisted ("vibe coding") workflow**: the Grid wiring, Form field renderers, and
  API client were scaffolded by describing the desired CRUD screen and validation rules
  to GitHub Copilot, then reviewed/adjusted by hand — see `/about` in the running app for
  more detail.

## Running locally

1. Start the Week 3 API (SQLite-backed):

   ```bash
   cd ../Week2/Week3
   dotnet run --project hello.csproj --urls http://localhost:5000
   ```

2. Start the Next.js portal:

   ```bash
   cd portal
   npm install
   npm run dev
   ```

3. Open `http://localhost:3000`. The portal calls the API using `API_URL` /
   `NEXT_PUBLIC_API_URL` from `portal/.env.local` (defaults to `http://localhost:5000`).

## Build

```bash
cd portal
npm run build
```

The build output lists each route's rendering strategy (static vs. server-rendered),
which is the easiest way to see the SSR/SSG split in practice.

## Note on Kendo UI licensing

The Kendo React packages run in trial mode without a commercial license key (a console
warning is expected, functionality is unaffected). For a production deployment, add a
Telerik/Kendo license via `@progress/kendo-licensing` as documented by Progress.
