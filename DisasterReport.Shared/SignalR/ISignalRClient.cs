
namespace DisasterReport.Shared.SignalR
{
    public interface INotificationClient
    {
        Task ReceiveInvitation(string organizationName, string invitedBy);
    }
}
