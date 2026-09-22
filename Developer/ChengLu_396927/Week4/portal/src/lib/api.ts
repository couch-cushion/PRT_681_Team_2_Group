import type { TaskInput, TaskItem } from "./types";

// Server-side code (Node runtime) uses API_URL; the browser uses NEXT_PUBLIC_API_URL.
// Both default to the ASP.NET Core API from Week 3, run locally on port 5000.
function resolveBaseUrl() {
  if (typeof window === "undefined") {
    return process.env.API_URL ?? "http://localhost:5000";
  }
  return process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000";
}

async function parseErrorMessage(response: Response) {
  const text = await response.text();
  return text || `Request failed with status ${response.status}`;
}

export async function fetchTasks(): Promise<TaskItem[]> {
  const response = await fetch(`${resolveBaseUrl()}/api/tasks`, {
    // Always hit the API fresh so the server-rendered list reflects the latest data.
    cache: "no-store",
  });
  if (!response.ok) {
    throw new Error(await parseErrorMessage(response));
  }
  return response.json();
}

export async function createTask(input: TaskInput): Promise<TaskItem> {
  const response = await fetch(`${resolveBaseUrl()}/api/tasks`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(input),
  });
  if (!response.ok) {
    throw new Error(await parseErrorMessage(response));
  }
  return response.json();
}

export async function updateTask(id: number, input: TaskInput): Promise<void> {
  const response = await fetch(`${resolveBaseUrl()}/api/tasks/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(input),
  });
  if (!response.ok) {
    throw new Error(await parseErrorMessage(response));
  }
}

export async function deleteTask(id: number): Promise<void> {
  const response = await fetch(`${resolveBaseUrl()}/api/tasks/${id}`, {
    method: "DELETE",
  });
  if (!response.ok) {
    throw new Error(await parseErrorMessage(response));
  }
}
