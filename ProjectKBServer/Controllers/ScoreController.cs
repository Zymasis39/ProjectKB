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
        private readonly DBWrapper _db;

        static Regex pnicRegex = new(@"[^a-zA-Z0-9\-_]+", RegexOptions.IgnoreCase);

        public ScoreController(ILogger<ScoreController> logger,
            DBWrapper db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet("{preset}")]
        public ActionResult<List<DBScore>> Get(byte preset)
        {
            if (preset < 1 || preset > 3) return NotFound("404 - invalid preset ID");

            var out_ = _db.GetByPreset(preset);

            return Ok(out_);
        }

        [HttpGet]
        public ActionResult<List<DBScoresByPreset>> Get()
        {
            var out_ = _db.GetAll();

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

            _db.Post(score);

            return NoContent();
        }
    }
}
