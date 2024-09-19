using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net.WebSockets;
using WebSocketManager;

namespace InstaHub.Models
{
    public class MessageSocketHandler : WebSocketHandler
    {
        private readonly WebSocketConnectionManager connectionManager;
        public MessageSocketHandler(WebSocketConnectionManager connectionManager)
          : base(connectionManager)
        {

        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);
            var socketId = connectionManager.GetId(socket);
            WebSocketManager.Common.Message message = new()
            {
                Data = $"Socket {socketId} is now connected."
            };
            await SendMessageAsync(socket, message);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            WebSocketManager.Common.Message webMessage = new()
            {
                Data = message
            };

            foreach (var socket in connectionManager.GetAll())
            {
                await SendMessageAsync(socket.Value, webMessage);
            }

        }

    }
}