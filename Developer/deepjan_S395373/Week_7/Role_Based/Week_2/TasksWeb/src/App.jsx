import { useEffect, useState } from "react";
import "./App.css";

const API_URL = "http://localhost:5050/api/tasks";

export default function App() {
  const [tasks, setTasks] = useState([]);
  const [title, setTitle] = useState("");
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    let cancelled = false;

    async function loadTasks() {
      try {
        const response = await fetch(API_URL);

        if (!response.ok) {
          throw new Error("Could not load tasks.");
        }

        const data = await response.json();

        if (!cancelled) {
          setTasks(data);
        }
      } catch (err) {
        if (!cancelled) {
          setError(err.message);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    }

    loadTasks();

    return () => {
      cancelled = true;
    };
  }, []);

  async function addTask(event) {
    event.preventDefault();

    const cleanTitle = title.trim();
    if (!cleanTitle || busy) return;

    setBusy(true);
    setError("");

    try {
      const response = await fetch(API_URL, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          title: cleanTitle,
          isCompleted: false,
        }),
      });

      if (!response.ok) {
        throw new Error("Could not add task.");
      }

      const created = await response.json();

      setTasks((current) => [...current, created]);
      setTitle("");
    } catch (err) {
      setError(err.message);
    } finally {
      setBusy(false);
    }
  }

  async function deleteTask(id) {
    if (busy) return;

    setBusy(true);
    setError("");

    try {
      const response = await fetch(`${API_URL}/${id}`, {
        method: "DELETE",
      });

      if (!response.ok) {
        throw new Error("Could not delete task.");
      }

      setTasks((current) =>
        current.filter((task) => task.id !== id)
      );
    } catch (err) {
      setError(err.message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <main className="app">
      <h1>Tasks</h1>
      <p>Week 7 — React + ASP.NET Core</p>

      <form onSubmit={addTask}>
        <label htmlFor="task-title">New task</label>

        <div className="form-row">
          <input
            id="task-title"
            value={title}
            onChange={(event) => setTitle(event.target.value)}
            placeholder="Enter a task"
            maxLength={200}
            disabled={busy || loading}
          />

          <button disabled={busy || loading || !title.trim()}>
            Add
          </button>
        </div>
      </form>

      {error && <p role="alert" className="error">{error}</p>}

      {loading ? (
        <p>Loading tasks...</p>
      ) : tasks.length === 0 ? (
        <p>No tasks yet. Add your first task.</p>
      ) : (
        <ul>
          {tasks.map((task) => (
            <li key={task.id}>
              <span>
                <strong>{task.title}</strong>
                <small>
                  {task.isCompleted ? "Completed" : "Not completed"}
                </small>
              </span>

              <button
                type="button"
                onClick={() => deleteTask(task.id)}
                disabled={busy}
              >
                Delete
              </button>
            </li>
          ))}
        </ul>
      )}
    </main>
  );
}