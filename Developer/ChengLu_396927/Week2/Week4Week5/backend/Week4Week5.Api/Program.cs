using System.ComponentModel.DataAnnotations;
using Exceptionless;
using Serilog;
using Temporalio.Client;
using Week4Week5.Api;

var builder = WebApplication.CreateBuilder(args);
var exceptionlessKey = builder.Configuration["Exceptionless:ApiKey"];
if (!string.IsNullOrWhiteSpace(exceptionlessKey))
{
    builder.AddExceptionless(options => options.ApiKey = exceptionlessKey);
}
builder.Services.AddProblemDetails();
builder.Host.UseSerilog((context, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Week4Week5.Api")
    .WriteTo.Console()
    .WriteTo.Seq(context.Configuration["Seq:Url"] ?? "http://localhost:5341"));
builder.Services.AddSingleton<OrderStore>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(builder.Configuration["Portal:Origin"] ?? "http://localhost:3000")
        .AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddHealthChecks();

var app = builder.Build();
app.UseExceptionHandler();
if (!string.IsNullOrWhiteSpace(exceptionlessKey))
{
    app.UseExceptionless();
}
app.UseSerilogRequestLogging();
app.UseCors();
app.MapHealthChecks("/health");

var orders = app.MapGroup("/api/orders").WithTags("Orders");
orders.MapGet("/", (OrderStore store) => Results.Ok(store.List()))
    .WithName("ListOrders").Produces<OrderResponse[]>();
orders.MapGet("/{id:guid}", (Guid id, OrderStore store) =>
    (IResult)(store.Find(id) is { } order ? Results.Ok(order) : Results.NotFound()))
    .WithName("GetOrder").Produces<OrderResponse>().ProducesProblem(404);
orders.MapPost("/", async (CreateOrderRequest request, OrderStore store, IConfiguration configuration,
    ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    var errors = Validate(request);
    if (errors.Count > 0) return Results.ValidationProblem(errors);

    var order = store.Create(request);
    var workflowStarted = false;
    try
    {
        var client = await TemporalClient.ConnectAsync(new TemporalClientConnectOptions
        {
            TargetHost = configuration["Temporal:Address"] ?? "localhost:7233",
        });
        await client.StartWorkflowAsync(
            (NotificationWorkflow workflow) => workflow.RunAsync(order.Id, order.Customer, order.Email),
            new(id: $"order-confirmation-{order.Id}", taskQueue: "notifications"));
        workflowStarted = true;
        logger.LogInformation("Started order confirmation workflow for {OrderId}", order.Id);
    }
    catch (Exception exception)
    {
        logger.LogError(exception, "Could not start confirmation workflow for {OrderId}", order.Id);
    }

    return Results.Created($"/api/orders/{order.Id}", order with { EmailWorkflowStarted = workflowStarted });
}).WithName("CreateOrder").Produces<OrderResponse>(201).ProducesValidationProblem();
orders.MapPut("/{id:guid}", (Guid id, UpdateOrderRequest request, OrderStore store) =>
{
    var errors = Validate(request);
    if (errors.Count > 0) return Results.ValidationProblem(errors);

    var updated = store.Update(id, request);
    return updated is null ? Results.NotFound() : Results.Ok(updated);
}).WithName("UpdateOrder").Produces<OrderResponse>().ProducesProblem(404).ProducesValidationProblem();
orders.MapDelete("/{id:guid}", (Guid id, OrderStore store) =>
    store.Delete(id) ? Results.NoContent() : Results.NotFound())
    .WithName("DeleteOrder").Produces(204).ProducesProblem(404);

app.Run();

static Dictionary<string, string[]> Validate(object request)
{
    var results = new List<ValidationResult>();
    Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
    return results.SelectMany(result => result.MemberNames.DefaultIfEmpty("request")
            .Select(member => new { Member = member, Message = result.ErrorMessage ?? "Invalid value." }))
        .GroupBy(item => item.Member)
        .ToDictionary(group => group.Key, group => group.Select(item => item.Message).ToArray());
}
