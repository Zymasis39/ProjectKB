using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKBShared.Model
{
    public class DBScore
    {
        public ulong id { get; set; }
        public string version { get; set; }
        public byte preset { get; set; }
        public byte? selectionMode { get; set; }
        public string playerName { get; set; }
        public DateTime timestamp { get; set; }
        public double score { get; set; }
        public double level { get; set; }
    }
}
