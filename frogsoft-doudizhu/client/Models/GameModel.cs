using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Models
{
    internal class GameModel
    {
        public MessageType MessageType { get; set; }
        public List<PlayerModel> Players { get; set; } = new List<PlayerModel>();

        // public string LastPlayer {  get; set; }
        public string CurrentPlayer { get; set; }

        public string CurrentPlayerConnectionId { get; set; }
        public string Message { get; set; } = new string("");

        public List<int> LastCombination = new List<int>();

        public string LastPlayer { get; set; }

        public string RoomNo { get; set; }

        public List<int> list = new List<int>();

        public bool HasGameStarted { get; set; } = false;

        public PlayerModel GetPlayerById(string id)
        {
            if (id == null && Players.Count >= 1)
            {
                return Players[0];
            }

            return Players.FirstOrDefault(p => p.Id == id);
        }

        public PlayerModel GetNextPlayerById(string id)
        {
            int idx = Players.FindIndex(p => p.Id == id);

            if (idx == -1)
            {
                return Players[0];
            }

            int nextIdx = idx + 1;

            if (nextIdx >= Players.Count)
            {
                nextIdx = 0;
            }

            return Players[nextIdx];
        }
    }

    public enum MessageType
    {
        JOIN,
        LEAVE,
        UPDATE
    }
}
