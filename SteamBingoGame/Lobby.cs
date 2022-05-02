namespace SteamBingoGame
{
    public class Lobby
    {
        public int Id { get;private set; }
        public bool Open { get; private set; }
        public List<Player> Players { get; private set; }
        public int ChallengeListId { get; private set; }

        public Lobby(int id)
        {
            Random random = new Random();
            Id = random.Next(1000, 10000);
            Players = new List<Player>();
            Open = true;
            ChallengeListId = id;
        }

        public int AddPlayer(Player player)
        {
            if (Players.Find(p => p.SteamId == player.SteamId) != null) return 0;
            if (Players.Find(p => p.Name == player.Name) != null) return -1;
            Players.Add(player);
            return 1;
        }
    }
}
