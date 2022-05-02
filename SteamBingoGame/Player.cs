namespace SteamBingoGame
{
    public class Player
    {
        public int SteamId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, int> BeginStats;
        public Dictionary<string, int> Stats;

        private string Url = "http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?appid=381210&key=B28FAD6C2B1A54EA2342EA465206F5A7&steamid=";
        private bool ValidId = false;
        private bool Loading = true;

        public Player(int steamid, string name)
        {
            SteamId = steamid;
            Name = name;
            BeginStats = new Dictionary<string, int>();
            Stats = new Dictionary<string, int>();
        }

        public bool CheckSteamid()
        {
            Loading = true;
            GetStats();
            while (Loading)
            {

            }
            return ValidId;
        }

        public async void GetStats()
        {
            try
            {
                HttpClient client = new HttpClient();
                string response = await client.GetStringAsync(Url + SteamId);
                ValidId = true;
                Loading = false;
            }
            catch
            {
                ValidId = false;
                Loading = false;
            }
        }
    }
}
