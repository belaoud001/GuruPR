using Microsoft.AspNetCore.SignalR;

namespace GuruPR.Hubs;

public class ChatHub : Hub
{
    public async Task NewMessage(string message)
    {
        await Clients.All.SendAsync("message-received", message);
    }
}
