namespace SteamBingoGame
{
    public class Challenge
    {
        public string discription { get; set; } = "";
        public string statName { get; set; } = "";
        public int value { get; set; }
        public int difficulty { get; set; }
        public int gameid { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
    }

    public class Challengelist
    {
        public int gameId { get; set; }
        public int id { get; set; }
        public string name { get; set; } = "";
        public int maxCardSize { get; set; }
        public List<Challenge> challenges { get; set; } = new List<Challenge>();
    }
}
