using Microsoft.AspNetCore.SignalR;

public class DisasterReportHub : Hub
{
    public async Task JoinReportGroup(string topicId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"topic-{topicId}");
        await Clients.Caller.SendAsync("JoinedGroup", $"topic-{topicId}");
    }

    public async Task LeaveReportGroup(string topicId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"topic-{topicId}");
        await Clients.Caller.SendAsync("LeftGroup", $"topic-{topicId}");
    }
}