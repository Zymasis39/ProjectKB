using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Font
{
    public class BMFTypesetData
    {
        public readonly string str;
        public readonly List<BMFTypesetGlyph> glyphs;
        public readonly int height;
        public readonly int width;
        public readonly int nextX;
        public readonly Rune? lastRune;

        public BMFTypesetData(string str, List<BMFTypesetGlyph> glyphs, int height, int right, int nextX, Rune? lastRune)
        {
            this.str = str;
            this.glyphs = glyphs;
            this.height = height;
            this.width = right;
            this.nextX = nextX;
            this.lastRune = lastRune;
        }

        public void Draw(float x, float y,
                         float viewportX = 0, float viewportY = 0,
                         float alignX = 0, float alignY = 0,
                         float scale = 1,
                         Color color = new Color())
        {
            Viewport vp = KBModules.GraphicsDeviceManager.GraphicsDevice.Viewport;
            x += (int)(vp.Width * viewportX - width * scale * alignX);
            y += (int)(vp.Height * viewportY - height * scale * alignY);
            foreach (BMFTypesetGlyph glyph in glyphs)
            {
                Vector2 position = glyph.offset.ToVector2() * scale + new Vector2(x, y);
                KBModules.SpriteBatch.Draw(glyph.texture, position, glyph.sourceRect, color,
                    0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
