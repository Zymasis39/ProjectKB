using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectKB.Content;
using ProjectKB.Draw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Gameplay
{
    public class TestElement : IKBDrawable
    {
        public DrawLayer layer { get; set; }

        public static RenderTarget2D renderTarget;

        public TestElement()
        {
            renderTarget = new RenderTarget2D(KBModules.GraphicsDeviceManager.GraphicsDevice, 128 * 7, 128);
        }

        public void PrepDraw()
        {
            Effect effect = KBEffects.ARROWS;

            SpriteBatch sb = KBModules.SpriteBatch;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, 128 * 7, 128, 0, 0, 1);
            effect.Parameters["view_projection"].SetValue(projection);
            effect.Parameters["xmult"].SetValue(3.5f);
            // effect.Parameters["offset"].SetValue(0f); // to be updated dynamically
            effect.Parameters["base_color"].SetValue(new Vector4(0f, 0.5f, 1f, 0f));

            KBModules.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(renderTarget);
            sb.Begin(effect: effect);
            sb.Draw(KBImages.NULL1, new Rectangle(0, 0, 896, 128), Color.White);
            sb.End();
        }

        public void Draw()
        {
            SpriteBatch sb = KBModules.SpriteBatch;

            sb.Draw(renderTarget, new Vector2(32, 32), new Color(255, 255, 255, 0));
        }
    }
}
