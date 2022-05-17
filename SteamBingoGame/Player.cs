using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.Json;

namespace SteamBingoGame
{
    public class Player
    {
        public string SteamId { get; set; }
        public string Name { get; set; }
        public byte[] Pic { get; set; }
        public Dictionary<string, double> BeginStats;
        public Dictionary<string, double> Stats;

        private string[] Url = { "http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?appid=", "&key=B28FAD6C2B1A54EA2342EA465206F5A7&steamid=" };
        private bool ValidId = false;
        private bool Loading = true;

        private string connectionString = "Server=am1.fcomet.com;Uid=steambin_steambin;Database=steambin_Data;Pwd=Appels1peren0";
        private MySqlConnection connection;
        private string query;

        public Player(string steamid, string name)
        {
            SteamId = steamid;
            Name = name;
            BeginStats = new Dictionary<string, double>();
            Stats = new Dictionary<string, double>();

            connection = new MySqlConnection(connectionString);
        }

        public async Task<bool> CheckSteamid(int gameid)
        {
            try
            {
                HttpClient client = new HttpClient();
                string response = await client.GetStringAsync("http://api.steampowered.com/ISteamUserStats/GetUserStatsForGame/v0002/?appid=" + gameid + "&key=B28FAD6C2B1A54EA2342EA465206F5A7&steamid=" + SteamId);
                if (response.Length > 50)
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

        public async Task<string> GetStatsString(int gameid)
        {
            try
            {
                HttpClient client = new HttpClient();
                string response = await client.GetStringAsync(Url[0]+ gameid + Url[1] + SteamId);
                //string fakeResponse = await client.GetStringAsync("https://api.npoint.io/f81c7c0323f3f885cffa");

                ValidId = true;
                Loading = false;
                return response;
                //return fakeResponse;
            }
            catch
            {
                ValidId = false;
                Loading = false;
                return "";
            }
            
        }

        public async Task<int> Update()
        {
            try
            {
                connection.Open();
                query = $"UPDATE `Player` SET `BeginStats`='{JsonSerializer.Serialize(BeginStats)}' WHERE `Steamid` = '{SteamId}' AND `Name` = '{Name}'";
                var cmd = new MySqlCommand(query, connection);
                cmd.ExecuteScalar();
            }
            catch
            {
                return 0;
            }
            return 1;
        }
    }
}
