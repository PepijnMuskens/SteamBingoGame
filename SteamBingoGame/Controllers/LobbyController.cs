using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace SteamBingoGame.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("CorsPolicy")]
    public class LobbyController : ControllerBase
    {
        private readonly ILogger<LobbyController> _logger;

        public LobbyController(ILogger<LobbyController> logger)
        {
            _logger = logger;
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("CreateLobby")]
        public Lobby CreateLobby(int id)
        {
            Random random = new Random();
            
            Lobby lobby = new Lobby(random.Next(1000, 10000), id);
            while (Main.LobbyContainer.CheckId(lobby))
            {
                lobby = new Lobby(random.Next(1000, 10000),id);
            }
            Main.LobbyContainer.Lobbys.Add(lobby);
            return lobby;
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("GetLobby")]
        public Lobby GetLobby(int id)
        {
            return Main.LobbyContainer.Lobbys.Find(l => l.Id == id);
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("AddPlayer")]
        public Lobby AddPlayer(int lobbyid, long playerid)
        {
            Lobby lobby = Main.LobbyContainer.Lobbys.Find(l => l.Id == lobbyid);
            if (lobby == null) return null;
            lobby.AddPlayer(new Player(playerid, "bob"));
            return lobby;
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("StartGame")]
        public async Task<Lobby> StartGame(int lobbyid)
        {
            Lobby lobby = Main.LobbyContainer.Lobbys.Find(l => l.Id == lobbyid);
            if (lobby == null) return null;
            await lobby.StartGame();
            return lobby;
        }
    }
}