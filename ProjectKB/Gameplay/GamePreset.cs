using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Gameplay
{
    public class GamePreset
    {
        public GamePresetID id;
        public string name;

        public Func<double, double> levelReq;
        public Func<int, double> baseGarbage;
        public Func<int, double> extraGarbage;
        public Func<double, double> garbageDelay;

        public double firstExtraGarbageTime;
        public double extraGarbageInterval;

        public double scoreDecayRate;

        public Tuple<Color, Color>[] levelColors;

        private static Tuple<Color, Color>[] STANDARD_LEVEL_COLORS = new[] {
            new Tuple<Color, Color>(
                new Color(85, 85, 85), new Color(170, 170, 170)),
            new Tuple<Color, Color>(
                new Color(34, 119, 51), new Color(85, 204, 119)),
            new Tuple<Color, Color>(
                new Color(119, 34, 51), new Color(221, 85, 85)),
            new Tuple<Color, Color>(
                new Color(136, 85, 204), new Color(68, 17, 136)),
            new Tuple<Color, Color>(
                new Color(136, 136, 136), new Color(255, 255, 85)),
            new Tuple<Color, Color>(
                new Color(85, 187, 221), new Color(17, 34, 170)),
            new Tuple<Color, Color>(
                new Color(170, 68, 34), new Color(255, 238, 221)),
            new Tuple<Color, Color>(
                new Color(34, 255, 238), new Color(153, 255, 255)),
            new Tuple<Color, Color>(
                new Color(119, 0, 119), new Color(136, 102, 221)),
            new Tuple<Color, Color>(
                new Color(51, 204, 51), new Color(51, 51, 51)),
            new Tuple<Color, Color>(
                new Color(0, 51, 102), new Color(51, 170, 255)),
            new Tuple<Color, Color>(
                new Color(51, 0, 153), new Color(221, 204, 255)),
            new Tuple<Color, Color>(
                new Color(221, 102, 0), new Color(68, 17, 17)),
            new Tuple<Color, Color>(
                new Color(221, 221, 221), new Color(102, 34, 85)),
            new Tuple<Color, Color>(
                new Color(34, 68, 136), new Color(170, 255, 51)),
            new Tuple<Color, Color>(
                new Color(187, 187, 51), new Color(0, 68, 17)),
            new Tuple<Color, Color>(
                new Color(0, 68, 17), new Color(255, 34, 34)),
            new Tuple<Color, Color>(
                new Color(221, 119, 51), new Color(255, 255, 153)),
            new Tuple<Color, Color>(
                new Color(187, 204, 221), new Color(255, 255, 170)),
            new Tuple<Color, Color>(
                new Color(255, 219, 34), new Color(170, 255, 255)),
            new Tuple<Color, Color>(
                new Color(51, 51, 51), new Color(255, 136, 17)),
        };

        private static Tuple<Color, Color>[] EXPERT_LEVEL_COLORS = new[] {
            new Tuple<Color, Color>(
                new Color(85, 85, 85), new Color(170, 170, 170)),
            new Tuple<Color, Color>(
                new Color(68, 207, 255), new Color(245, 250, 255)),
            new Tuple<Color, Color>(
                new Color(136, 238, 170), new Color(245, 255, 250)),
            new Tuple<Color, Color>(
                new Color(238, 204, 170), new Color(255, 250, 245)),
            new Tuple<Color, Color>(
                new Color(119, 34, 34), new Color(255, 255, 51)),
            new Tuple<Color, Color>(
                new Color(153, 170, 187), new Color(153, 17, 34)),
            new Tuple<Color, Color>(
                new Color(255, 136, 34), new Color(255, 255, 255)),
            new Tuple<Color, Color>(
                new Color(34, 170, 136), new Color(153, 221, 255)),
            new Tuple<Color, Color>(
                new Color(128, 255, 17), new Color(17, 17, 17)),
            new Tuple<Color, Color>(
                new Color(78, 0, 136), new Color(255, 51, 255)),
            new Tuple<Color, Color>(
                new Color(255, 204, 34), new Color(34, 34, 255)),
            new Tuple<Color, Color>(
                new Color(187, 102, 255), new Color(250, 245, 255)),
            new Tuple<Color, Color>(
                new Color(245, 250, 255), new Color(41, 46, 51)),
        };

        private static Dictionary<GamePresetID, GamePreset> list = new()
        {
            { GamePresetID.STANDARD, new()
                {
                    id = GamePresetID.STANDARD,
                    name = "STANDARD",
                    levelReq = level => {
                        // nerf for levels above 4, becomes significant at 10-ish
                        if (level > 4) return Math.Pow(level, 2) * 6 + 16;
                        else return (Math.Pow(4 + level, 3) - 64) * 0.25;
                    },
                    baseGarbage = level => (Math.Pow(10 + level, 2) - 100) * 0.0000025,
                    extraGarbage = level => Math.Pow(Math.Pow(0.00025, 2) + Math.Pow((Math.Pow(10 + level, 2) - 100) * 0.0000025, 2), 0.5) / 3,
                    garbageDelay = gr => 3 / (0.0005 + gr),
                    firstExtraGarbageTime = 150000,
                    extraGarbageInterval = 15000,
                    scoreDecayRate = 1.0 / 600000,
                    levelColors = STANDARD_LEVEL_COLORS
                }
            },
            { GamePresetID.MARATHON, new()
                {
                    id = GamePresetID.MARATHON,
                    name = "MARATHON",
                    levelReq = level => {
                        // nerf for levels above 4, becomes significant at 10-ish
                        if (level > 4) return Math.Pow(level, 2) * 24 + 64;
                        else return (Math.Pow(4 + level, 3) - 64);
                    },
                    baseGarbage = level => (Math.Pow(10 + level, 2) - 100) * 0.0000025,
                    extraGarbage = level => Math.Pow(Math.Pow(0.00025, 2) + Math.Pow((Math.Pow(10 + level, 2) - 100) * 0.0000025, 2), 0.5) / 3,
                    garbageDelay = gr => 3 / (0.0005 + gr),
                    firstExtraGarbageTime = 600000,
                    extraGarbageInterval = 60000,
                    scoreDecayRate = 1.0 / 2400000,
                    levelColors = STANDARD_LEVEL_COLORS
                }
            },
            { GamePresetID.EXPERT, new()
                {
                    id = GamePresetID.EXPERT,
                    name = "EXPERT",
                    levelReq = level => (Math.Pow(8 + level, 3) - 512),
                    baseGarbage = level => (Math.Pow(24 + level, 3) - 6000) * 0.000000125,
                    extraGarbage = level => Math.Pow(Math.Pow(0.00025, 2) + Math.Pow((Math.Pow(10 + level, 3) - 600) * 0.0000025, 2), 0.5) / 3,
                    garbageDelay = gr => 3 / (0.001 + gr),
                    firstExtraGarbageTime = 150000,
                    extraGarbageInterval = 15000,
                    scoreDecayRate = 1.0 / 600000,
                    levelColors = EXPERT_LEVEL_COLORS
                }
            },
        };

        public static GamePreset Get(GamePresetID id)
        {
            return list[id];
        }
    }
}
