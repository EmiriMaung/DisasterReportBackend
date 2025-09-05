using DisasterReport.Shared.SignalR;
using Microsoft.AspNetCore.SignalR;

//For Future Improvements
namespace DisasterReport.WebApi.SignalR
{
    public class ChatHub : Hub<IChatClient>
    {
        public async Task SendDirectMessage(string toUserId, ChatMessageDto message)
        {
            await Clients.User(toUserId).ReceiveDirectMessage(message);
        }

        public async Task SendTyping(string toUserId, TypingDto typing)
        {
            await Clients.User(toUserId).ReceiveTyping(typing);
        }

        public async Task SendReadReceipt(string toUserId, ReadReceiptDto receipt)
        {
            await Clients.User(toUserId).ReceiveReadReceipt(receipt);
        }
    }
}
