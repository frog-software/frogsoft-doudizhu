namespace backend.GameService.com.frogsoft.doudizhu.Models
{
    public class GameModel
    {
        public MessageType MessageType {  get; set; }
        public List<PlayerModel> Players {  get; set; } = new List<PlayerModel>();

        // public string LastPlayer {  get; set; }
        public string CurrentPlayer {  get; set; }

        public string CurrentPlayerConnectionId {  get; set; }
        public string Message {  get; set; } = new string("");

        public List<int> LastCombination = new List<int>();

        public string RoomNo {  get; set; }

        public bool AddPlayer(string playerId)
        {
            var existingPlayer = Players.FirstOrDefault(p => p.Id == playerId);
            if(existingPlayer != null)
            {
                return false;
            }

            var newPlayer = new PlayerModel();
            newPlayer.Id = playerId;

            Players.Add(newPlayer);

            return true;
        }

        public PlayerModel GetPlayerById(string id)
        {
            if(id == null && Players.Count >= 1)
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
                return null;
            }

            int nextIdx = idx + 1;

            if(nextIdx >= Players.Count)
            {
                nextIdx = 0;
            }

            return Players[nextIdx];
        }

        public void RemovePlayerById(string playerId)
        {
            Players.RemoveAll(p=> p.Id == playerId);
        }

        public bool DetermineLandlord()
        {
            if (Players.Count < 3)
            {
                return false;
            }
            bool allReady = true;
            bool allCalled = true;
            foreach (PlayerModel player in Players)
            {
                if (player.Status != PlayerStatus.READY)
                {
                    allReady = false;
                }
                
                if(player.CallScore == -1)
                {
                    allCalled = false;
                }
            }
            if(!allReady)
            {
                return false;
            }


            PlayerModel playerWithMaxScore = null;
            int maxScore = 0;
            foreach (PlayerModel player in Players)
            {
                if(player.CallScore > maxScore)
                {
                    maxScore = player.CallScore;
                    playerWithMaxScore = player;
                }
            }

            if(maxScore == 3 || allCalled)
            {
                foreach (PlayerModel player in Players)
                {
                    player.Status = PlayerStatus.PEASANT;
                    if (player.Id == playerWithMaxScore.Id)
                    {
                        player.Status = PlayerStatus.LANDLORD;
                    }
                }
                return true;
            }



            return false;
        }
       
    }

    public enum MessageType
    {
        JOIN,
        LEAVE,
        UPDATE
    }
}
