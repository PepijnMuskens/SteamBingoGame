using System.Text.Json;

namespace SteamBingoGame
{
    public class Lobby
    {
        public int Id { get;private set; }
        public bool Open { get; private set; }
        public List<Player> Players { get; private set; }
        public int ChallengeListId { get; private set; }
        public Challengelist Challengelist { get; private set; }

        private bool wait = true;
        public Lobby(int id)
        {
            wait = true;
            GetChallenges(id);
            Random random = new Random();
            Id = random.Next(1000, 10000);
            Players = new List<Player>();
            Open = true;
            ChallengeListId = id;
            while (wait)
            {

            }
            if(Challengelist.name == null)Challengelist = new Challengelist();
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
            if(player.CheckSteamid()) return -3;
            Players.Add(player);
            return 1;
        }

        public async void StartGame()
        {
            if(!Open) return;
            foreach(Player player in Players)
            {

                foreach(Challenge challenge in Challengelist.challenges)
                {

                }
                
            }
            Open = false;
        }


        private async void GetChallenges(int id)
        {
            HttpClient Client = new HttpClient();
            var challengelist = await Client.GetStringAsync("https://i437675.luna.fhict.nl/steambingo/getchallengelist?id=" + id);
            try
            {
                Challengelist = JsonSerializer.Deserialize<Challengelist>(challengelist);
            }
            catch(Exception ex)
            {
            }
            wait = false;
        }


    }
}
