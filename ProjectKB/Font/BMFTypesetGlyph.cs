using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Font
{
    public class BMFTypesetGlyph
    {
        public readonly Texture2D texture;
        public readonly Rectangle sourceRect;
        public readonly Point offset;

        public BMFTypesetGlyph(Texture2D texture, Rectangle sourceRect, Point offset)
        {
            this.texture = texture;
            this.sourceRect = sourceRect;
            this.offset = offset;
        }
    }
}
