using ProjectKB.Gameplay;
using ProjectKB.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Modules
{
    public class ScoreBoard
    {
        private const int version = 1;

        public List<GameResult> scores = new();

        public const int N_SCORES = 10;

        private ScoreBoard() { }

        public static ScoreBoard Load()
        {
            ScoreBoard board = new ScoreBoard();

            string fn = AppPaths.GetPath(".scores");
            if (File.Exists(fn))
            {
                byte[] data = File.ReadAllBytes(fn);
                byte v = data[0];
                int i = 1;
                if (v == 1)
                {
                    try
                    {
                        for (int j = 0; j < N_SCORES; j++)
                        {
                            if (i == data.Length) break;
                            DateTime timestamp = DateTime.FromBinary(StreamUtil.Int64FromBytes(data, i, false));
                            int l = StreamUtil.Int32FromBytes(data, i + 8, false);
                            string name = Encoding.UTF8.GetString(data, i + 12, l);
                            i += 12 + l;
                            double peakScore = StreamUtil.DoubleFromBytes(data, i, false);
                            double peakLevel = StreamUtil.DoubleFromBytes(data, i + 8, false);
                            double gameTime = StreamUtil.DoubleFromBytes(data, i + 16, false);
                            GameResult score = new(timestamp, peakScore, peakLevel, gameTime, name);
                            board.scores.Add(score);
                            i += 24;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error while loading scoreboard file.");
                    }
                }
                else
                {
                    Console.WriteLine("Unrecognized scoreboard file version.");
                }
            }
            else
            {
                Console.WriteLine("Scoreboard file not found.");
            }
            return board;
        }

        public void AddScore(GameResult score, out int i)
        {
            i = 0;
            while (i < scores.Count && scores[i].score > score.score) i++;
            if (i == N_SCORES) return;
            if (scores.Count == N_SCORES) scores.RemoveAt(N_SCORES - 1);
            scores.Insert(i, score);
        }

        public void Save()
        {
            string fn = AppPaths.GetPath(".scores");
            if (File.Exists(fn)) File.Delete(fn);
            FileStream fs = File.OpenWrite(fn);
            fs.WriteByte(version);
            for (int i = 0; i < N_SCORES && i < scores.Count; i++)
            {
                GameResult score = scores[i];
                fs.Write(StreamUtil.Int64ToBytes(score.ts.ToBinary(), false));
                byte[] nameAsBytes = Encoding.UTF8.GetBytes(score.playerName);
                fs.Write(StreamUtil.Int32ToBytes(nameAsBytes.Length, false));
                fs.Write(nameAsBytes);
                fs.Write(StreamUtil.DoubleToBytes(score.score, false));
                fs.Write(StreamUtil.DoubleToBytes(score.level, false));
                fs.Write(StreamUtil.DoubleToBytes(score.gameTime, false));
            }
            fs.Close();
        }
    }
}
