﻿namespace SteamBingoGame
{
    public class Player
    {
        public long SteamId { get; set; }
        public string Name { get; set; }
        public Dictionary<string, double> BeginStats;
        public Dictionary<string, double> Stats;

        private string Url = "http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?appid=381210&key=B28FAD6C2B1A54EA2342EA465206F5A7&steamid=";
        private bool ValidId = false;
        private bool Loading = true;

        public Player(long steamid, string name)
        {
            SteamId = steamid;
            Name = name;
            BeginStats = new Dictionary<string, double>();
            Stats = new Dictionary<string, double>();
        }

        public bool CheckSteamid()
        {
            Loading = true;
            GetStatsString();
            while (Loading)
            {

            }
            return ValidId;
        }

        public async Task<string> GetStatsString()
        {
            try
            {
                HttpClient client = new HttpClient();
                string response = await client.GetStringAsync(Url + SteamId);
                ValidId = true;
                Loading = false;
                return response;
            }
            catch
            {
                ValidId = false;
                Loading = false;
                return "";
            }
            
        }
    }
}
