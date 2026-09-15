using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskApi.Data;

namespace TaskApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<TaskDbContext>(options => options.UseSqlite("Data Source=tasks.db"));
			services.AddCors(options => options.AddPolicy("frontend", builder =>
				builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
			services.AddControllers();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TaskDbContext dbContext)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			dbContext.Database.EnsureCreated();
			app.UseRouting();
			app.UseCors("frontend");
			app.UseEndpoints(endpoints => endpoints.MapControllers());
		}
	}
}