using ProjectKB.Gameplay;
using ProjectKBShared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ProjectKB.Modules
{
    public class ScoreApi
    {
        private const string DEFAULT_SERVER = "http://3.79.234.6:8080";
        private string server;

        private HttpClient _client = new();

        public ScoreApi()
        {
            // this is why we init this after config!!
            server = KBModules.Config.server;
            if (server == "DEFAULT") server = DEFAULT_SERVER;
        }

        public async Task<List<DBScore>> GetScoresByPreset(GamePresetID preset)
        {
            HttpRequestMessage req = new()
            {
                RequestUri = new Uri($"{server}/scores/{(byte)preset}"),
                Method = HttpMethod.Get
            };
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage res = await _client.SendAsync(req);
            if (!res.IsSuccessStatusCode) throw new Exception($"API request returned code {res.StatusCode}");
            string json = await res.Content.ReadAsStringAsync();
            List<DBScore> scores = JsonConvert.DeserializeObject<List<DBScore>>(json);
            return scores;
        }

        public async Task<List<DBScoresByPreset>> GetScores()
        {
            HttpRequestMessage req = new()
            {
                RequestUri = new Uri($"{server}/scores"),
                Method = HttpMethod.Get
            };
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage res = await _client.SendAsync(req);
            if (!res.IsSuccessStatusCode) throw new Exception($"API request returned code {res.StatusCode}");
            string json = await res.Content.ReadAsStringAsync();
            List<DBScoresByPreset> scores = JsonConvert.DeserializeObject<List<DBScoresByPreset>>(json);
            return scores;
        }

        public async Task SubmitScore(DBScore score)
        {
            HttpRequestMessage req = new()
            {
                RequestUri = new Uri($"{server}/scores"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(score)),
            };
            req.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");
            HttpResponseMessage res = await _client.SendAsync(req);
            if (!res.IsSuccessStatusCode) throw new Exception($"API request returned code {res.StatusCode}");
        }

        public async Task SubmitScore(GameResult score)
        {
            await SubmitScore(new DBScore()
            {
                id = 0,
                version = KBModules.VERSION,
                preset = (byte)score.preset,
                playerName = score.playerName,
                timestamp = score.ts,
                score = score.score,
                level = score.level
            });
        }
    }
}
