import TaskManager from "./TaskManager";
import { fetchTasks } from "@/lib/api";
import type { TaskItem } from "@/lib/types";

// Force a fresh server render on every request so the Grid always shows
// the latest data from the ASP.NET Core API (classic SSR, no static cache).
export const dynamic = "force-dynamic";

export default async function TasksPage() {
  let tasks: TaskItem[];
  let loadError = "";
  try {
    tasks = await fetchTasks();
  } catch {
    tasks = [];
    loadError =
      "Could not reach the ASP.NET Core API. Start it with 'dotnet run --project hello.csproj --urls http://localhost:5000' from Week2/Week3.";
  }

  return (
    <main className="page">
      <header className="page-header">
        <p className="eyebrow">WEEK 4 · MANAGEMENT PORTAL</p>
        <h1>Task Management Portal</h1>
        <p className="subtitle">
          Server-rendered on every request (SSR), hydrated in the browser, and backed by the
          Week 3 ASP.NET Core Web API.
        </p>
      </header>

      {loadError && <div className="error-banner">{loadError}</div>}

      <TaskManager initialTasks={tasks} />
    </main>
  );
}
