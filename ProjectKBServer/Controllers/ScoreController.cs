using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjectKBShared.Model;

namespace ProjectKBServer.Controllers
{
    [ApiController]
    [Route("scores")]
    public class ScoreController : ControllerBase
    {
        private readonly ILogger<ScoreController> _logger;
        private readonly MySqlConnection _conn;

        private MySqlCommand readCmd;
        private MySqlCommand writeCmd;

        public ScoreController(ILogger<ScoreController> logger,
            MySqlConnection conn)
        {
            _logger = logger;
            _conn = conn;

            readCmd = new();
            readCmd.Connection = _conn;
            readCmd.CommandType = System.Data.CommandType.Text;
            readCmd.CommandText =
                "SELECT `id`, `version`, `preset`, `playerName`, `timestamp`, `score`, `level` FROM `scores` WHERE `preset` = @preset ORDER BY `level` DESC LIMIT 10";

            writeCmd = new();
            writeCmd.Connection = _conn;
            writeCmd.CommandType = System.Data.CommandType.Text;
            writeCmd.CommandText =
                "INSERT INTO `scores` (`version`, `preset`, `playerName`, `timestamp`, `score`, `level`) VALUES (@version, @preset, @playerName, @timestamp, @score, @level)";
        }

        [HttpGet("{preset}")]
        public string Get(byte preset)
        {
            readCmd.Parameters.AddWithValue("@preset", preset);

            string out_ = "";
            MySqlDataReader reader = readCmd.ExecuteReader();
            while (reader.Read())
            {
                out_ += $"{reader.GetUInt64(0)} {reader.GetString(1)} {reader.GetByte(2)} {reader.GetString(3)} {reader.GetDateTime(4)} {reader.GetDouble(5)} {reader.GetDouble(6)}\n";
            }

            return out_;
        }

        [HttpPost]
        public void Post(DBScore score)
        {
            writeCmd.Parameters.AddWithValue("@version", score.version);
            writeCmd.Parameters.AddWithValue("@preset", score.preset);
            writeCmd.Parameters.AddWithValue("@playerName", score.playerName);
            writeCmd.Parameters.AddWithValue("@timestamp", score.timestamp);
            writeCmd.Parameters.AddWithValue("@score", score.score);
            writeCmd.Parameters.AddWithValue("@level", score.level);

            writeCmd.ExecuteNonQuery();
        }
    }
}
