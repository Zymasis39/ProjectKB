using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Gameplay
{
    public class CornerSpawnGenerator
    {
        private Corner last = Corner.None;

        public Corner Next()
        {
            Corner out_ = Corner.None;
            if (last == Corner.None) out_ = (Corner)(1 + KBModules.GameplayRNG.Next(4));
            else out_ = (Corner)(((int)last + KBModules.GameplayRNG.Next(3)) % 4 + 1);
            last = out_;
            return out_;
        }
    }

    public enum Corner
    {
        None = 0,
        TL = 1,
        TR = 2,
        BR = 3,
        BL = 4
    }
}
