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
    public class GarbageSpawn : IKBDrawable
    {
        public DrawLayer layer { get; set; }

        public readonly int x, y;
        private double rem, delay;

        public GarbageSpawn(int x, int y, double delay)
        {
            this.x = x;
            this.y = y;
            this.delay = delay;
            rem = delay;
            KBModules.ViewManager.gameplayView.DLM.AddToLayer(this, 3);
        }

        public bool TickDown(double time)
        {
            rem -= time;
            return rem <= 0;
        }

        public void Draw()
        {
            float fac = 1 - (float)(rem / delay);
            Point topLeft = GameBoard.topLeft;
            float step = GameBoard.TILE_TEX_SIZE * GameBoard.scale;

            KBModules.SpriteBatch.Draw(KBImages.GP_IND_GARBAGE, new Vector2(topLeft.X + step * x, topLeft.Y + step * y), null,
                new Color(255, 51, 51, 0) * fac,
                0f, new Vector2(0, 0), GameBoard.scale, SpriteEffects.None, 0f);
        }

        public void PrepDraw()
        {

        }
    }
}
