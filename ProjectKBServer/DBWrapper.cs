using MySql.Data.MySqlClient;
using ProjectKBShared.Model;
using System.Data;

namespace ProjectKBServer
{
    public class DBWrapper
    {
        private MySqlConnection _conn;

        public DBWrapper(string connectionString)
        {
            _conn = new MySqlConnection(connectionString);
        }

        public List<DBScore> GetByPreset(byte preset)
        {
            _conn.Open();

            MySqlCommand cmd = new();
            cmd.Connection = _conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                "SELECT t.`id`, t.`version`, t.`preset`, t.`selectionMode`, t.`playerName`, t.`timestamp`, t.`score`, t.`level` FROM (SELECT *, dense_rank() OVER (PARTITION BY playerName ORDER BY level DESC, id DESC) AS `rank` FROM `scores` WHERE `preset` = @preset) t WHERE t.`rank` = 1 ORDER BY t.`level` DESC LIMIT 10;";
            cmd.Parameters.AddWithValue("@preset", preset);

            List<DBScore> out_ = new();
            MySqlDataReader reader = cmd.ExecuteReader();
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

            return out_;
        }

        public List<DBScoresByPreset> GetAll()
        {
            _conn.Open();

            MySqlCommand cmd = new();
            cmd = new();
            cmd.Connection = _conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                "SELECT t.`id`, t.`version`, t.`preset`, t.`selectionMode`, t.`playerName`, t.`timestamp`, t.`score`, t.`level` FROM (SELECT *, dense_rank() OVER (PARTITION BY playerName ORDER BY level DESC, id DESC) AS `rank` FROM `scores` WHERE `preset` = 1) t WHERE t.`rank` = 1 ORDER BY t.`level` DESC LIMIT 10;"
                + "SELECT t.`id`, t.`version`, t.`preset`, t.`selectionMode`, t.`playerName`, t.`timestamp`, t.`score`, t.`level` FROM (SELECT *, dense_rank() OVER (PARTITION BY playerName ORDER BY level DESC, id DESC) AS `rank` FROM `scores` WHERE `preset` = 2) t WHERE t.`rank` = 1 ORDER BY t.`level` DESC LIMIT 10;"
                + "SELECT t.`id`, t.`version`, t.`preset`, t.`selectionMode`, t.`playerName`, t.`timestamp`, t.`score`, t.`level` FROM (SELECT *, dense_rank() OVER (PARTITION BY playerName ORDER BY level DESC, id DESC) AS `rank` FROM `scores` WHERE `preset` = 3) t WHERE t.`rank` = 1 ORDER BY t.`level` DESC LIMIT 10;";

            List<DBScoresByPreset> out_ = new();
            MySqlDataReader reader = cmd.ExecuteReader();
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
            return out_;
        }

        public void Post(DBScore score)
        {
            _conn.Open();
            MySqlTransaction t = _conn.BeginTransaction();

            MySqlCommand cmd = new();
            cmd.Connection = _conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
                "INSERT INTO `scores` (`version`, `preset`, `selectionMode`, `playerName`, `timestamp`, `score`, `level`) VALUES (@version, @preset, @selectionMode, @playerName, @timestamp, @score, @level);";

            cmd.Parameters.AddWithValue("@version", score.version);
            cmd.Parameters.AddWithValue("@preset", score.preset);
            cmd.Parameters.AddWithValue("@selectionMode", score.selectionMode);
            cmd.Parameters.AddWithValue("@playerName", score.playerName);
            cmd.Parameters.AddWithValue("@timestamp", score.timestamp);
            cmd.Parameters.AddWithValue("@score", score.score);
            cmd.Parameters.AddWithValue("@level", score.level);

            cmd.ExecuteNonQuery();

            t.Commit();
            _conn.Close();
        }
    }
}
