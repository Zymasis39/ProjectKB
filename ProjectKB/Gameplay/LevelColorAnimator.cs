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

        private static Tuple<Color, Color>[] levelColors;

        private static readonly Tuple<Color, Color> gameOverColors = new(
            new Color(17, 17, 17), Color.White);

        public static void InitWithColors(Tuple<Color, Color>[] colors)
        {
            primary = Color.Black;
            secondary = Color.Black;
            lastPrimary = Color.Black;
            lastSecondary = Color.Black;
            levelColors = colors;
        }

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
