using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace SteamBingoGame
{
    public class Lobby
    {
        public int Id { get; set; }
        public bool Open { get; set; }
        public List<Player> Players { get; set; }
        public List<Player> Winners { get; set; }
        public int ChallengeListId { get; set; }
        public Challengelist Challengelist { get; set; }
        public List<List<Challenge>> Board { get; set; }

        private readonly string connectionString = "Server=am1.fcomet.com;Uid=steambin_steambin;Database=steambin_Data;Pwd=Appels1peren0";
        private readonly MySqlConnection connection;
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
                string query = "";
                while (true)
                {
                    Id = random.Next(1000, 10000);
                    query = $"SELECT Id FROM `Lobby` WHERE Id = {Id} AND Open = 1";
                    var cmd = new MySqlCommand(query, connection);
                    if (cmd.ExecuteScalar() == null)
                    {
                        break;
                    }
                }
                query = $"INSERT INTO `Lobby`(`Id`, `Open`, `Challengelistid`) VALUES ({Id},1,{id})";
                var cmd2 = new MySqlCommand(query, connection);
                cmd2.ExecuteNonQuery();
                Players = new List<Player>();
                Winners = new List<Player>();
                Open = true;
                ChallengeListId = id;
                Board = new List<List<Challenge>>();
                while (wait)
                {

                }
                if (Challengelist.name == null)
                {
                    Challengelist = new Challengelist();
                }
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
            ChallengeListId = chid;
            Open = open;
            Players = new List<Player>();
            Winners = new List<Player>();
            while (wait)
            {

            }
        }
        public Lobby(int lobbyid, int chid)
        {
            Open = true;
            Id = lobbyid;
            ChallengeListId = chid;
            Players = new List<Player>();
            Winners = new List<Player>();
            Challengelist = new Challengelist();
            Board = new List<List<Challenge>>();
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
            if (Players.Find(p => p.SteamId == player.SteamId) != null)
            {
                return 0;
            }
            if (Players.Find(p => p.Name == player.Name) != null)
            {
                return -1;
            }
            if (!Open) 
            {
                return -2;
            }
            Players.Add(player);
            return 1;
        }

        public async Task StartGame()
        {
            if (!Open)
            {
                return;
            }
            await CreateBoard();
            foreach (Player player in Players)
            {
                player.BeginStats = new Dictionary<string, double>();
                string stats = await player.GetStatsString(Challengelist.gameId);
                for (int i = 0; i < Board.Count(); i++)
                {
                    for (int j = 0; j < Board[i].Count; j++)
                    {
                        Challenge challenge = Board[i][j];
                        JObject data = JObject.Parse(stats);
                        double test = (double)data.SelectToken("playerstats.stats[?(@.name == '" + challenge.statName + "')].value");
                        player.BeginStats.Add(challenge.statName, test);
                    }
                }
                player.Update();
            }
            Open = false;
        }

        private async Task CreateBoard()
        {
            int size = (int)Math.Sqrt(Challengelist.challenges.Count());
            Board = new List<List<Challenge>>();
            Random random = new Random();
            for (int i = 0; i < size; i++)
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

        public async Task UpdateBoard()
        {
            if (Open) return;
            foreach (Player player in Players)
            {
                HttpClient client = new HttpClient();
                string stats = await player.GetStatsString(Challengelist.gameId);
                for (int i = 0; i < Board.Count(); i++)
                {
                    for (int j = 0; j < Board[i].Count; j++)
                    {
                        Challenge challenge = Board[i][j];
                        JObject data = JObject.Parse(stats);
                        double value = (double)data.SelectToken("playerstats.stats[?(@.name == '" + challenge.statName + "')].value");
                        player.Stats[challenge.statName] = value;
                        if (value >= player.BeginStats[challenge.statName] + challenge.value)
                        {
                            if (Board[i][j].Players.Find(p => p.Name == player.Name) == null)
                            {
                                Board[i][j].Players.Add(player);
                            }
                        }
                    }
                }
                CheckWinner(player);
            }
            Open = false;
            return;
        }

        public void CheckWinner(Player player)
        {
            bool winner = true;
            if (Winners.Contains(player)) return;
            //fill tempboard
            List<List<bool>> tempboard = new List<List<bool>>();
            for (int i = 0; i < Board.Count(); i++)
            {
                tempboard.Add(new List<bool> { false, false ,false });
                for(int j = 0; j < Board[i].Count; j++)
                {
                    if (Board[i][j].Players.Contains(Board[i][j].Players.Find(p => p.Name == player.Name)))
                    {
                        tempboard[i][j] = true;
                    }
                }
            }

            //check horizontal
            foreach(List<bool> row in tempboard)
            {
                if (!row.Contains(false))
                {
                    Winners.Add(player);
                    return;
                }
            }
            //check vertical
            for(int j = 0;j < tempboard.Count(); j++)
            {
                winner = true;
                for(int i = 0; i < tempboard.Count; i++)
                {
                    if(tempboard[i][j] == false){
                        winner = false;
                        break;
                    }
                }
                if (winner)
                {
                    Winners.Add(player);
                    return;
                }
            }
            //check diagonal
            winner = true;
            for (int i = 0; i < tempboard.Count(); i++)
            {
                if (tempboard[i][tempboard.Count - 1 - i] == false)
                {
                    winner = false;
                    break;
                }
            }
            if (winner == true)
            {
                Winners.Add(player);
                return;
            }
            //other diagonal
            winner = true;
            for(int i = 0; i < tempboard.Count(); i++)
            {
                if(tempboard[i][i] == false)
                {
                    winner =false;
                    break;
                }
            }
            if (winner == true)
            {
                Winners.Add(player);
                return;
            }
        }

        private async Task GetChallenges(int id)
        {
            HttpClient Client = new HttpClient();
            var challengelist = await Client.GetStringAsync("https://localhost:7219/steambingo/getchallengelist?id=" + id);
            try
            {
                Challengelist = new Challengelist();
                Challengelist = JsonSerializer.Deserialize<Challengelist>(challengelist);
            }
            catch (Exception ex)
            {
            }
            wait = false;
        }


    }
}
