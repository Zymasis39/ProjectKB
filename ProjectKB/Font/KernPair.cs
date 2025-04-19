using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Font
{
    public struct KernPair
    {
        public Rune a;
        public Rune b;

        public KernPair(Rune a, Rune b)
        {
            this.a = a;
            this.b = b;
        }
    }
}
