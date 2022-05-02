namespace SteamBingoGame
{
    public class Player
    {
        public int SteamId { get; set; }
        public string Name { get; set; }

        public Player(int steamid, string name)
        {
            SteamId = steamid;
            Name = name;
        }
    }
}
