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
    }
}
