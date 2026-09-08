using Microsoft.Data.Sqlite;
using TasksConsole.Models;

string databasePath = Path.Combine(AppContext.BaseDirectory, "tasks.db");

using var connection = new SqliteConnection($"Data Source={databasePath}");
connection.Open();

using (var command = connection.CreateCommand())
{
    command.CommandText = """
        CREATE TABLE IF NOT EXISTS Tasks
        (
            Id INTEGER PRIMARY KEY,
            Title TEXT NOT NULL,
            IsCompleted INTEGER NOT NULL DEFAULT 0
                CHECK (IsCompleted IN (0, 1))
        );
        """;

    command.ExecuteNonQuery();
}

Console.WriteLine("Tasks Console App");
Console.WriteLine($"Database: {databasePath}");

while (true)
{
    Console.WriteLine("\n1. Add task");
    Console.WriteLine("2. View tasks");
    Console.WriteLine("3. Update task");
    Console.WriteLine("4. Delete task");
    Console.WriteLine("5. Exit");
    Console.Write("Choose: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            AddTask();
            break;
        case "2":
            ViewTasks();
            break;
        case "3":
            UpdateTask();
            break;
        case "4":
            DeleteTask();
            break;
        case "5":
            return;
        default:
            Console.WriteLine("Invalid choice.");
            break;
    }
}

void AddTask()
{
    Console.Write("Title: ");
    string title = (Console.ReadLine() ?? "").Trim();

    if (string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("Title cannot be empty.");
        return;
    }

    using var command = connection.CreateCommand();
    command.CommandText = """
        INSERT INTO Tasks (Title, IsCompleted)
        VALUES ($title, 0);
        """;
    command.Parameters.AddWithValue("$title", title);

    command.ExecuteNonQuery();
    Console.WriteLine("Task added.");
}

void ViewTasks()
{
    using var command = connection.CreateCommand();
    command.CommandText = """
        SELECT Id, Title, IsCompleted
        FROM Tasks
        ORDER BY Id;
        """;

    using var reader = command.ExecuteReader();

    Console.WriteLine("\n--- Tasks ---");
    bool found = false;

    while (reader.Read())
    {
        found = true;

        TaskItem task = new TaskItem
        {
            Id = reader.GetInt32(0),
            Title = reader.GetString(1),
            IsCompleted = reader.GetInt64(2) == 1
        };

        Console.WriteLine(
            $"{task.Id}. {task.Title} | Completed: {task.IsCompleted}"
        );
    }

    if (!found)
        Console.WriteLine("No tasks found.");
}

void UpdateTask()
{
    int id = ReadId();
    if (id == 0) return;

    Console.Write("New title: ");
    string title = (Console.ReadLine() ?? "").Trim();

    if (string.IsNullOrWhiteSpace(title))
    {
        Console.WriteLine("Title cannot be empty.");
        return;
    }

    Console.Write("Completed? (y/n): ");
    string completedInput = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

    if (completedInput != "y" && completedInput != "n")
    {
        Console.WriteLine("Enter y or n.");
        return;
    }

    bool isCompleted = completedInput == "y";

    using var command = connection.CreateCommand();
    command.CommandText = """
        UPDATE Tasks
        SET Title = $title,
            IsCompleted = $completed
        WHERE Id = $id;
        """;

    command.Parameters.AddWithValue("$title", title);
    command.Parameters.AddWithValue("$completed", isCompleted ? 1 : 0);
    command.Parameters.AddWithValue("$id", id);

    int rows = command.ExecuteNonQuery();

    Console.WriteLine(rows == 0
        ? "Task not found."
        : "Task updated.");
}

void DeleteTask()
{
    int id = ReadId();
    if (id == 0) return;

    Console.Write("Delete this task? (y/n): ");
    string confirm = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

    if (confirm != "y")
    {
        Console.WriteLine("Deletion cancelled.");
        return;
    }

    using var command = connection.CreateCommand();
    command.CommandText = """
        DELETE FROM Tasks
        WHERE Id = $id;
        """;

    command.Parameters.AddWithValue("$id", id);

    int rows = command.ExecuteNonQuery();

    Console.WriteLine(rows == 0
        ? "Task not found."
        : "Task deleted.");
}

int ReadId()
{
    Console.Write("Task ID: ");

    if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
    {
        Console.WriteLine("Enter a valid positive ID.");
        return 0;
    }

    return id;
}