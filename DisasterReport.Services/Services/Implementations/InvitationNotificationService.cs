using DisasterReport.Shared.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace DisasterReport.Services.Services;

public class InvitationNotificationService
{
    private readonly IHubContext<BaseNotificationHub, INotificationClient> _hubContext;

    public InvitationNotificationService(IHubContext<BaseNotificationHub, INotificationClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyUserAsync(string userId, string organizationName, string invitedBy)
    {
        var connectionIds = BaseNotificationHub.GetConnectionIds(userId);

        foreach (var connectionId in connectionIds)
        {
            await _hubContext.Clients.Client(connectionId).ReceiveInvitation(organizationName, invitedBy);
        }
    }
}
