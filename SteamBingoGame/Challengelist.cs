namespace SteamBingoGame
{
    public class Challenge
    {
        public string Discription { get; set; }
        public string StatName { get; set; }
        public int Value { get; set; }
        public int Difficulty { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
    }

    public class Challengelist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxCardSize { get; set; }
        public List<Challenge> Challenges { get; set; }
    }
}
