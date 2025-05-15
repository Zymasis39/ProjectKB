using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKBShared.Model
{
    public class DBScoresByPreset
    {
        public byte preset { get; set; }
        public List<DBScore> scores { get; set; }
    }
}
