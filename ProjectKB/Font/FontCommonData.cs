using ProjectKB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Font
{
    public class FontCommonData
    {
        public readonly ushort lineHeight;
        public readonly ushort baseline;
        public readonly ushort scaleW;
        public readonly ushort scaleH;
        public readonly ushort nPages;
        public readonly byte bitField;
        public readonly byte[] channels = new byte[4]; // ARGB

        public FontCommonData(byte[] bytes)
        {
            lineHeight = (ushort)StreamUtil.Int16FromBytes(bytes, 0, true);
            baseline = (ushort)StreamUtil.Int16FromBytes(bytes, 2, true);
            scaleW = (ushort)StreamUtil.Int16FromBytes(bytes, 4, true);
            scaleH = (ushort)StreamUtil.Int16FromBytes(bytes, 6, true);
            nPages = (ushort)StreamUtil.Int16FromBytes(bytes, 8, true);
            bitField = bytes[10];
            Array.Copy(bytes, 11, channels, 0, 4);
        }
    }
}
