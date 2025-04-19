using Microsoft.Xna.Framework;
using ProjectKB.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Views
{
    public abstract class BaseView
    {
        public abstract void Update(GameTime gt);

        public abstract void OnLoadContent();

        public abstract void OnSwitch();

        public DrawLayerManager DLM;
    }
}
