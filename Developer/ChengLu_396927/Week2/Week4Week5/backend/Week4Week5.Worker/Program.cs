using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Serilog;
using Temporalio.Activities;
using Temporalio.Client;
using Temporalio.Worker;
using Week4Week5.Api;

var logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.Enrich.WithProperty("Application", "Week4Week5.Worker")
	.WriteTo.Console()
	.WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL") ?? "http://localhost:5341")
	.CreateLogger();
Log.Logger = logger;

var temporalAddress = Environment.GetEnvironmentVariable("TEMPORAL_ADDRESS") ?? "localhost:7233";
var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "localhost";
var smtpPort = int.TryParse(Environment.GetEnvironmentVariable("SMTP_PORT"), out var port) ? port : 1025;
var client = await TemporalClient.ConnectAsync(new TemporalClientConnectOptions { TargetHost = temporalAddress });
using var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
	eventArgs.Cancel = true;
	cancellation.Cancel();
};

var activities = new NotificationActivities(smtpHost, smtpPort, logger);
using var worker = new TemporalWorker(client, new TemporalWorkerOptions("notifications")
	.AddActivity(activities.SendOrderConfirmationAsync)
	.AddWorkflow<NotificationWorkflow>());

logger.Information("Notification worker listening on {TaskQueue}", "notifications");
await worker.ExecuteAsync(cancellation.Token);

public sealed class NotificationActivities(string smtpHost, int smtpPort, Serilog.ILogger logger) : INotificationActivities
{
	[Activity]
	public async Task SendOrderConfirmationAsync(Guid orderId, string customer, string email)
	{
		var message = new MimeMessage();
		message.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("MAIL_FROM") ?? "orders@example.test"));
		message.To.Add(MailboxAddress.Parse(email));
		message.Subject = $"Order {orderId.ToString()[..8]} received";
		message.Body = new TextPart("plain")
		{
			Text = $"Hello {customer},\n\nWe have received your order ({orderId}) and will begin processing it shortly.",
		};

		using var smtp = new SmtpClient();
		await smtp.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.None);
		await smtp.SendAsync(message);
		await smtp.DisconnectAsync(true);
		logger.Information("Order confirmation sent for {OrderId} to {Recipient}", orderId, email);
	}
}
