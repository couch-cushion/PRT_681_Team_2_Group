using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskApi.Data;
using TaskApi.Models;

namespace TaskApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TasksController : ControllerBase
	{
		private readonly TaskDbContext context;

		public TasksController(TaskDbContext context)
		{
			this.context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
		{
			return await context.Tasks.OrderByDescending(task => task.CreatedAt).ToListAsync();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<TaskItem>> GetTask(int id)
		{
			TaskItem task = await context.Tasks.FindAsync(id);
			if (task == null)
			{
				return NotFound();
			}

			return task;
		}

		[HttpPost]
		public async Task<ActionResult<TaskItem>> CreateTask(TaskItem task)
		{
			if (string.IsNullOrWhiteSpace(task.Title))
			{
				return BadRequest("Title is required.");
			}

			task.Id = 0;
			task.Title = task.Title.Trim();
			task.CreatedAt = DateTime.UtcNow;
			context.Tasks.Add(task);
			await context.SaveChangesAsync();

			return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateTask(int id, TaskItem update)
		{
			TaskItem task = await context.Tasks.FindAsync(id);
			if (task == null)
			{
				return NotFound();
			}
			if (string.IsNullOrWhiteSpace(update.Title))
			{
				return BadRequest("Title is required.");
			}

			task.Title = update.Title.Trim();
			task.Description = update.Description?.Trim();
			task.IsCompleted = update.IsCompleted;
			await context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteTask(int id)
		{
			TaskItem task = await context.Tasks.FindAsync(id);
			if (task == null)
			{
				return NotFound();
			}

			context.Tasks.Remove(task);
			await context.SaveChangesAsync();
			return NoContent();
		}
	}
}