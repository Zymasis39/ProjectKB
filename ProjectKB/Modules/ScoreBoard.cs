using ProjectKB.Gameplay;
using ProjectKB.Utils;
using ProjectKBShared.Model;
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
        private const int version = 2;

        public Dictionary<GamePresetID, List<GameResult>> scoresLocal = new();
        public Dictionary<GamePresetID, Pair<FetchStatus, List<DBScore>>> scoresOnline = new();

        public const int N_SCORES = 10;

        private ScoreBoard()
        { 
            foreach (var val in Enum.GetValues(typeof(GamePresetID)))
            {
                scoresLocal[(GamePresetID)val] = new List<GameResult>();
                scoresOnline[(GamePresetID)val] = new(FetchStatus.Outdated, new List<DBScore>());
            }
        }

        public static ScoreBoard Load()
        {
            ScoreBoard board = new ScoreBoard();

            string fn = AppPaths.GetPath(".scores");
            if (File.Exists(fn))
            {
                byte[] data = File.ReadAllBytes(fn);
                try
                {
                    byte v = data[0];
                    int i = 1;
                    if (v == 1)
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
                            GameResult score = new(timestamp, GamePresetID.STANDARD, peakScore, peakLevel, gameTime, name);
                            board.scoresLocal[GamePresetID.STANDARD].Add(score);
                            i += 24;
                        }
                    }
                    else if (v == 2)
                    {
                        while (i < data.Length)
                        {
                            GamePresetID preset = (GamePresetID)data[i++];
                            int c = StreamUtil.Int32FromBytes(data, i, false);
                            i += 4;
                            for (int j = 0; j < c; j++)
                            {
                                DateTime timestamp = DateTime.FromBinary(StreamUtil.Int64FromBytes(data, i, false));
                                int l = StreamUtil.Int32FromBytes(data, i + 8, false);
                                string name = Encoding.UTF8.GetString(data, i + 12, l);
                                i += 12 + l;
                                double peakScore = StreamUtil.DoubleFromBytes(data, i, false);
                                double peakLevel = StreamUtil.DoubleFromBytes(data, i + 8, false);
                                double gameTime = StreamUtil.DoubleFromBytes(data, i + 16, false);
                                GameResult score = new(timestamp, preset, peakScore, peakLevel, gameTime, name);
                                board.scoresLocal[preset].Add(score);
                                i += 24;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Unrecognized scoreboard file version.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while loading scoreboard file.");
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
            List<GameResult> spp = scoresLocal[score.preset];
            i = 0;
            while (i < spp.Count && spp[i].score > score.score) i++;
            if (i == N_SCORES) return;
            if (spp.Count == N_SCORES) spp.RemoveAt(N_SCORES - 1);
            spp.Insert(i, score);
        }

        public void Save()
        {
            string fn = AppPaths.GetPath(".scores");
            if (File.Exists(fn)) File.Delete(fn);
            FileStream fs = File.OpenWrite(fn);
            fs.WriteByte(version);
            /* v1
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
            */
            // v2
            foreach (var kvp in scoresLocal)
            {
                if (kvp.Value.Count == 0) continue;
                fs.WriteByte((byte)kvp.Key);
                fs.Write(StreamUtil.Int32ToBytes(kvp.Value.Count, false));
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    GameResult score = kvp.Value[i];
                    fs.Write(StreamUtil.Int64ToBytes(score.ts.ToBinary(), false));
                    byte[] nameAsBytes = Encoding.UTF8.GetBytes(score.playerName);
                    fs.Write(StreamUtil.Int32ToBytes(nameAsBytes.Length, false));
                    fs.Write(nameAsBytes);
                    fs.Write(StreamUtil.DoubleToBytes(score.score, false));
                    fs.Write(StreamUtil.DoubleToBytes(score.level, false));
                    fs.Write(StreamUtil.DoubleToBytes(score.gameTime, false));
                }
            }
            fs.Close();
        }
    }
}
