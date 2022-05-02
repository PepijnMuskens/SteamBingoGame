using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace SteamBingoGame.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LobbyController : ControllerBase
    {
        LobbyContainer LobbyContainer = new LobbyContainer();
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
            while (LobbyContainer.CheckId(lobby))
            {
                lobby = new Lobby(id);
            }
            LobbyContainer.Lobbys.Add(lobby);
            return lobby;
        }
        [EnableCors("CorsPolicy")]
        [HttpGet("GetLobby")]
        public Lobby GetLobby(int id)
        {
            return LobbyContainer.Lobbys.Find(l => l.Id == id);
        }
    }
}