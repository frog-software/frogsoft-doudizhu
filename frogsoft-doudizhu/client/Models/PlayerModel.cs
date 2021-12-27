using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Models
{
    internal class PlayerModel
    {
        public string Id { get; set; }
        public PlayerStatus Status { get; set; } = PlayerStatus.NOT_READY;

        public List<int> CardsInHand = new List<int>();

        public List<int> CardsOut = new List<int>();

        //public string Avatar = "";
        public int CallScore = -1;

        public WinStatus IsWin = WinStatus.UNDEF;

        public int Passed = 0;

        public string ConnectionId { get; set; }
    }

    public enum PlayerStatus
    {
        NOT_READY,
        READY,
        LANDLORD,
        PEASANT
    }

    public enum WinStatus
    {
        WIN,
        LOSE,
        UNDEF
    }
}
