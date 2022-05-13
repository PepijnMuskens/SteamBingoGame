using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System.Text.Json;

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
                    lobby.Board = System.Text.Json.JsonSerializer.Deserialize<List<List<Challenge>>>(reader.GetString(3));
                    
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
                query = $"SELECT Player.Steamid, Player.Name, Player.Pic, Player.BeginStats FROM `Player` WHERE `LobbyId` = {id}";
                var cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Player player = new Player(reader.GetString(0), reader.GetString(1));
                    player.Pic = (byte[])reader.GetValue(2);
                    try
                    {
                        player.BeginStats = JsonConvert.DeserializeObject<Dictionary<string, double>>(reader.GetString(3));
                    }
                    catch
                    {

                    }
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
        public async Task<Lobby> AddPlayer(int lobbyid, string name, string steamid)
        {
            int id = 0;
            Lobby lobby = GetLobby(lobbyid);
            Player player = new Player(steamid,name);
            if (lobby == null) return null;
            if (lobby.AddPlayer(player) != 1) return null;
            await player.GetPic();
            try
            {
                connection.Open();
                lobby.AddPlayer(player);
                query = $"INSERT INTO `Player`(`Steamid`, `Name`, `Pic`, `LobbyId`) VALUES (@steamid, @name, @pic, @lobbyid)";
                var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("steamid", player.SteamId);
                cmd.Parameters.AddWithValue("name", player.Name);
                cmd.Parameters.AddWithValue("pic", player.Pic);
                cmd.Parameters.AddWithValue("lobbyid", lobbyid);
                cmd.ExecuteScalar();
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
            
            {
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
            }
            return player;
        }

        [EnableCors("CorsPolicy")]
        [HttpGet("StartGame")]
        public async Task<Lobby> StartGame(int lobbyid)
        {
            Lobby lobby = GetLobby(lobbyid);
            if (lobby == null) return null;
            await lobby.StartGame();
            Update(lobby);
            return lobby;
        }
        [EnableCors("CorsPolicy")]
        [HttpGet("Update")]
        public async Task<Lobby> Update(int lobbyid)
        {
            Lobby lobby = GetLobby(lobbyid);
            if (lobby == null) return null;
            await lobby.UpdateBoard();
            Update(lobby);
            return lobby;
        }

        private int Update(Lobby lobby)
        {
            try
            {
                connection.Open();
                query = $"UPDATE `Lobby` SET `Id`={lobby.Id},`Open`={lobby.Open},`Board`='{System.Text.Json.JsonSerializer.Serialize(lobby.Board)}' WHERE `Id` = {lobby.Id}";
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