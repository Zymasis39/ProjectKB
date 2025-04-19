using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectKB.Content;
using ProjectKB.Draw;
using ProjectKB.Font;
using ProjectKB.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Gameplay
{
    public class ScoreDisplay : IKBDrawable
    {
        private BMFTypesetData scoreTypeset;
        private static BMFTypesetData levelLabelTypeset;
        private BMFTypesetData levelNumTypeset;
        private BMFTypesetData timeTypeset;

        public static void InitTypeset()
        {
            levelLabelTypeset = KBFonts.SAEADA_600_96.Typeset("LEVEL ");
        }

        public ScoreDisplay()
        {
            scoreTypeset = KBFonts.SAEADA_600_96.Typeset("0.000 / 0.000");
            levelNumTypeset = KBFonts.SAEADA_600_96.Typeset("0", new Rune(' '));
            timeTypeset = KBFonts.SAEADA_600_96.Typeset("0:00 / 0:00");
            KBModules.ViewManager.gameplayView.DLM.AddToLayer(this, 3);
        }

        public DrawLayer layer { get; set; }

        public void Draw()
        {
            float x = 32, y = 32;
            foreach (BMFTypesetGlyph glyph in scoreTypeset.glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() * 0.5f + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                    0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
            y = 80;
            foreach (BMFTypesetGlyph glyph in levelLabelTypeset.glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() * 0.5f + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                    0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
            x += levelLabelTypeset.nextX * 0.5f;
            foreach (BMFTypesetGlyph glyph in levelNumTypeset.glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() * 0.5f + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                    0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
            x = 32;
            y = 128;
            foreach (BMFTypesetGlyph glyph in timeTypeset.glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() * 0.5f + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                    0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
        }

        public void PrepDraw()
        {
            
        }

        public void UpdateTypeset(double score, double peakScore, int level, double gameTime, double levelTime)
        {
            string scoreStr = string.Format("{0:N3}", Math.Truncate(score * 1000) / 1000)
                + " / " + string.Format("{0:N3}", Math.Truncate(peakScore * 1000) / 1000);
            if (scoreStr != scoreTypeset.str)
                scoreTypeset = KBFonts.SAEADA_600_96.Typeset(scoreStr);
            if (level.ToString() != levelNumTypeset.str) 
                levelNumTypeset = KBFonts.SAEADA_600_96.Typeset(level.ToString());
            string timeStr = TimeStr((int)(gameTime / 1000)) + " / " + TimeStr((int)(levelTime / 1000));
            if (timeStr != levelNumTypeset.str)
                timeTypeset = KBFonts.SAEADA_600_96.Typeset(timeStr);
        }

        private string TimeStr(int tsec)
        {
            int min = tsec / 60, sec = tsec % 60;
            return string.Format("{0:0}:{1:00}", min, sec);
        }
    }
}
