using Microsoft.Xna.Framework;
using ProjectKB.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Gameplay
{
    public static class LevelColorAnimator
    {
        private static Color primary = Color.Black;
        private static Color secondary = Color.Black;
        private const double transitionTime = 1000;
        private static float transitionProgress = 0;
        private static int lastLevel = -1;
        private static Color lastPrimary = Color.Black;
        private static Color lastSecondary = Color.Black;

        public const int GAME_OVER = int.MaxValue;

        private static Tuple<Color, Color>[] levelColors = new Tuple<Color, Color>[]
        {
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

        private static readonly Tuple<Color, Color> gameOverColors = new(
            new Color(17, 17, 17), Color.White);

        public static void UpdateToLevel(int level, double delta)
        {
            if (level == lastLevel)
            {
                if (transitionProgress == 1) return;
                transitionProgress = (float)Math.Min(transitionProgress + delta / transitionTime, 1);
            }
            else
            {
                int li = Math.Min(level, levelColors.Length - 1);
                lastPrimary = Color.Lerp(lastPrimary, primary, transitionProgress);
                lastSecondary = Color.Lerp(lastSecondary, secondary, transitionProgress);
                Tuple<Color, Color> colors = level == GAME_OVER ? gameOverColors : levelColors[li];
                primary = colors.Item1;
                secondary = colors.Item2;
                transitionProgress = (float)(delta / transitionTime);
                lastLevel = level;
            }
            Color a = Color.Lerp(lastPrimary, primary, transitionProgress),
                b = Color.Lerp(lastSecondary, secondary, transitionProgress);

            KBEffects.RECOLOR.Parameters["rep_color_r"].SetValue(a.ToVector4());
            KBEffects.RECOLOR.Parameters["rep_color_g"].SetValue(b.ToVector4());
        }
    }
}
