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
    public class ScoreDisplayV2 : IKBDrawable
    {
        private static BMFTypesetData levelLabelTypeset;
        private BMFTypesetData levelNumTypeset;
        private BMFTypesetData gameTimeTypeset;
        private BMFTypesetData garbageAmpLabelTypeset;
        private BMFTypesetData garbageAmpTimeTypeset;

        private double levelProgress = 0;
        private double peakLevelProgress = 0;

        public static void InitTypeset()
        {
            
        }

        public ScoreDisplayV2()
        {
            levelLabelTypeset = KBFonts.SAEADA_600_96.Typeset("LEVEL");
            levelNumTypeset = KBFonts.SAEADA_600_96.Typeset("0");
            garbageAmpLabelTypeset = KBFonts.SAEADA_600_96.Typeset("GARBAGE AMP IN");
            garbageAmpTimeTypeset = KBFonts.SAEADA_600_96.Typeset("0:00");
            gameTimeTypeset = KBFonts.SAEADA_600_96.Typeset("0:00");
            KBModules.ViewManager.gameplayView.DLM.AddToLayer(this, 3);
        }

        public DrawLayer layer { get; set; }

        public void Draw()
        {
            Viewport vp = KBModules.GraphicsDeviceManager.GraphicsDevice.Viewport;

            float x = 16, y = 16;
            foreach (BMFTypesetGlyph glyph in levelLabelTypeset.glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() * 0.5f + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                    0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
            y = 64;
            foreach (BMFTypesetGlyph glyph in levelNumTypeset.glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                    0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            x = vp.Width - 16 - (int)(garbageAmpLabelTypeset.width * 0.5f);
            y = 16;
            foreach (BMFTypesetGlyph glyph in garbageAmpLabelTypeset.glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() * 0.5f + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                    0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            }
            x = vp.Width - 16 - garbageAmpTimeTypeset.width;
            y = 64;
            foreach (BMFTypesetGlyph glyph in garbageAmpTimeTypeset.glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, Color.White,
                    0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            KBModules.SpriteBatch.Draw(KBImages.WHITE1, new Rectangle(0, 128, 64, 0), Color.White);
            int barHeight = (int)(levelProgress * (vp.Height - 128));
            KBModules.SpriteBatch.Draw(KBImages.WHITE1, new Rectangle(0, vp.Height - barHeight, 48, barHeight), new Color(102, 102, 102, 0));
        }

        public void PrepDraw()
        {

        }

        public void Update(double lp, double plp, int level, double gameTime, int gas, double tuga)
        {
            string levelStr = level.ToString();
            if (levelStr != levelNumTypeset.str)
                levelNumTypeset = KBFonts.SAEADA_600_96.Typeset(levelStr);
            string timeStr = TimeStr((int)(gameTime / 1000));
            if (timeStr != levelNumTypeset.str)
                gameTimeTypeset = KBFonts.SAEADA_600_96.Typeset(timeStr);
            string galStr = (gas == 0 ? "" : (gas + "X - ")) + "GARBAGE AMP IN";
            if (galStr != garbageAmpLabelTypeset.str)
                garbageAmpLabelTypeset = KBFonts.SAEADA_600_96.Typeset(galStr);
            string gatStr = TimeStr((int)(tuga / 1000));
            if (gatStr != garbageAmpTimeTypeset.str)
                garbageAmpTimeTypeset = KBFonts.SAEADA_600_96.Typeset(gatStr);

            levelProgress = lp;
            peakLevelProgress = plp;
        }

        private string TimeStr(int tsec)
        {
            int min = tsec / 60, sec = tsec % 60;
            return string.Format("{0:0}:{1:00}", min, sec);
        }
    }
}
