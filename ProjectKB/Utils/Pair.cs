using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Utils
{
    public class Pair<T1, T2>
    {
        public T1 a;
        public T2 b;

        public Pair(T1 a, T2 b)
        {
            this.a = a;
            this.b = b;
        }
    }
}
