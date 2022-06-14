using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamBingoGame;
using System.Net.Http;
using System.Text.Json;

namespace TestBingoCard
{
    [TestClass]
    public class TestBingocard
    {
        Lobby Lobby;
        Player Player;
        public TestBingocard()
        {
            string challengelist = "{\"id\":1,\"name\":\"test\",\"gameId\":381210,\"challenges\":[{\"id\":8,\"value\":1,\"gameid\":381210,\"statName\":\"DBD_Chapter12_Camper_Stat1\",\"difficulty\":2,\"discription\":\"Wiggle from the killers grasp\"},{\"id\":7,\"value\":1,\"gameid\":381210,\"statName\":\"DBD_Chapter9_Camper_Stat1\",\"difficulty\":2,\"discription\":\"Unhook yourself 1 time\"},{\"id\":4,\"value\":3,\"gameid\":381210,\"statName\":\"DBD_DLC7_Camper_Stat1\",\"difficulty\":1,\"discription\":\"Open 3 chests\"},{\"id\":9,\"value\":1,\"gameid\":381210,\"statName\":\"DBD_CamperNewItem\",\"difficulty\":2,\"discription\":\"Escaped 1 time with a new item\"},{\"id\":6,\"value\":1,\"gameid\":381210,\"statName\":\"DBD_Chapter21_Camper_Stat1\",\"difficulty\":3,\"discription\":\"Bless 1 hex totem\"},{\"id\":10,\"value\":2,\"gameid\":381210,\"statName\":\"DBD_Escape\",\"difficulty\":1,\"discription\":\"Escape 2 trials while knocked down\"},{\"id\":3,\"value\":2,\"gameid\":381210,\"statName\":\"DBD_Chapter10_Camper_Stat1\",\"difficulty\":1,\"discription\":\"Sabotage 2 hooks\"},{\"id\":2,\"value\":2,\"gameid\":381210,\"statName\":\"DBD_Chapter18_Camper_Stat1\",\"difficulty\":2,\"discription\":\"Save a survivor from the killers grasp using a pallet\"},{\"id\":5,\"value\":2,\"gameid\":381210,\"statName\":\"DBD_DLC7_Camper_Stat2\",\"difficulty\":2,\"discription\":\"Open 2 exitgates\"}],\"maxCardSize\":0,\"iChallengelist\":{}}";
            Lobby = new Lobby(1,1);
            Lobby.Challengelist = new Challengelist();
            Lobby.Challengelist = JsonSerializer.Deserialize<Challengelist>(challengelist);
            Lobby.StartGame();
            Player = new Player("76561198290099510", "Pepijn");
        }

        [TestMethod]
        public void testBingovertical()
        {
            Lobby.Board[1][0].Players.Add(Player);
            Lobby.Board[1][1].Players.Add(Player);
            Lobby.Board[1][2].Players.Add(Player);

            Lobby.CheckWinner(Player);
            Assert.IsTrue(Lobby.Winners.Contains(Player));
        }
        [TestMethod]
        public void testBingohorizontal()
        {
            Lobby.Board[0][1].Players.Add(Player);
            Lobby.Board[1][1].Players.Add(Player);
            Lobby.Board[2][1].Players.Add(Player);

            Lobby.CheckWinner(Player);
            Assert.IsTrue(Lobby.Winners.Contains(Player));
        }
        [TestMethod]
        public void testBingodiagonal()
        {
            Lobby.Board[0][0].Players.Add(Player);
            Lobby.Board[1][1].Players.Add(Player);
            Lobby.Board[2][2].Players.Add(Player);

            Lobby.CheckWinner(Player);
            Assert.IsTrue(Lobby.Winners.Contains(Player));
        }
        [TestMethod]
        public void testBingodiagonal2()
        {
            Lobby.Board[0][2].Players.Add(Player);
            Lobby.Board[1][1].Players.Add(Player);
            Lobby.Board[2][0].Players.Add(Player);

            Lobby.CheckWinner(Player);
            Assert.IsTrue(Lobby.Winners.Contains(Player));
        }
    }
}