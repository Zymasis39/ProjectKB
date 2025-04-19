using ProjectKB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Font
{
    public class FontInfo
    {
        public readonly short fontSize;
        public readonly byte bitField;
        public readonly byte charSet;
        public readonly ushort stretchH;
        public readonly byte aa;
        public readonly byte[] padding = new byte[4];
        public readonly byte spacingH;
        public readonly byte spacingV;
        public readonly byte outline;
        public readonly string fontName;

        public FontInfo(byte[] bytes)
        {
            fontSize = StreamUtil.Int16FromBytes(bytes, 0, true);
            bitField = bytes[2];
            charSet = bytes[3];
            stretchH = (ushort)StreamUtil.Int16FromBytes(bytes, 4, true);
            aa = bytes[6];
            Array.Copy(bytes, 7, padding, 0, 4);
            spacingH = bytes[11];
            spacingV = bytes[12];
            outline = bytes[13];
            byte[] nameAsBytes = new byte[bytes.Length - 15];
            Array.Copy(bytes, 14, nameAsBytes, 0, bytes.Length - 15);
            fontName = Encoding.UTF8.GetString(nameAsBytes);
        }
    }
}
