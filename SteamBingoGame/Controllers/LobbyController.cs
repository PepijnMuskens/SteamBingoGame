using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace SteamBingoGame.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [EnableCors("CorsPolicy")]
    public class LobbyController : ControllerBase
    {
        private readonly ILogger<LobbyController> _logger;
        private string connectionString = "Server=am1.fcomet.com;Uid=steambin_steambin;Database=steambin_Data;Pwd=Appels1peren0";
        private MySqlConnection connection;
        private string query;

        public LobbyController(ILogger<LobbyController> logger)
        {
            _logger = logger;
            connection = new MySqlConnection(connectionString);
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("CreateLobby")]
        public Lobby CreateLobby(int id)
        {
            Lobby lobby = new Lobby(id);
            return lobby;
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("GetLobby")]
        public Lobby GetLobby(int id)
        {
            try
            {
                
                connection.Open();
                query = $"SELECT * FROM `Lobby` WHERE Id = {id}";
                var cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Lobby lobby = new Lobby(
                        reader.GetInt32(0),
                        reader.GetInt32(2),
                        reader.GetBoolean(1));
                    connection.Close();
                    return lobby;
                }
            }
            catch
            {
                
            }
            connection.Close();
            return null;
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("AddPlayer")]
        public Lobby AddPlayer(int lobbyid, long playerid)
        {
            Lobby lobby = GetLobby(lobbyid);
            Player player = new Player(0,"");
            if (lobby == null) return null;
            try
            {
                connection.Open();
                query = $"SELECT * FROM `Player` WHERE SteamId = {playerid}";
                var cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    player = new Player(
                        reader.GetInt64(0),
                        reader.GetString(1));
                }
                if (player.Name != "")
                {
                    lobby.AddPlayer(player);
                    query = $"INSERT INTO `LobbyPlayer`(`LobbyId`, `PlayerId`) VALUES ({lobby.Id},{player.SteamId})";
                    var cmd2 = new MySqlCommand(query, connection);
                    cmd2.ExecuteScalar();
                }


            }
            catch
            {

            }
            
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