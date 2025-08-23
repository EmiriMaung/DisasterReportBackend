
namespace DisasterReport.Shared.SignalR
{
    public interface INotificationClient
    {
        Task ReceiveInvitation(string organizationName, string invitedBy);
        Task ReceiveDisasterAlert(string title, string message, string url);

    }
}
