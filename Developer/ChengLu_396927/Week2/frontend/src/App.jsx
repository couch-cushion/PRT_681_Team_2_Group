import { useEffect, useState } from 'react'

const API_URL = '/api/tasks'

function App() {
  const [tasks, setTasks] = useState([])
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [loading, setLoading] = useState(true)
  const [saving, setSaving] = useState(false)
  const [error, setError] = useState('')

  async function loadTasks() {
    setError('')
    try {
      const response = await fetch(API_URL)
      if (!response.ok) throw new Error('Unable to load tasks')
      setTasks(await response.json())
    } catch (requestError) {
      setError('API is unavailable. Start the ASP.NET Core server on port 5000.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadTasks()
  }, [])

  async function addTask(event) {
    event.preventDefault()
    if (!title.trim()) return

    setSaving(true)
    setError('')
    try {
      const response = await fetch(API_URL, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title, description, isCompleted: false })
      })
      if (!response.ok) throw new Error('Unable to add task')
      const task = await response.json()
      setTasks((currentTasks) => [task, ...currentTasks])
      setTitle('')
      setDescription('')
    } catch (requestError) {
      setError('The task could not be added.')
    } finally {
      setSaving(false)
    }
  }

  async function deleteTask(id) {
    setError('')
    try {
      const response = await fetch(`${API_URL}/${id}`, { method: 'DELETE' })
      if (!response.ok) throw new Error('Unable to delete task')
      setTasks((currentTasks) => currentTasks.filter((task) => task.id !== id))
    } catch (requestError) {
      setError('The task could not be deleted.')
    }
  }

  async function toggleTask(task) {
    setError('')
    try {
      const response = await fetch(`${API_URL}/${task.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          title: task.title,
          description: task.description,
          isCompleted: !task.isCompleted
        })
      })
      if (!response.ok) throw new Error('Unable to update task')
      setTasks((currentTasks) => currentTasks.map((currentTask) =>
        currentTask.id === task.id
          ? { ...currentTask, isCompleted: !currentTask.isCompleted }
          : currentTask
      ))
    } catch (requestError) {
      setError('The task could not be updated.')
    }
  }

  const completedCount = tasks.filter((task) => task.isCompleted).length

  return (
    <main className="page-shell">
      <section className="hero">
        <p className="eyebrow">LOCAL WORKSPACE / TASKS</p>
        <div className="hero-row">
          <div>
            <h1>Make room<br /><em>for progress.</em></h1>
            <p className="hero-copy">A small, focused task board backed by your local SQLite database.</p>
          </div>
          <div className="progress-mark" aria-label={`${completedCount} of ${tasks.length} tasks complete`}>
            <strong>{String(completedCount).padStart(2, '0')}</strong>
            <span>done<br />of {String(tasks.length).padStart(2, '0')}</span>
          </div>
        </div>
      </section>

      <section className="workspace">
        <form className="new-task" onSubmit={addTask}>
          <div className="section-label">NEW TASK</div>
          <label>
            <span>What needs doing?</span>
            <input value={title} onChange={(event) => setTitle(event.target.value)} placeholder="Name the next step" maxLength="200" />
          </label>
          <label>
            <span>Context <small>optional</small></span>
            <textarea value={description} onChange={(event) => setDescription(event.target.value)} placeholder="Add a little detail" maxLength="1000" rows="3" />
          </label>
          <button className="primary-button" type="submit" disabled={saving || !title.trim()}>
            {saving ? 'Adding...' : 'Add task'} <span>↗</span>
          </button>
        </form>

        <div className="task-list">
          <div className="list-heading">
            <div>
              <div className="section-label">YOUR LIST</div>
              <h2>{tasks.length ? 'Keep moving.' : 'Nothing here yet.'}</h2>
            </div>
            <span className="task-count">{tasks.length} {tasks.length === 1 ? 'task' : 'tasks'}</span>
          </div>

          {error && <div className="error-message">{error}</div>}
          {loading ? <div className="empty-state">Loading your tasks...</div> : tasks.length === 0 ? (
            <div className="empty-state">Add your first task to get started.</div>
          ) : (
            <div className="tasks">
              {tasks.map((task) => (
                <article className={`task-item ${task.isCompleted ? 'completed' : ''}`} key={task.id}>
                  <button className="check-button" onClick={() => toggleTask(task)} aria-label={`Mark ${task.title} complete`}>
                    {task.isCompleted ? '✓' : ''}
                  </button>
                  <div className="task-content">
                    <h3>{task.title}</h3>
                    {task.description && <p>{task.description}</p>}
                  </div>
                  <button className="delete-button" onClick={() => deleteTask(task.id)} aria-label={`Delete ${task.title}`}>×</button>
                </article>
              ))}
            </div>
          )}
        </div>
      </section>
    </main>
  )
}

export default App