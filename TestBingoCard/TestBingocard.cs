using Microsoft.VisualStudio.TestTools.UnitTesting;
using SteamBingoGame;
namespace TestBingoCard
{
    [TestClass]
    public class TestBingocard
    {
        Lobby Lobby;
        Player Player;
        public TestBingocard()
        {
            Lobby = new Lobby(1);
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