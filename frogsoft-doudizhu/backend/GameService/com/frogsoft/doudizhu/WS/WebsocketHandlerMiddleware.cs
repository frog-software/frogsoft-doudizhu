using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using backend.GameService.com.frogsoft.doudizhu.Models;
using backend.GameService.com.frogsoft.doudizhu.Game;

namespace backend.GameService.com.frogsoft.doudizhu.WS
{
    public class WebsocketHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public WebsocketHandlerMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory
            )
        {
            _next = next;
            _logger = loggerFactory.
                CreateLogger<WebsocketHandlerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/api/games/com/frogsoft/doudizhu/room")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    string clientId = Guid.NewGuid().ToString(); ;
                    var wsClient = new WebsocketClient
                    {
                        Id = clientId,
                        WebSocket = webSocket
                    };
                    try
                    {
                        await Handle(wsClient);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Echo websocket client {0} err .", clientId);
                        await context.Response.WriteAsync("closed");
                    }
                }
                else
                {
                    context.Response.StatusCode = 404;
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task Handle(WebsocketClient webSocket)
        {
            WebsocketClientCollection.Add(webSocket);
            _logger.LogInformation($"Websocket client added.");

            WebSocketReceiveResult result = null;
            do
            {
                var buffer = new byte[1024 * 1];
                result = await webSocket.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text && !result.CloseStatus.HasValue)
                {
                    var msgString = Encoding.UTF8.GetString(buffer);
                    _logger.LogInformation($"Websocket client ReceiveAsync message {msgString}.");
                    var message = JsonConvert.DeserializeObject<GameModel>(msgString);
                  
                    message.CurrentPlayerConnectionId = webSocket.Id;
                    MessageRoute(message);
                }
            }
            while (!result.CloseStatus.HasValue);

            WebsocketClientCollection.Remove(webSocket);
            _logger.LogInformation($"Websocket client closed.");
        }

        private void MessageRoute(GameModel message)
        {
            var client = WebsocketClientCollection.GetByClientId(message.CurrentPlayerConnectionId);
            _logger.LogInformation("recv msg" + JsonConvert.SerializeObject(message));
            switch (message.MessageType)
            {
                case MessageType.JOIN:
                    {
                        
                        client.RoomNo = message.RoomNo;
                        _logger.LogInformation("[JOIN] Websocket client sent " + JsonConvert.SerializeObject(message));
                        
                        if (!GameCollection.AddOrJoinGame(message.RoomNo, message.CurrentPlayer, message.CurrentPlayerConnectionId))
                        {
                            client.SendMessageAsync("{\"msg\": \"room full\"}");
                        }

                        var clients = WebsocketClientCollection.GetRoomClients(message.RoomNo);
                        clients.ForEach(c =>
                        {
                            c.SendMessageAsync(JsonConvert.SerializeObject(GameCollection.GetGameByRoomNo(client.RoomNo)));
                        });


                        _logger.LogInformation($"Websocket client {message.CurrentPlayer} join room {client.RoomNo}.");
                        break;
                    }
                    
                case MessageType.UPDATE:
                    {
                        if (string.IsNullOrEmpty(client.RoomNo))
                        {
                            break;
                        }

                        _logger.LogInformation("[UPDATE] Websocket client sent " + JsonConvert.SerializeObject(message));

                        GameCollection.UpdateGame(message);

                       

                        var clients = WebsocketClientCollection.GetRoomClients(client.RoomNo);
                        clients.ForEach(c =>
                        {
                            c.SendMessageAsync(JsonConvert.SerializeObject(GameCollection.GetGameByRoomNo(client.RoomNo)));
                        });
                        _logger.LogInformation($"Websocket client {message.CurrentPlayer} updated {client.RoomNo}");

                        break;
                    }
                
                case MessageType.LEAVE:
                    {
                        var roomNo = client.RoomNo;
                        client.RoomNo = "";
                        client.SendMessageAsync($"{{\"msg\": \"player {message.CurrentPlayer} leave room {roomNo} success.\"}}");
                        _logger.LogInformation($"Websocket client {message.CurrentPlayer} leave room {roomNo}");
                        
                        var game = GameCollection.GetGameByRoomNo(roomNo);

                        game.EndGame(client.Id);

                        game.Players.RemoveAll(p => p.ConnectionId == client.Id);

                        _logger.LogInformation($"Room {roomNo} now has {game.Players.Count()} players");

                        if (game.Players.Count() <= 0)
                        {
                            GameCollection.RemoveGame(roomNo);
                            _logger.LogInformation($"Removed room {roomNo} beacause there is no players");
                        }
                        
                        break;
                    }
                case MessageType.NEW_GAME:
                    {

                        break;
                    }
                default:
                    break;
            }
        }
    }
}
