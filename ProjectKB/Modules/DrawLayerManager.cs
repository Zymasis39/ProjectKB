using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectKB.Draw;
using ProjectKB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Modules
{
    public class DrawLayerManager
    {
        readonly int nLayers;
        DrawLayer[] layers;

        /* list of layers:
         * 0 fixed gameplay elements (board)
         * 1 gameplay blocks
         * 2 additive effects
         */

        public DrawLayerManager(params DrawLayer[] layers)
        {
            nLayers = layers.Length;
            this.layers = layers;
            for (int i = 0; i < nLayers; i++)
            {
                if (this.layers[i].manager != null)
                {
                    throw new Exception("The same layer cannot be used in multiple layer managers");
                }
                this.layers[i].manager = this;
            }
        }

        public void AddToLayer(IKBDrawable drawable, int i)
        {
            layers[i].drawables.Add(drawable);
            drawable.layer = layers[i];
        }

        public void RemoveFromLayer(IKBDrawable drawable)
        {
            drawable.layer.drawables.Remove(drawable);
            drawable.layer = null;
        }

        public void SetLayerEffect(int i, Effect effect)
        {
            layers[i].effect = effect;
        }

        public void Draw()
        {
            for (int i = 0; i < nLayers; i++)
            {
                for (int j = 0; j < layers[i].drawables.Count; j++)
                {
                    layers[i].drawables[j].PrepDraw();
                }
            }
            KBModules.GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);
            KBModules.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

            for (int i = 0; i < nLayers; i++)
            {
                KBModules.SpriteBatch.Begin(effect: layers[i].effect);
                for (int j = 0; j < layers[i].drawables.Count; j++)
                {
                    layers[i].drawables[j].Draw();
                }
                KBModules.SpriteBatch.End();
            }
        }
    }
}
