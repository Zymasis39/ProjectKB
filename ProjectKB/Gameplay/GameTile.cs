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
    public class GameTile : IKBDrawable
    {
        public DrawLayer layer { get; set; }

        public int tier = 1;
        public const int MAX_TIER = 6;
        public int x = 0, y = 0;

        private static Texture2D[] textures;

        private double scaleAnim = 0;
        private const double scaleAnimRate = 0.004;
        private double visualX, visualY;
        private const double moveRate = 0.1;

        private Color baseColor = new Color(255, 255, 255, 204);

        public static void LoadTextures()
        {
            textures = new Texture2D[]
            {
                KBImages.GP_TILEG, // garbage
                KBImages.GP_TILE1,
                KBImages.GP_TILE2,
                KBImages.GP_TILE3,
                KBImages.GP_TILE4,
                KBImages.GP_TILE5,
                KBImages.GP_TILE6
            };
        }

        public GameTile(GameBoard board, int x, int y)
        {
            this.x = x;
            this.y = y;
            visualX = x;
            visualY = y;
            KBModules.ViewManager.gameplayView.DLM.AddToLayer(this, 1);
        }

        public void PrepDraw() { }

        public void Draw()
        {
            Texture2D texture = textures[tier];
            Point topLeft = GameBoard.topLeft;
            float step = GameBoard.TILE_TEX_SIZE * GameBoard.scale;

            float scale = (float)(1 + Math.Pow(scaleAnim, 2) / 4);

            KBModules.SpriteBatch.Draw(texture,
                new Vector2(topLeft.X + step * ((float)visualX + 0.5f), topLeft.Y + step * ((float)visualY + 0.5f)), null, baseColor,
                0f, new Vector2(GameBoard.TILE_TEX_SIZE, GameBoard.TILE_TEX_SIZE) / 2, GameBoard.scale * scale, SpriteEffects.None, 0f);
        }

        public void OnRemove()
        {
            layer.RemoveFromLayer(this);
        }

        public void OnMerge()
        {
            layer.ReAddToLayer(this);
            scaleAnim = 1;
        }

        public void UpdateAnim( double delta)
        {
            scaleAnim = Math.Max(scaleAnim - delta * scaleAnimRate, 0);
            if (visualX < x) visualX = Math.Min(visualX + delta * moveRate, x);
            else if (visualX > x) visualX = Math.Max(visualX - delta * moveRate, x);
            if (visualY < y) visualY = Math.Min(visualY + delta * moveRate, y);
            else if (visualY > y) visualY = Math.Max(visualY - delta * moveRate, y);
        }
    }
}
