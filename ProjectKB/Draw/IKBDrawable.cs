using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Draw
{
    public interface IKBDrawable
    {
        public DrawLayer layer { get; set; }

        public void PrepDraw();
        public void Draw();
    }
}
