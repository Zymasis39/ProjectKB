using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Font
{
    public class BitmapFontGlyph
    {
        public readonly byte page;
        public readonly Rectangle rect;
        public readonly Point offset;
        public readonly short xAdvance;
        public readonly byte channel;

        public BitmapFontGlyph(byte page, Rectangle rect, Point offset, short xAdvance, byte channel)
        {
            this.page = page;
            this.rect = rect;
            this.offset = offset;
            this.xAdvance = xAdvance;
            this.channel = channel;
        }
    }
}
