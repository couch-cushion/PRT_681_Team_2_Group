using Temporalio.Workflows;

namespace Week4Week5.Api;

[Workflow]
public sealed class NotificationWorkflow
{
    [WorkflowRun]
    public Task RunAsync(Guid orderId, string customer, string email) =>
        Workflow.ExecuteActivityAsync(
            (INotificationActivities activity) => activity.SendOrderConfirmationAsync(orderId, customer, email),
            new ActivityOptions
            {
                StartToCloseTimeout = TimeSpan.FromMinutes(2),
                RetryPolicy = new Temporalio.Common.RetryPolicy
                {
                    InitialInterval = TimeSpan.FromSeconds(2),
                    MaximumInterval = TimeSpan.FromMinutes(1),
                    MaximumAttempts = 5,
                },
            });
}

public interface INotificationActivities
{
    Task SendOrderConfirmationAsync(Guid orderId, string customer, string email);
}