namespace SteamBingoGame
{
    public class LobbyContainer
    {
        public List<Lobby> Lobbys { get; set; }

        public LobbyContainer()
        {
            Lobbys = new List<Lobby>();
        }

        /// <summary>
        /// checks of lobby id exists
        /// </summary>
        /// <param name="lobby"></param>
        /// <returns>returns true if lobby id already exists</returns>
        public bool CheckId(Lobby lobby)
        {
            Lobby temp = Lobbys.Find(x => x.Id == lobby.Id);
            if(temp == null)return false;
            return true;
        }
    }
}
