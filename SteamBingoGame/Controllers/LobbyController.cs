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
        private string connectionString = "Server=am1.fcomet.com;Uid=steambin_steambin;Database=steambin_Data;Pwd=Appels1peren0";
        private MySqlConnection connection;
        private string query;

        public LobbyController()
        {
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
            Lobby lobby = new Lobby(0, 0, false);
            try
            {
                
                connection.Open();
                query = $"SELECT * FROM `Lobby` WHERE Id = {id}";
                var cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lobby = new Lobby(
                        reader.GetInt32(0),
                        reader.GetInt32(2),
                        reader.GetBoolean(1));
                    
                }
                connection.Close();
                lobby.Players = Getplayers(id);
            }
            catch
            {
                
            }
            connection.Close();
            return lobby;
        }

        private List<Player> Getplayers(int id)
        {
            List<Player> players = new List<Player>();
            try
            {
                connection.Open();
                query = $"SELECT Player.Steamid, Player.Name, Player.Pic FROM `LobbyPlayer` INNER JOIN Player ON LobbyPlayer.PlayerId = Player.Id WHERE `LobbyId` = {id}";
                var cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Player player = new Player(reader.GetString(0), reader.GetString(1));
                    player.Pic = reader.GetString(2);
                    players.Add(player);
                }
            }
            catch
            {
                
            }
            connection.Close();
            return players;
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("AddPlayer")]
        public Lobby AddPlayer(int lobbyid, long playerid)
        {
            Lobby lobby = GetLobby(lobbyid);
            Player player = new Player("","");
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
                        reader.GetString(0),
                        reader.GetString(1));
                }
                if (player.Name != "")
                {
                    lobby.AddPlayer(player);
                    query = $"INSERT INTO `LobbyPlayer`(`LobbyId`, `PlayerId`) VALUES ({lobby.Id},{player.SteamId})";
                    connection.Close();
                    connection.Open();
                    var cmd2 = new MySqlCommand(query, connection);
                    cmd2.ExecuteScalar();
                }


            }
            catch
            {

            }
            connection.Close();
            return lobby;
        }
        [EnableCors("CorsPolicy")]
        [HttpPost("CreatePlayer")]
        public async Task<Player> CreatePlayer(string name, string steamid)
        {
            Player player = new Player(steamid, name);
            if(await player.CheckSteamid()) await player.GetPic();

            try
            {
                connection.Open();
                query = $"INSERT INTO `Player`(`Steamid`, `Name`, `Pic`) VALUES (@steamid, @name, @pic)";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("steamid", player.SteamId);
                cmd.Parameters.AddWithValue("name", player.Name);
                cmd.Parameters.AddWithValue("pic", player.Pic);
                cmd.ExecuteScalar();
            }
            catch
            {

            }
            connection.Close();
            return player;
            
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