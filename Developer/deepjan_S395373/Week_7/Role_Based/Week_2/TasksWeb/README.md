# React + Vite

This template provides a minimal setup to get React working in Vite with HMR and some Oxlint rules.

Currently, two official plugins are available:

- [@vitejs/plugin-react](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react) uses [Oxc](https://oxc.rs)
- [@vitejs/plugin-react-swc](https://github.com/vitejs/vite-plugin-react/blob/main/packages/plugin-react-swc) uses [SWC](https://swc.rs/)

## React Compiler

The React Compiler is not enabled on this template because of its impact on dev & build performances. To add it, see [this documentation](https://react.dev/learn/react-compiler/installation).

## Expanding the Oxlint configuration

If you are developing a production application, we recommend using TypeScript with type-aware lint rules enabled. Check out the [TS template](https://github.com/vitejs/vite/tree/main/packages/create-vite/template-react-ts) for information on how to integrate TypeScript and Oxlint's TypeScript related rules in your project.

## Coursework: Week 7 Tasks frontend

This React application connects to the Week 7 ASP.NET Core Tasks API.

### Features
- List tasks from the API.
- Add a task.
- Delete a task.
- Reload saved tasks after refreshing the page.

### Run locally
Install dependencies with `npm.cmd install`.
Start the frontend with `npm.cmd run dev`.
Open http://localhost:5173/.

Start the backend from the Week_2 folder with:
`dotnet run --project TasksApi -- --urls http://localhost:5050`

The frontend calls http://localhost:5050/api/tasks.
The API stores data using EF Core and SQLite.

### Verification
Manual test: add two tasks, delete one, and refresh to confirm persistence.
Production build: `npm.cmd run build`.
Production dependency audit: `npm.cmd audit --omit=dev`.
