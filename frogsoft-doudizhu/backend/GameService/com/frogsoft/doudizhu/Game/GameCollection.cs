using backend.GameService.com.frogsoft.doudizhu.Models;

namespace backend.GameService.com.frogsoft.doudizhu.Game
{
    public class GameCollection
    {
        public static List<GameModel> Games = new List<GameModel>();

        public static bool AddOrJoinGame(string roomNo, string currentPlayer)
        {
            var existingGame = GetGameByRoomNo(roomNo);

            if (existingGame != null)
            {
                if (existingGame.Players.Count >= 3)
                {
                    return false;
                }
                existingGame.AddPlayer(currentPlayer);
                Console.WriteLine("Added player " + currentPlayer + " to room " + roomNo);

                return true;
            }

            var game = new GameModel();
            game.RoomNo = roomNo;
            game.AddPlayer(currentPlayer);
            Games.Add(game);

            Console.WriteLine("Created room " + roomNo);

            return true;
        }

        public static bool UpdateGame(GameModel remoteGame)
        {
            var existingGame = GetGameByRoomNo(remoteGame.RoomNo);

            if (existingGame == null)
            {
                return false;
            }


            // existingGame.CurrentPlayer = remoteGame.CurrentPlayer;

            var playerInRemoteGame = remoteGame.GetPlayerById(remoteGame.CurrentPlayer);
            var playerId = existingGame.CurrentPlayer == null ? remoteGame.CurrentPlayer : existingGame.CurrentPlayer;
            var playerInExistingGame = existingGame.GetPlayerById(playerId);

            // existingGame.CurrentPlayer = playerInExistingGame.Id;

            playerInExistingGame.CopyFrom(playerInRemoteGame);



            // the remote current player will soon be the last
            existingGame.LastCombination = playerInRemoteGame.CardsOut;


            if (existingGame.LastPlayer == playerInRemoteGame.Id)
            {
                existingGame.Players[0].CardsOut = new List<int>();
                existingGame.Players[1].CardsOut = new List<int>();
                existingGame.Players[2].CardsOut = new List<int>();
            }

            if (playerInRemoteGame.CardsOut.Count > 0)
            {
                existingGame.LastPlayer = playerInExistingGame.Id;
            }


            existingGame.DetermineLandlord();

            if (existingGame.IsPlayersAllReady())
            {
                // move to next player
                existingGame.CurrentPlayer = existingGame.GetNextPlayerById(existingGame.CurrentPlayer).Id;

                if (existingGame.IsPlayerAllNoCards())
                {
                    existingGame.AssignCards();
                }

                if (existingGame.IsPlayersAllCalled() || existingGame.IsLandlordDetermined())
                {
                    existingGame.AssignLandlordCards();
                    existingGame.CurrentPlayer = existingGame.GetLandlord().Id;
                }
            }




            return true;
        }

        public static GameModel GetGameByRoomNo(string roomNo)
        {
            return Games.FirstOrDefault(g => g.RoomNo == roomNo);
        }

        public static void RemoveGame(string roomNo)
        {
            Games.RemoveAll(g => g.RoomNo == roomNo);
        }
    }
}
