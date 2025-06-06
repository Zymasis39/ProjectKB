﻿using Microsoft.Xna.Framework;
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
    internal class ColumnIndicator : IKBDrawable
    {
        private const int width = 32;
        private const int margin = 80;
        private bool vertical;
        private int c;

        private BMFTypesetData[] labels;
        private int labelOffset = 64;
        private float fontScale = 0.375f;

        private ColumnSelectionMode selectionMode;

        public DrawLayer layer { get; set; }

        public ColumnIndicator(bool vertical, int c, ColumnSelectionMode selectionMode, string[] labels)
        {
            this.vertical = vertical;
            this.c = c;
            this.selectionMode = selectionMode;
            this.labels = new BMFTypesetData[labels.Length];
            for (int i = 0; i < labels.Length; i++)
            {
                this.labels[i] = KBFonts.SAEADA_600_96.Typeset(labels[i]);
            }
            KBModules.ViewManager.gameplayView.DLM.AddToLayer(this, 2);
        }

        public void Draw()
        {
            Point topLeft = GameBoard.topLeft;
            float step = GameBoard.TILE_TEX_SIZE * GameBoard.scale;
            if (vertical)
            {
                KBModules.SpriteBatch.Draw(KBImages.GP_IND_VER,
                    new Vector2(topLeft.X - (width + margin) * GameBoard.scale, topLeft.Y + step * c), null, Color.Lime,
                    0f, new Vector2(0, 0), GameBoard.scale, SpriteEffects.None, 0f);
                KBModules.SpriteBatch.Draw(KBImages.GP_IND_VER,
                    new Vector2(topLeft.X + step * GameBoard.DIM + margin * GameBoard.scale, topLeft.Y + step * c), null, Color.Lime,
                    0f, new Vector2(0, 0), GameBoard.scale, SpriteEffects.None, 0f);
                float tx1 = topLeft.X - labelOffset, tx2 = topLeft.X + (GameBoard.DIMPX * GameBoard.scale) + labelOffset;
                if (selectionMode == ColumnSelectionMode.Absolute)
                {
                    for (int iy = 0; iy < GameBoard.DIM; iy++)
                    {
                        if (iy == c) continue;
                        BMFTypesetData tsd = labels[iy];
                        float txd = tsd.width * -0.5f * fontScale;
                        float ty = topLeft.Y + step * (iy + 0.5f) - tsd.height * 0.5f * fontScale;
                        tsd.Draw(tx1, ty, alignX: 0.5f, alignY: 0.5f, scale: fontScale, color: Color.Lime);
                        tsd.Draw(tx2, ty, alignX: 0.5f, alignY: 0.5f, scale: fontScale, color: Color.Lime);
                    }
                }
                else if (selectionMode == ColumnSelectionMode.Relative)
                {
                    // will figure that out...
                }
            }
            else
            {
                KBModules.SpriteBatch.Draw(KBImages.GP_IND_HOR,
                    new Vector2(topLeft.X + step * c, topLeft.Y - (width + margin) * GameBoard.scale), null, Color.Lime,
                    0f, new Vector2(0, 0), GameBoard.scale, SpriteEffects.None, 0f);
                KBModules.SpriteBatch.Draw(KBImages.GP_IND_HOR,
                    new Vector2(topLeft.X + step * c, topLeft.Y + step * GameBoard.DIM + margin * GameBoard.scale), null, Color.Lime,
                    0f, new Vector2(0, 0), GameBoard.scale, SpriteEffects.None, 0f);
                float ty1 = topLeft.Y - labelOffset, ty2 = topLeft.Y + (GameBoard.DIMPX * GameBoard.scale) + labelOffset;
                if (selectionMode == ColumnSelectionMode.Absolute)
                {
                    for (int ix = 0; ix < GameBoard.DIM; ix++)
                    {
                        if (ix == c) continue;
                        BMFTypesetData tsd = labels[ix];
                        float tyd = tsd.height * -0.5f * fontScale;
                        float tx = topLeft.X + step * (ix + 0.5f) - tsd.width * 0.5f * fontScale;
                        tsd.Draw(tx, ty1, alignX: 0.5f, alignY: 0.5f, scale: fontScale, color: Color.Lime);
                        tsd.Draw(tx, ty2, alignX: 0.5f, alignY: 0.5f, scale: fontScale, color: Color.Lime);
                    }
                }
                else if (selectionMode == ColumnSelectionMode.Relative)
                {

                }
            }
        }

        public void PrepDraw()
        {
            
        }

        public void UpdateC(int c)
        {
            this.c = c;
        }
    }
}
