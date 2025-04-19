using ProjectKB.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Gameplay
{
    public class GameResult
    {
        public readonly DateTime ts;
        public readonly double score;
        public readonly double level;
        public readonly double gameTime;
        public readonly string playerName;

        public GameResult(double score, double level, double gameTime, string playerName) : this(DateTime.Now, score, level, gameTime, playerName) { }

        public GameResult(DateTime ts, double score, double level, double gameTime, string playerName)
        {
            this.ts = ts;
            this.score = score;
            this.level = level;
            this.gameTime = gameTime;
            this.playerName = playerName;
        }

        public void SavePlaintextFile()
        {
            List<string> lines = new()
            {
                "Timestamp (UTC): " + ts.ToString(),
                "Points: " + score,
                "Level (as float): " + level,
                "Duration (millis): " + (long)gameTime
            };

            string fn = AppPaths.GetPath(ts.Ticks.ToString("X") + ".kbgr");
            if (File.Exists(fn)) File.Delete(fn);
            FileStream fs = File.OpenWrite(fn);
            foreach (string line in lines)
            {
                fs.Write(Encoding.UTF8.GetBytes(line));
                fs.WriteByte(0xD);
                fs.WriteByte(0xA);
            }
            fs.Close();
        }
    }
}
