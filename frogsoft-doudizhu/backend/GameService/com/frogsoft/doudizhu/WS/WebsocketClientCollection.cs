using System.Collections.Generic;
using System.Linq;

namespace backend.GameService.com.frogsoft.doudizhu.WS
{
    public class WebsocketClientCollection
    {
        private static List<WebsocketClient> _clients = new List<WebsocketClient>();

        public static void Add(WebsocketClient client)
        {
            _clients.Add(client);
        }

        public static void Remove(WebsocketClient client)
        {
            _clients.Remove(client);
        }

        public static void RemoveById(string websocketId)
        {
            _clients.RemoveAll(c=>c.Id==websocketId);
        }

        public static WebsocketClient GetByClientId(string clientId)
        {
            var client = _clients.FirstOrDefault(c => c.Id == clientId);

            return client;
        }

        public static List<WebsocketClient> GetRoomClients(string roomNo)
        {
            var client = _clients.Where(c => c.RoomNo == roomNo);
            return client.ToList();
        }

    }
}
