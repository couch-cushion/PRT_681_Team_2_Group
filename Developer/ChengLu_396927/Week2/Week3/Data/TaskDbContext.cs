using Microsoft.EntityFrameworkCore;
using TaskApi.Models;

namespace TaskApi.Data
{
	public class TaskDbContext : DbContext
	{
		public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
		{
		}

		public DbSet<TaskItem> Tasks { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TaskItem>().Property(task => task.Title).IsRequired().HasMaxLength(200);
			modelBuilder.Entity<TaskItem>().Property(task => task.Description).HasMaxLength(1000);
			modelBuilder.Entity<TaskItem>().Property(task => task.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
		}
	}
}