using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ProjectKBShared.Model;
using System.Data;
using System.Text.RegularExpressions;

namespace ProjectKBServer.Controllers
{
    [ApiController]
    [Route("scores")]
    public class ScoreController : ControllerBase
    {
        private readonly ILogger<ScoreController> _logger;
        private readonly MySqlConnection _conn;

        private MySqlCommand readCmd;
        private MySqlCommand readAllCmd;
        private MySqlCommand writeCmd;

        static Regex pnicRegex = new(@"[^a-zA-Z0-9\-_]+", RegexOptions.IgnoreCase);

        public ScoreController(ILogger<ScoreController> logger,
            MySqlConnection conn)
        {
            _logger = logger;
            _conn = conn;

            readCmd = new();
            readCmd.Connection = _conn;
            readCmd.CommandType = CommandType.Text;
            readCmd.CommandText =
                "SELECT t.`id`, t.`version`, t.`preset`, t.`selectionMode`, t.`playerName`, t.`timestamp`, t.`score`, t.`level` FROM (SELECT *, dense_rank() OVER (PARTITION BY playerName ORDER BY level DESC, id DESC) AS `rank` FROM `scores` WHERE `preset` = @preset) t WHERE t.`rank` = 1 ORDER BY t.`level` DESC LIMIT 10;";

            readAllCmd = new();
            readAllCmd.Connection = _conn;
            readAllCmd.CommandType = CommandType.Text;
            readAllCmd.CommandText =
                "SELECT t.`id`, t.`version`, t.`preset`, t.`selectionMode`, t.`playerName`, t.`timestamp`, t.`score`, t.`level` FROM (SELECT *, dense_rank() OVER (PARTITION BY playerName ORDER BY level DESC, id DESC) AS `rank` FROM `scores` WHERE `preset` = 1) t WHERE t.`rank` = 1 ORDER BY t.`level` DESC LIMIT 10;"
                + "SELECT t.`id`, t.`version`, t.`preset`, t.`selectionMode`, t.`playerName`, t.`timestamp`, t.`score`, t.`level` FROM (SELECT *, dense_rank() OVER (PARTITION BY playerName ORDER BY level DESC, id DESC) AS `rank` FROM `scores` WHERE `preset` = 2) t WHERE t.`rank` = 1 ORDER BY t.`level` DESC LIMIT 10;"
                + "SELECT t.`id`, t.`version`, t.`preset`, t.`selectionMode`, t.`playerName`, t.`timestamp`, t.`score`, t.`level` FROM (SELECT *, dense_rank() OVER (PARTITION BY playerName ORDER BY level DESC, id DESC) AS `rank` FROM `scores` WHERE `preset` = 3) t WHERE t.`rank` = 1 ORDER BY t.`level` DESC LIMIT 10;";

            writeCmd = new();
            writeCmd.Connection = _conn;
            writeCmd.CommandType = CommandType.Text;
            writeCmd.CommandText =
                "INSERT INTO `scores` (`version`, `preset`, `selectionMode`, `playerName`, `timestamp`, `score`, `level`) VALUES (@version, @preset, @selectionMode, @playerName, @timestamp, @score, @level);";
        }

        [HttpGet("{preset}")]
        public ActionResult<List<DBScore>> Get(byte preset)
        {
            if (preset < 1 || preset > 3) return NotFound("404 - invalid preset ID");

            _conn.Open();
            readCmd.Parameters.AddWithValue("@preset", preset);

            List<DBScore> out_ = new();
            MySqlDataReader reader = readCmd.ExecuteReader();
            while (reader.Read())
            {
                out_.Add(new DBScore()
                {
                    id = reader.GetUInt64(0),
                    version = reader.GetString(1),
                    preset = reader.GetByte(2),
                    selectionMode = reader.GetByte(3),
                    playerName = reader.GetString(4),
                    timestamp = reader.GetDateTime(5),
                    score = reader.GetDouble(6),
                    level = reader.GetDouble(7)
                });
            }
            reader.Close();
            _conn.Close();

            return Ok(out_);
        }

        [HttpGet]
        public ActionResult<List<DBScoresByPreset>> Get()
        {
            _conn.Open();
            List<DBScoresByPreset> out_ = new();
            MySqlDataReader reader = readAllCmd.ExecuteReader();
            byte preset = 1;
            while (true)
            {
                List<DBScore> outPreset = new();
                while (reader.Read())
                {
                    outPreset.Add(new DBScore()
                    {
                        id = reader.GetUInt64(0),
                        version = reader.GetString(1),
                        preset = reader.GetByte(2),
                        selectionMode = reader.GetByte(3),
                        playerName = reader.GetString(4),
                        timestamp = reader.GetDateTime(5),
                        score = reader.GetDouble(6),
                        level = reader.GetDouble(7)
                    });
                }
                out_.Add(new() { preset = preset, scores = outPreset });

                if (!reader.NextResult()) break;
                preset++;
            }
            reader.Close();
            _conn.Close();

            return Ok(out_);
        }

        [HttpPost]
        public ActionResult Post(DBScore score)
        {
            // here goes data validation
            if (score.selectionMode == null) score.selectionMode = 1;

            if (score.preset < 1 || score.preset > 3
                || score.selectionMode < 1 || score.selectionMode > 2
                || score.playerName == string.Empty
                || score.playerName.Length > 16
                || pnicRegex.IsMatch(score.playerName)
                || score.score < 0
                || score.level < 0)
                return BadRequest("400 - Invalid score data");

            score.playerName = score.playerName.ToUpperInvariant();

            _conn.Open();
            MySqlTransaction t = _conn.BeginTransaction();

            writeCmd.Parameters.AddWithValue("@version", score.version);
            writeCmd.Parameters.AddWithValue("@preset", score.preset);
            writeCmd.Parameters.AddWithValue("@selectionMode", score.selectionMode);
            writeCmd.Parameters.AddWithValue("@playerName", score.playerName);
            writeCmd.Parameters.AddWithValue("@timestamp", score.timestamp);
            writeCmd.Parameters.AddWithValue("@score", score.score);
            writeCmd.Parameters.AddWithValue("@level", score.level);

            writeCmd.ExecuteNonQuery();

            t.Commit();
            _conn.Close();

            return NoContent();
        }
    }
}
