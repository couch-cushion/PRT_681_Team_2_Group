import { useEffect, useState } from 'react'

const API_URL = '/api/tasks'

export default function App() {
  const [tasks, setTasks] = useState([])
  const [title, setTitle] = useState('')
  const [loading, setLoading] = useState(true)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')

  async function loadTasks() {
    setLoading(true)
    setError('')

    try {
      const response = await fetch(API_URL)
      if (!response.ok) throw new Error('Could not load tasks.')
      setTasks(await response.json())
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadTasks()
  }, [])

  async function addTask(event) {
    event.preventDefault()
    const cleanTitle = title.trim()
    if (!cleanTitle || busy) return

    setBusy(true)
    setError('')

    try {
      const response = await fetch(API_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          title: cleanTitle,
          isCompleted: false,
        }),
      })

      if (!response.ok) throw new Error('Could not add task.')

      const created = await response.json()
      setTasks((current) => [...current, created])
      setTitle('')
    } catch (err) {
      setError(err.message)
    } finally {
      setBusy(false)
    }
  }

  async function toggleTask(task) {
    if (busy) return
    setBusy(true)
    setError('')

    try {
      const response = await fetch(`${API_URL}/${task.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          title: task.title,
          isCompleted: !task.isCompleted,
        }),
      })

      if (!response.ok) throw new Error('Could not update task.')
      const updated = await response.json()
      setTasks((current) =>
        current.map((item) => item.id === updated.id ? updated : item)
      )
    } catch (err) {
      setError(err.message)
    } finally {
      setBusy(false)
    }
  }

  async function deleteTask(id) {
    if (busy) return
    setBusy(true)
    setError('')

    try {
      const response = await fetch(`${API_URL}/${id}`, {
        method: 'DELETE',
      })

      if (!response.ok) throw new Error('Could not delete task.')
      setTasks((current) => current.filter((task) => task.id !== id))
    } catch (err) {
      setError(err.message)
    } finally {
      setBusy(false)
    }
  }

  return (
    <main className="app-shell">
      <section className="card">
        <div className="heading-row">
          <div>
            <p className="eyebrow">PRT681 / PRT585 · Developer Week 3</p>
            <h1>Tasks Full-Stack Deployment</h1>
            <p className="subtitle">
              React frontend · ASP.NET Core API · EF Core · SQLite · Docker
            </p>
          </div>
          <div style={{ display: 'grid', gap: '8px', justifyItems: 'end' }}>
            <span className="status">Week 8</span>
            <a href="/mvc" style={{ color: '#214c8c', fontWeight: 700 }}>MVC learning page</a>
          </div>
        </div>

        <form onSubmit={addTask} className="task-form">
          <label htmlFor="task-title">Add a task</label>
          <div className="form-row">
            <input
              id="task-title"
              value={title}
              onChange={(event) => setTitle(event.target.value)}
              placeholder="e.g. Verify Docker deployment"
              maxLength={200}
              disabled={busy}
            />
            <button disabled={busy || !title.trim()}>Add</button>
          </div>
        </form>

        {error && <p className="error" role="alert">{error}</p>}

        <div className="list-heading">
          <h2>Tasks</h2>
          <button className="secondary" onClick={loadTasks} disabled={busy || loading}>
            Refresh
          </button>
        </div>

        {loading ? (
          <p>Loading tasks...</p>
        ) : tasks.length === 0 ? (
          <p className="empty">No tasks yet. Add the first deployment task.</p>
        ) : (
          <ul className="task-list">
            {tasks.map((task) => (
              <li key={task.id}>
                <button
                  className="check-button"
                  type="button"
                  aria-label={task.isCompleted ? 'Mark incomplete' : 'Mark complete'}
                  onClick={() => toggleTask(task)}
                  disabled={busy}
                >
                  {task.isCompleted ? '✓' : '○'}
                </button>
                <div className="task-text">
                  <strong className={task.isCompleted ? 'done' : ''}>{task.title}</strong>
                  <small>{task.isCompleted ? 'Completed' : 'Not completed'}</small>
                </div>
                <button
                  className="danger"
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
      </section>
    </main>
  )
}
