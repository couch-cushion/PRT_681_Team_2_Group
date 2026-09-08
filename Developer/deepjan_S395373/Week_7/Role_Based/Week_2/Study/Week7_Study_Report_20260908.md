# Week 7 — Web API, EF Core and React
## Implementation and Study Report

**Student:** Deepjan Thapaliya (S395373)  
**Unit:** PRT681 / PRT585 — Software Engineering: Practice  
**Role:** Developer, Team 2  
**Date:** 8 September 2026  
**Mapping:** University Week 7 / Developer Role-Based Week 2

## 1. Purpose and scope

The Developer Week 2 practical requires the Week 1 console CRUD application to be developed into an ASP.NET Core Web API using Entity Framework, with a minimal React or Angular frontend that lists, adds and deletes tasks through the API. React was selected. The associated study topics are RESTful APIs, Entity Framework Core and React.

This report documents the implementation, explains the main concepts and records the observed tests. The application is a local coursework prototype. Completing working code does not establish completion of the named training courses or independent mastery of every concept. The Week 6 console application and its database remain separate from the Week 7 application.

## 2. Architecture

```text
Browser / React
       ↓ HTTP request + JSON
ASP.NET Core Web API
       ↓ C# data-access operations
Entity Framework Core
       ↓ SQLite provider
SQLite database
       ↑
API response + JSON → React updates the screen
```

React handles the interface. ASP.NET Core receives requests, validates input and controls operations. EF Core maps C# objects to relational data and performs database work through a provider. SQLite stores records persistently.

In Week 6, the console application used Microsoft.Data.Sqlite and explicitly written SQL. In Week 7, EF Core handles much of that repetitive data-access work. The frontend does not connect directly to SQLite.

| Technology | Responsibility |
|---|---|
| C# / .NET | Backend application and logic |
| ASP.NET Core | HTTP server and endpoint handling |
| REST-style API | Resource URLs and HTTP operations |
| Entity Framework Core | Object-relational mapping and data access |
| SQLite | Persistent local database |
| React | Browser interface and state |
| Vite | Frontend development server and build |
| Git / GitHub | Version control and team repository |

## 3. API and HTTP fundamentals

An API is a defined interface through which software components communicate. A Web API uses HTTP requests and responses. It is not the database: it is the backend boundary that controls access to the database.

The local API address is `http://localhost:5050/api/tasks`. `localhost` refers to the developer's computer, `5050` is the API port, and `/api/tasks` identifies the task resource.

REST is an architectural style. This practical uses resource URLs and HTTP methods to describe operations.

| Method | Endpoint | Purpose | Success |
|---|---|---|---|
| GET | `/api/tasks` | Retrieve all tasks | 200 OK |
| GET | `/api/tasks/{id}` | Retrieve one task | 200 OK |
| POST | `/api/tasks` | Create a task | 201 Created |
| PUT | `/api/tasks/{id}` | Update a task | 200 OK |
| DELETE | `/api/tasks/{id}` | Delete a task | 204 No Content |

A missing individual task returns 404 Not Found. An invalid task title returns 400 Bad Request. The frontend implements List, Add and Delete; the API additionally supports retrieving one task and updating a task.

### JSON

JSON is a text format for exchanging structured data. A returned task looks like:

```json
{
  "id": 1,
  "title": "Buy Milk",
  "isCompleted": false
}
```

ASP.NET Core serializes the C# object into JSON. The browser parses the JSON into JavaScript data. The client does not receive a C# object directly. A creation request supplies the title and completion value; the database generates the ID.

## 4. Backend project structure

```text
TasksApi/
├── Program.cs
├── Models/
│   └── TaskItem.cs
├── Data/
│   └── TasksDbContext.cs
├── TasksApi.csproj
├── appsettings.json
├── appsettings.Development.json
├── Properties/
│   └── launchSettings.json
├── README.md
└── .gitignore
```

Program.cs configures services, the database provider, CORS and endpoints. TaskItem.cs defines the entity. TasksDbContext.cs defines the EF Core context. The project uses ASP.NET Core Minimal APIs, so a separate MVC controller is not required for this small implementation.

## 5. Entity Framework Core

Entity Framework Core is an Object-Relational Mapper (ORM). It lets .NET applications query and save relational data using C# objects and queries. It reduces repetitive SQL and mapping code, but it does not replace SQL knowledge or the database engine.

### Direct SQL versus EF Core

Week 6:

```csharp
command.CommandText = "INSERT INTO Tasks (Title, IsCompleted) VALUES ($title, 0);";
command.Parameters.AddWithValue("$title", title);
command.ExecuteNonQuery();
```

Week 7:

```csharp
db.Tasks.Add(task);
await db.SaveChangesAsync();
```

Both can result in an INSERT. The difference is which layer translates the C# operation into database commands.

### Entity

```csharp
namespace TasksApi.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}
```

A TaskItem is a C# object in memory. A SQLite row is persistent database data. EF Core maps between them; they are not literally the same object.

### DbContext and DbSet

```csharp
using Microsoft.EntityFrameworkCore;
using TasksApi.Models;

namespace TasksApi.Data;

public class TasksDbContext : DbContext
{
    public TasksDbContext(DbContextOptions<TasksDbContext> options)
        : base(options) {}

    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}
```

DbContext manages database queries, change tracking and saving. DbSet<TaskItem> exposes the task entity set for querying and modification. It is not simply an in-memory list of every row.

### Change tracking

`db.Tasks.Add(task)` marks the entity as Added. `SaveChangesAsync()` saves pending changes through the database provider.

```text
Create C# object
    ↓
Add to DbContext
    ↓
EF Core tracks it as Added
    ↓
SaveChangesAsync()
    ↓
Database INSERT
    ↓
Persisted row
```

Creating an object or calling Add alone does not guarantee that a row has been saved. The database operation can still fail.

### Provider and dependency injection

The application registers the SQLite provider:

```csharp
builder.Services.AddDbContext<TasksDbContext>(options =>
    options.UseSqlite(connectionString));
```

EF Core supplies the ORM layer; the provider supplies database-specific behaviour. ASP.NET Core's dependency injection container can then supply a context to an endpoint:

```csharp
app.MapGet("/api/tasks", async (TasksDbContext db) =>
{
    return await db.Tasks
        .AsNoTracking()
        .OrderBy(t => t.Id)
        .ToListAsync();
});
```

AsNoTracking is useful for this read-only query. OrderBy explicitly requests ordering; relational databases do not guarantee row order without an ordering instruction. The context is normally scoped to the HTTP request when registered through AddDbContext.

## 6. Complete request lifecycle

### Creating a task

1. The user enters a title in React and clicks Add.
2. React trims the title and sends an HTTP POST request containing JSON.
3. ASP.NET Core matches the request to the POST endpoint.
4. The framework deserializes the JSON into the request type.
5. The backend validates the title.
6. A new TaskItem is created.
7. EF Core tracks it and SaveChangesAsync writes it to SQLite.
8. The API returns 201 Created and the new task as JSON.
9. React parses the response and adds the returned task to its displayed state.

The frontend request follows this pattern:

```jsx
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
```

The frontend does not execute SQL directly. The API remains responsible for database access and backend validation.

### Reading tasks

```text
React loads → GET /api/tasks → ASP.NET Core endpoint
→ EF Core LINQ query → SQLite SELECT → Task entities
→ JSON response → React displays the list
```

### Updating and deleting

The API checks whether the requested entity exists. For an update, it changes the title and completion status and saves the changes. For deletion, it removes the entity and saves the change. A valid positive integer is not necessarily an existing ID: task 999 may not exist even though 999 is valid input.

The request record is similar to:

```csharp
public record TaskRequest(string? Title, bool IsCompleted);
```

The backend trims the title and requires a nonempty value within its 200-character limit. Frontend validation improves usability, but backend validation is necessary because clients can call the API without using React.

## 7. Async and await

The API uses asynchronous database methods such as ToListAsync and SaveChangesAsync. Database operations involve I/O. Asynchronous methods allow the request thread to be released while waiting for I/O, helping the server use resources efficiently under concurrent requests.

Await does not mean the database operation has already completed. It means the method waits asynchronously for the result before continuing with dependent code.

## 8. React frontend

The frontend is stored in `Week_7/Role_Based/Week_2/TasksWeb`. Important files include src/App.jsx, src/App.css, src/index.css, src/main.jsx, package.json, package-lock.json and vite.config.js.

React uses component state to determine what appears on the screen. The implementation stores the task list, current title, loading status, busy status and error messages.

```jsx
const [tasks, setTasks] = useState([]);
const [title, setTitle] = useState("");
```

useEffect loads tasks when the component starts. The Add form sends POST JSON and appends the returned task after success. The Delete button sends DELETE for the selected ID and removes the task from local state after success.

React state alone is temporary browser memory. Persistence comes from SQLite:

```text
Add task → API saves to SQLite
                  ↓
             Refresh browser
                  ↓
          React sends GET
                  ↓
          SQLite returns saved task
```

The frontend also displays loading, empty-list and error states.

## 9. CORS and local development

React runs at `http://localhost:5173`, while the API runs at `http://localhost:5050`. Different ports mean different origins. Browsers apply cross-origin rules, so the API configures a CORS policy allowing the React development origins, including localhost:5173 and 127.0.0.1:5173.

CORS is a browser access-control mechanism. It is not authentication and does not make an API private. Production origins and security policies must be configured appropriately.

## 10. Database initialization and limitations

The coursework API uses EnsureCreated during startup to create its database schema if needed. This is convenient for a small prototype but is not the same as migration-based schema management. EF Core migrations support evolving a database schema as the model changes.

The Week 7 API database is separate from the Week 6 console database. Local tasks.db files and generated build output are excluded from Git. For a larger application, endpoint, service and data-access responsibilities can be separated when that provides a clear benefit.

## 11. Local execution

From the Week_2 folder, start the backend:

```powershell
dotnet run --project TasksApi -- --urls http://localhost:5050
```

The backend must remain running while React calls it. The observed output included:

```text
Now listening on: http://localhost:5050
Application started.
```

From TasksWeb, start the frontend:

```powershell
npm.cmd install
npm.cmd run dev
```

Open http://localhost:5173/.

Node.js was installed but initially absent from the PowerShell PATH. The temporary terminal fix was:

```powershell
$env:Path = "C:\Program Files\nodejs;" + $env:Path
```

The observed versions were Node.js v24.20.0 and npm 11.19.0. A new terminal may need its PATH refreshed if Windows has not yet propagated the installation.

## 12. Testing and evidence

### API manual verification

| Test | Observed result |
|---|---|
| POST | Created “Finish Week 7”. |
| GET collection | Returned the created task. |
| PUT | Updated it to “Finish Week 7 API” and completed=true. |
| DELETE | Removed the task. |
| GET after deletion | Returned an empty collection. |

The API build succeeded after resolving a file lock caused by attempting to rebuild while TasksApi.exe was still running.

### Frontend manual verification

The browser displayed the Tasks interface. Two tasks were added: “Buy Milk” and “do push up”. After deleting one and refreshing, “Buy Milk” remained. This verifies the required list, add, delete and refresh-persistence behaviour for the tested local scenario.

### Build and dependency checks

The .NET API build succeeded. The React production build succeeded with Vite, and the production-dependency audit reported zero vulnerabilities at the time of testing.

The SQLite native dependency was previously updated to SQLitePCLRaw.bundle_e_sqlite3 version 2.1.13 after a vulnerability warning. The .NET dependency audit then reported no vulnerable packages from the configured sources.

These are manual and build-level checks. Automated unit, integration, end-to-end, performance and security tests have not been demonstrated by the available evidence.

## 13. Git evidence

The work was published to the team repository on the deepjan/career-preparation branch.

| Commit | Work |
|---|---|
| c0c0daf | Complete Week 6 Tasks console CRUD practical |
| bf2a430 | Update Week 6 SQLite dependency |
| 14c3d93 | Complete Week 7 Tasks API with EF Core |
| e6001df | Complete Week 7 React Tasks frontend |

Repository: https://github.com/couch-cushion/PRT_681_Team_2_Group

The observed pushes succeeded. This establishes publication to the named branch, not a merge into main.

## 14. Challenges and lessons learned

**Incorrect model filename:** The model was initially named TaksItem.cs. The application built because the class was correct, but Git staging failed when the expected path was TaskItem.cs. The file was renamed and rebuilt.

**Running executable locked:** Windows prevented replacement of TasksApi.exe while the API was running. Stopping the server with Ctrl+C released the lock.

**PowerShell variable scope:** Commands using $api or $lab failed when variables were undefined in the current terminal. Explicit paths or running from the project directory resolved this.

**Node.js PATH:** Node and npm were installed but not initially found. Adding the installation directory to the terminal PATH resolved the issue.

**Separate processes:** React and the API must both be running for browser-to-database communication.

These experiences show the importance of distinguishing source-code errors, build errors, environment configuration and runtime communication problems.

## 15. Learning reflection and next steps

The practical was completed to meet the coursework deadline, but working code is not the same as independent understanding. The main learning gap identified is the connection between C# and relational databases, followed by the HTTP/API lifecycle.

The planned foundation sequence is:

1. Database providers, connection strings, opening and disposing connections.
2. SQL commands, parameters, ExecuteNonQuery, ExecuteScalar and ExecuteReader.
3. CRUD, validation, missing records, exceptions and transactions.
4. EF Core entities, context, DbSet, LINQ translation, change tracking, saving and migrations.
5. HTTP methods, URLs, headers, JSON and status codes.
6. ASP.NET Core routing, dependency injection, validation and testing.
7. React asynchronous requests, state, loading/error handling and persistence.
8. A cumulative independent application in a different business domain.

The goal is transferable understanding: being able to build database access and APIs for another system, not memorizing the Tasks implementation.

## 16. Presentation questions

**What is the difference between SQLite and EF Core?**  
SQLite is the database engine. EF Core is the .NET ORM that helps application code query and save relational data.

**Why use an API instead of connecting React directly to SQLite?**  
The API provides a controlled backend boundary for validation, business rules and database access. Different clients can use the same contract.

**What happens when Add is clicked?**  
React sends POST JSON, ASP.NET Core handles the request, the backend validates it, EF Core saves a new entity, and the API returns the created task for React to display.

**What does SaveChangesAsync do?**  
It saves pending tracked changes through the database provider. Calling Add alone does not guarantee persistence.

**What is dependency injection?**  
It supplies required services, such as TasksDbContext, without every endpoint manually constructing them.

**Why does a task remain after refresh?**  
SQLite stores it persistently, and React reloads it through the API.

**What is CORS?**  
A browser mechanism controlling cross-origin access to responses. It is not authentication.

**What would be improved next?**  
Automated testing, validation and error handling, migrations and clearer service separation, followed by deployment when the foundations are understood.

## 17. Short presentation summary

In Week 6, I built a C# console application using direct SQL and SQLite for CRUD operations. In Week 7, I developed the same idea into an ASP.NET Core Web API using Entity Framework Core and connected a React frontend. The frontend sends HTTP requests, the API validates and processes them, EF Core performs database operations, and SQLite stores the records. I manually verified the API CRUD operations and confirmed that React could add, delete and reload saved tasks. The API and frontend were built and pushed to the team repository branch. My next focus is understanding the database connection, EF Core and HTTP request lifecycle in depth so I can apply these concepts independently to other projects.

## 18. References and acknowledgement

Coursework requirement: CDU PRT681/PRT585 Group Weekly Activities, Full Stack .NET Developer, Role-Based Week 2.

Official learning references:

- ASP.NET Core Minimal APIs: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis
- Entity Framework Core: https://learn.microsoft.com/en-us/ef/core/
- EF Core DbContext: https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/
- EF Core change tracking: https://learn.microsoft.com/en-us/ef/core/change-tracking/
- EF Core migrations: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/
- React: https://react.dev/learn
- HTTP overview: https://developer.mozilla.org/en-US/docs/Web/HTTP/Overview

**AI assistance:** This report was prepared with ChatGPT assistance using the coursework requirements, implementation code and command/test outputs supplied in the conversation. The student ran and manually tested the applications and performed the Git commits. Verified results are distinguished from training or testing not demonstrated. The report should be reviewed against the actual repository before final submission.
