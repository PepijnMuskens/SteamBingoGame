using Newtonsoft.Json.Linq;
using System.Net;

namespace SteamBingoGame
{
    public class Player
    {
        public string SteamId { get; set; }
        public string Name { get; set; }
        public byte[] Pic { get; set; }
        public Dictionary<string, double> BeginStats;
        public Dictionary<string, double> Stats;

        private string Url = "http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?appid=381210&key=B28FAD6C2B1A54EA2342EA465206F5A7&steamid=";
        private bool ValidId = false;
        private bool Loading = true;

        public Player(string steamid, string name)
        {
            SteamId = steamid;
            Name = name;
            BeginStats = new Dictionary<string, double>();
            Stats = new Dictionary<string, double>();
        }

        public async Task<bool> CheckSteamid()
        {
            try
            {
                HttpClient client = new HttpClient();
                string response = await client.GetStringAsync("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=B28FAD6C2B1A54EA2342EA465206F5A7&steamids=" + SteamId);
                JObject data = JObject.Parse(response);
                int test = (int)data.SelectToken("response.players[0].profilestate");
                if (test == 1)
                {
                    return true;
                }
            }
            catch
            {
                
            }
            return false;
        }
        public async Task<bool> GetPic()
        {
            try
            {
                HttpClient client = new HttpClient();
                string response = await client.GetStringAsync("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=B28FAD6C2B1A54EA2342EA465206F5A7&steamids=" + SteamId);
                JObject data = JObject.Parse(response);
                string test = (string)data.SelectToken("response.players[0].avatarfull");
                if (test != "")
                {
                    var webClient = new WebClient();
                    byte[] imageBytes = webClient.DownloadData(test);
                    Pic = imageBytes;
                    return true;
                }
            }
            catch
            {

            }
            return false;
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
