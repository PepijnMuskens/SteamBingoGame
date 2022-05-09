using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace SteamBingoGame
{
    public class Lobby
    {
        public int Id { get; set; }
        public bool Open { get; private set; }
        public List<Player> Players { get; private set; }
        public int ChallengeListId { get; private set; }
        public Challengelist Challengelist { get; private set; }
        public List<List<Challenge>> Board { get; private set; }

        private string connectionString = "Server=am1.fcomet.com;Uid=steambin_steambin;Database=steambin_Data;Pwd=Appels1peren0";
        private MySqlConnection connection;
        private string query;
        private bool wait = true;
        public Lobby(int id)
        {
            connection = new MySqlConnection(connectionString);

            wait = true;
            try
            {
                GetChallenges(id);
                connection.Open();
                Random random = new Random();
                while(true)
                {
                    Id = random.Next(1000, 10000);
                    query = $"SELECT Id FROM `Lobby` WHERE Id = {Id} AND Open = 1";
                    var cmd = new MySqlCommand(query, connection);
                    if (cmd.ExecuteScalar() == null) break;
                }
                query = $"INSERT INTO `Lobby`(`Id`, `Open`, `Challengelistid`) VALUES ({Id},1,{id})";
                var cmd2 = new MySqlCommand(query, connection);
                cmd2.ExecuteNonQuery();
                Players = new List<Player>();
                Open = true;
                ChallengeListId = id;
                Board = new List<List<Challenge>>();
                while (wait)
                {

                }
                if (Challengelist.name == null) Challengelist = new Challengelist();
            }
            catch
            {
                connection.Close();
            }
            connection.Close();
        }
        public Lobby(int lobbyid, int chid, bool open)
        {
            wait = true;
            GetChallenges(chid);
            Id = lobbyid;
            ChallengeListId= chid;
            Open = open;
            Players = new List<Player>();
            while (wait)
            {

            }
        }

        /// <summary>
        /// Adds player to the lobby
        /// </summary>
        /// <param name="player"></param>
        /// <returns>
        /// 1 if success
        /// 0 if double steam id
        /// -1 if double name
        /// -2 if lobby is closed
        /// -3 if not a valid steam id 
        /// </returns>
        public int AddPlayer(Player player)
        {
            if (Players.Find(p => p.SteamId == player.SteamId) != null) return 0;
            if (Players.Find(p => p.Name == player.Name) != null) return -1;
            if (!Open) return -2;
            if(!player.CheckSteamid()) return -3;
            Players.Add(player);
            return 1;
        }

        public async Task StartGame()
        {
            if(!Open) return;
            await CreateBoard();
            foreach(Player player in Players)
            {
                HttpClient client = new HttpClient();
                string stats = await player.GetStatsString();
                for (int i = 0; i < Board.Count(); i++)
                {
                    for (int j = 0; j < Board[i].Count; j++)
                    {
                        Challenge challenge = Board[i][j];
                        JObject data = JObject.Parse(stats);
                        double test = (double)data.SelectToken("playerstats.stats[?(@.name == '"+ challenge.statName +"')].value");
                        player.BeginStats.Add(challenge.statName, test);
                    }
                }
            }
            Open = false;
            return;
        }

        private async Task CreateBoard()
        {
            int size = (int)Math.Sqrt(Challengelist.challenges.Count());
            Board = new List<List<Challenge>>();
            Random random = new Random();
            for(int i = 0; i < size; i++)
            {
                Board.Add(new List<Challenge>());
                for (int j = 0; j < size; j++)
                {
                    Challenge challenge = Challengelist.challenges[random.Next(Challengelist.challenges.Count() - 1)];
                    Board[i].Add(challenge);
                    Challengelist.challenges.Remove(challenge);
                }
            }

        }

        private async void GetChallenges(int id)
        {
            HttpClient Client = new HttpClient();
            var challengelist = await Client.GetStringAsync("https://i437675.luna.fhict.nl/steambingo/getchallengelist?id=" + id);
            try
            {
                Challengelist = new Challengelist();
                Challengelist = JsonSerializer.Deserialize<Challengelist>(challengelist);
            }
            catch(Exception ex)
            {
            }
            wait = false;
        }


    }
}
