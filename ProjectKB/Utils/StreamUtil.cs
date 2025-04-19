using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectKB.Utils
{
    public static class StreamUtil
    {
        public static long Int64FromBytes(byte[] bytes, int index, bool le)
        {
            byte[] cut = bytes[index..(index + 8)];
            if (le != BitConverter.IsLittleEndian) Array.Reverse(cut);
            return BitConverter.ToInt64(cut);
        }

        public static int Int32FromBytes(byte[] bytes, int index, bool le)
        {
            byte[] cut = bytes[index..(index + 4)];
            if (le != BitConverter.IsLittleEndian) Array.Reverse(cut);
            return BitConverter.ToInt32(cut);
        }

        public static short Int16FromBytes(byte[] bytes, int index, bool le)
        {
            byte[] cut = bytes[index..(index + 2)];
            if (le != BitConverter.IsLittleEndian) Array.Reverse(cut);
            return BitConverter.ToInt16(cut);
        }

        public static double DoubleFromBytes(byte[] bytes, int index, bool le)
        {
            byte[] cut = bytes[index..(index + 8)];
            if (le != BitConverter.IsLittleEndian) Array.Reverse(cut);
            return BitConverter.ToDouble(cut);
        }

        public static byte[] Int32ToBytes(int v, bool le)
        {
            byte[] a = BitConverter.GetBytes(v);
            if (le != BitConverter.IsLittleEndian) Array.Reverse(a);
            return a;
        }

        public static byte[] Int64ToBytes(long v, bool le)
        {
            byte[] a = BitConverter.GetBytes(v);
            if (le != BitConverter.IsLittleEndian) Array.Reverse(a);
            return a;
        }

        public static byte[] DoubleToBytes(double v, bool le)
        {
            byte[] a = BitConverter.GetBytes(v);
            if (le != BitConverter.IsLittleEndian) Array.Reverse(a);
            return a;
        }

        public static bool MatchBytes(byte[] bytesA, byte[] bytesB, int indexA, int indexB, int count)
        {
            if (indexA + count > bytesA.Length || indexB + count > bytesB.Length) return false;

            for (int i = 0; i < count; i++)
            {
                if (bytesA[indexA + i] != bytesB[indexB + i]) return false;
            }
            return true;
        }
    }
}
