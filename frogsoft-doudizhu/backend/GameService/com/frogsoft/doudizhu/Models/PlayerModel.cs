namespace backend.GameService.com.frogsoft.doudizhu.Models
{
    public class PlayerModel
    {
        public string Id {  get; set; } 
        public PlayerStatus Status { get; set; } = PlayerStatus.NOT_READY;

        public List<int> CardsInHand = new List<int>();  

        public List<int> CardsOut = new List<int>();

        //public string Avatar = "";
        public int CallScore = -1;

        public WinStatus IsWin = WinStatus.UNDEF;

        public int Passed = 0;

        public string ConnectionId { get; set; }

        
        public bool Move(List<int> combination)
        {

            return true;
        }

        public void CopyFrom(PlayerModel from)
        {
            this.Status = from.Status;
            this.CardsInHand = from.CardsInHand;
            this.CardsOut = from.CardsOut;
            this.CallScore = from.CallScore;
        }
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
