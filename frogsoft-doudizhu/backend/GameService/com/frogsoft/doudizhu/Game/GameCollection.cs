﻿using backend.GameService.com.frogsoft.doudizhu.Models;

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
                if( existingGame.Players.Count >=3)
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

            if(existingGame == null)
            {
                return false;
            }

            var playerInRemoteGame = remoteGame.GetPlayerById(remoteGame.CurrentPlayer);
            var playerInExistingGame = existingGame.GetNextPlayerById(remoteGame.CurrentPlayer);

            existingGame.CurrentPlayer = playerInExistingGame.Id;

            playerInExistingGame.CopyFrom(playerInRemoteGame);

            existingGame.LastCombination = playerInRemoteGame.CardsOut;

            existingGame.DetermineLandlord();

            // move to next player
            existingGame.CurrentPlayer = existingGame.GetNextPlayerById(existingGame.CurrentPlayer).Id;


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
