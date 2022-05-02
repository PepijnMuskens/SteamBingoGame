using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace SteamBingoGame.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LobbyController : ControllerBase
    {
        private readonly ILogger<LobbyController> _logger;

        public LobbyController(ILogger<LobbyController> logger)
        {
            _logger = logger;
        }

        [EnableCors("CorsPolicy")]
        [HttpPost("CreateLobby")]
        public Lobby CreateLobby(int id)
        {
            Lobby lobby = new Lobby(id);
            while (Main.LobbyContainer.CheckId(lobby))
            {
                lobby = new Lobby(id);
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
        [HttpPost("AddPlayer")]
        public Lobby AddPlayer(int lobbyid, int playerid)
        {
            Lobby lobby = Main.LobbyContainer.Lobbys.Find(l => l.Id == lobbyid);
            if (lobby == null) return null;
            lobby.AddPlayer(new Player(playerid, "bob"));
            return lobby;
        }
    }
}