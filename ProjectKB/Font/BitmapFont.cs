using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectKB.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ProjectKB.Font
{
    public class BitmapFont
    {
        private Texture2D[] pages;

        private Dictionary<Rune, BitmapFontGlyph> glyphs;

        private Dictionary<KernPair, int> kerning;

        private FontInfo info;
        private FontCommonData common;

        public BitmapFont(Stream ds, params Texture2D[] pages)
        {
            this.pages = pages;
            glyphs = new Dictionary<Rune, BitmapFontGlyph>();
            kerning = new Dictionary<KernPair, int>();
            // read descriptor file, good luck

            if (ds != null)
            {
                Debug.WriteLine("Yep, that res definitely exists.");
            }

            int br;

            {
                byte[] initBuffer = new byte[4];
                br = ds.Read(initBuffer, 0, 4);
                if (br < 4)
                {
                    throw new Exception("Unexpected end of FNT file at identifier");
                }
                bool valid = StreamUtil.MatchBytes(initBuffer, new byte[] { 0x42, 0x4d, 0x46, 3 }, 0, 0, 4);
                if (!valid)
                {
                    throw new Exception("FNT file is invalid - identifier/version mismatch");
                }
            }

            while (true)
            {
                byte[] blockDescBuffer = new byte[5];
                br = ds.Read(blockDescBuffer, 0, 5);
                if (br == 0) break;
                else if (br < 5) throw new Exception("Unexpected end of FNT file at block header");
                byte bt = blockDescBuffer[0];
                int bl = StreamUtil.Int32FromBytes(blockDescBuffer, 1, true);
                byte[] blockContentBuffer = new byte[bl];
                br = ds.Read(blockContentBuffer, 0, bl);
                if (br < bl) throw new Exception("Unexpected end of FNT file at block content");
                switch (bt)
                {
                    case 1:
                        info = new FontInfo(blockContentBuffer);
                        break;
                    case 2:
                        common = new FontCommonData(blockContentBuffer);
                        break;
                    // no need to care about the page block, we load the pages in constructor anyway
                    case 4:
                        if (bl % 20 != 0) throw new Exception("Unexpected char block size - must be a multiple of 20");
                        int nChars = bl / 20;
                        for (int i = 0; i < bl; i += 20)
                        {
                            Rune rune = new(StreamUtil.Int32FromBytes(blockContentBuffer, i, true));
                            Rectangle rect = new(
                                StreamUtil.Int16FromBytes(blockContentBuffer, i + 4, true),
                                StreamUtil.Int16FromBytes(blockContentBuffer, i + 6, true),
                                StreamUtil.Int16FromBytes(blockContentBuffer, i + 8, true),
                                StreamUtil.Int16FromBytes(blockContentBuffer, i + 10, true));
                            Point offset = new(
                                StreamUtil.Int16FromBytes(blockContentBuffer, i + 12, true),
                                StreamUtil.Int16FromBytes(blockContentBuffer, i + 14, true));
                            short xAdvance = StreamUtil.Int16FromBytes(blockContentBuffer, i + 16, true);
                            byte page = blockContentBuffer[i + 18];
                            byte channel = blockContentBuffer[i + 19];
                            BitmapFontGlyph glyph = new(page, rect, offset, xAdvance, channel);
                            glyphs.Add(rune, glyph);
                        }
                        break;
                    case 5:
                        if (bl % 10 != 0) throw new Exception("Unexpected kerning block size - must be a multiple of 10");
                        int nPairs = bl / 10;
                        for (int i = 0; i < bl; i += 10)
                        {
                            Rune a = new(StreamUtil.Int32FromBytes(blockContentBuffer, i, true)),
                                b = new(StreamUtil.Int32FromBytes(blockContentBuffer, i + 4, true));
                            short amt = StreamUtil.Int16FromBytes(blockContentBuffer, i + 8, true);
                            kerning.Add(new KernPair(a, b), amt);
                        }
                        break;
                }
            }
        }
        public BMFTypesetData Typeset(string str, Rune? lastRune = null)
        {
            List<BMFTypesetGlyph> tsGlyphs = new();

            int len = str.Length;
            int x = 0;
            int right = 0;
            for (int i = 0; i < len; i++)
            {
                Rune rune;
                char a = str[i];
                if (char.IsLowSurrogate(a)) throw new Exception("Unmatched low surrogate");
                if (char.IsHighSurrogate(a))
                {
                    i++;
                    if (i == len) throw new Exception("Unexpected end of string after high surrogate");
                    char b = str[i];
                    if (!char.IsLowSurrogate(a)) throw new Exception("Unmatched high surrogate");
                    rune = new(a, b);
                }
                else rune = new(a);

                if (lastRune.HasValue)
                {
                    if (kerning.TryGetValue(new KernPair(lastRune.Value, rune), out int dx)) x += dx;
                }

                if (!glyphs.ContainsKey(rune)) rune = new('#');
                BitmapFontGlyph cg = glyphs[rune];

                tsGlyphs.Add(new(pages[cg.page], cg.rect,
                    new Point(x + cg.offset.X, cg.offset.Y)));
                right = Math.Max(right, x + cg.offset.X + cg.rect.Width);
                x += cg.xAdvance;
                lastRune = rune;
            }
            return new BMFTypesetData(str, tsGlyphs, info.fontSize, right, x, lastRune);
        }
    }
}
