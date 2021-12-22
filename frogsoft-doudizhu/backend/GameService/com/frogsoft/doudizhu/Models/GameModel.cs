namespace backend.GameService.com.frogsoft.doudizhu.Models
{
    public class GameModel
    {
        public MessageType MessageType { get; set; }
        public List<PlayerModel> Players { get; set; } = new List<PlayerModel>();

        // public string LastPlayer {  get; set; }
        public string CurrentPlayer { get; set; }

        public string CurrentPlayerConnectionId { get; set; }
        public string Message { get; set; } = new string("");

        public List<int> LastCombination = new List<int>();

        public string LastPlayer {  get; set; }

        public string RoomNo { get; set; }

        public List<int> list = new List<int>();

        public bool HasGameStarted {  get; set; } = false;


        public bool AddPlayer(string playerId)
        {
            var existingPlayer = Players.FirstOrDefault(p => p.Id == playerId);
            if (existingPlayer != null)
            {
                return false;
            }

            var newPlayer = new PlayerModel();
            newPlayer.Id = playerId;

            Players.Add(newPlayer);

            return true;
        }

        public PlayerModel GetPlayerByStatus(PlayerStatus status)
        {
            return Players.FirstOrDefault(p => p.Status == status);
        }

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

        public PlayerModel GetLandlord()
        {
            return Players.FirstOrDefault(p => p.Status == PlayerStatus.LANDLORD);
        }

        public void RemovePlayerById(string playerId)
        {
            Players.RemoveAll(p => p.Id == playerId);
        }

        public bool IsPlayersAllReady()
        {
            if (Players.Count < 3)
            {
                return false;
            }
            bool allReady = true;
            foreach (PlayerModel player in Players)
            {
                if (player.Status == PlayerStatus.NOT_READY)
                {
                    allReady = false;
                }
            }
            return allReady;

        }

        public bool IsPlayersAllCalled()
        {
            if (Players.Count < 3)
            {
                return false;
            }
            bool allCalled = true;
            foreach (PlayerModel player in Players)
            {
                if (player.CallScore == -1)
                {
                    allCalled = false;
                }
            }
            return allCalled;

        }

        public bool IsLandlordDetermined()
        {
            if (Players.Count < 3)
            {
                return false;
            }

            foreach (PlayerModel player in Players)
            {
                if (player.Status == PlayerStatus.LANDLORD || player.Status == PlayerStatus.PEASANT)
                    return true;
            }
            return false;
        }

        public bool IsPlayerAllNoCards()
        {
            bool allNoCards = true;
            foreach (PlayerModel player in Players)
            {
                if (player.CardsInHand.Count > 0)
                {
                    allNoCards = false;
                }
            }

            return allNoCards;
        }

        public void AssignCards()
        {

            for(int i = 0; i<54; i++)
            {
                list.Add(i);
            }

            Random rng = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            for (int i = 0; i < 51; i++)
            {
                int currentPlayer = i / 17;

                Players[currentPlayer].CardsInHand.Add(list[i]);
            }
        }

        public void AssignLandlordCards()
        {
            for (int i = 51; i < 54; i++)
            {
                var player = GetPlayerByStatus(PlayerStatus.LANDLORD);
                player.CardsInHand.Add(list[i]);
            }
        }

        public bool DetermineLandlord()
        {
            bool allReady = IsPlayersAllReady();
            bool allCalled = IsPlayersAllCalled();
            if (!allReady)
            {
                return false;
            }


            PlayerModel playerWithMaxScore = null;
            int maxScore = 0;
            foreach (PlayerModel player in Players)
            {
                if (player.CallScore > maxScore)
                {
                    maxScore = player.CallScore;
                    playerWithMaxScore = player;
                }
            }

            if (maxScore == 3 || allCalled)
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
