using Microsoft.Xna.Framework.Graphics;
using ProjectKB.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Draw
{
    public class DrawLayer
    {
        public List<IKBDrawable> drawables;
        public Effect effect;
        public DrawLayerManager manager;
        public DrawLayer(Effect effect = null)
        {
            drawables = new();
            this.effect = effect;
        }

        public void RemoveFromLayer(IKBDrawable drawable)
        {
            drawables.Remove(drawable);
            drawable.layer = null;
        }

        public void ReAddToLayer(IKBDrawable drawable)
        {
            drawables.Remove(drawable);
            drawables.Add(drawable);
        }
    }
}
