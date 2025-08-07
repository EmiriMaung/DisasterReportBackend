// DisasterReport.Shared/SignalR/BaseNotificationHub.cs
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace DisasterReport.Shared.SignalR;

public abstract class BaseNotificationHub : Hub<INotificationClient>
{
    // Shared connection tracker
    protected static readonly ConcurrentDictionary<string, HashSet<string>> userConnections = new();

    public override Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;

        if (!string.IsNullOrEmpty(userId))
        {
            var connections = userConnections.GetOrAdd(userId, _ => new HashSet<string>());
            lock (connections)
            {
                connections.Add(Context.ConnectionId);
            }
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;

        if (!string.IsNullOrEmpty(userId))
        {
            if (userConnections.TryGetValue(userId, out var connections))
            {
                lock (connections)
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        userConnections.TryRemove(userId, out _);
                    }
                }
            }
        }

        return base.OnDisconnectedAsync(exception);
    }

    public static IEnumerable<string> GetConnectionIds(string userId)
    {
        if (userConnections.TryGetValue(userId, out var connections))
        {
            lock (connections)
            {
                return connections.ToList();
            }
        }

        return Enumerable.Empty<string>();
    }
}
