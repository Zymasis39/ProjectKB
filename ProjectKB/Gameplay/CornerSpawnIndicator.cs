using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectKB.Content;
using ProjectKB.Draw;
using ProjectKB.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Gameplay
{
    public class CornerSpawnIndicator : IKBDrawable
    {
        public DrawLayer layer { get; set; }

        private Corner[] sqa;

        private int size;

        private const int margin = 32;
        private const int gap = 48;
        private const int gapBig = 64;

        public CornerSpawnIndicator(int size)
        {
            this.size = size;
            sqa = new Corner[size];
            KBModules.ViewManager.gameplayView.DLM.AddToLayer(this, 3);
        }

        public void Advance(Corner corner)
        {
            for (int i = 0; i < size - 1; i++) sqa[i] = sqa[i + 1];
            sqa[^1] = corner;
        }

        public void Draw()
        {
            Vector2 tl = GameBoard.topLeft.ToVector2();
            DrawCorner(tl + new Vector2(-margin, -margin) * GameBoard.scale, 1, 1, Corner.TL);
            DrawCorner(tl + new Vector2(margin + GameBoard.DIMPX, -margin) * GameBoard.scale, -1, 1, Corner.TR);
            DrawCorner(tl + new Vector2(-margin, margin + GameBoard.DIMPX) * GameBoard.scale, 1, -1, Corner.BL);
            DrawCorner(tl + new Vector2(margin + GameBoard.DIMPX, margin + GameBoard.DIMPX) * GameBoard.scale, -1, -1, Corner.BR);
        }

        private void DrawCorner(Vector2 initial, int xm, int ym, Corner match)
        {
            float sf = GameBoard.scale;
            float cm = sqa[0] == match ? 1f : 0.2f;
            KBModules.SpriteBatch.Draw(KBImages.GP_IND_SPAWN_BIG, initial, null, new Color(255, 255, 255, 0) * cm,
                0f, new Vector2(32, 32), sf, SpriteEffects.None, 0f);
            for (int i = 0; i < size - 1; i++)
            {
                cm = sqa[i + 1] == match ? 1f : 0.2f;
                KBModules.SpriteBatch.Draw(KBImages.GP_IND_SPAWN,
                    initial + new Vector2((gapBig + gap * i) * sf * xm, 0), null, new Color(255, 255, 255, 0) * cm,
                0f, new Vector2(16, 16), sf, SpriteEffects.None, 0f);
                KBModules.SpriteBatch.Draw(KBImages.GP_IND_SPAWN,
                    initial + new Vector2(0, (gapBig + gap * i) * sf * ym), null, new Color(255, 255, 255, 0) * cm,
                0f, new Vector2(16, 16), sf, SpriteEffects.None, 0f);
            }
        }

        public void PrepDraw()
        {
            
        }
    }
}
