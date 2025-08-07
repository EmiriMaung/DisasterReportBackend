using DisasterReport.Shared.SignalR;

namespace DisasterReport.WebApi.SignalR;
public class NotificationHub : BaseNotificationHub
{
    public async Task SendInvitation(string userId, string organizationName, string invitedBy)
    {
        await Clients.User(userId).ReceiveInvitation(organizationName, invitedBy);
    }

}
