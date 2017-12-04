namespace CXLight.Exts
{
    using System;

    public static class ByteArrayExt
    {       
        public static int GetPosition(this byte[] readBuffer, Func<byte, bool> predicate)
        {
            var i = 0;
            while (i < readBuffer.Length)
            {
                if (predicate(readBuffer[i])) return i;
                i++;
            }

            return -1;
        }

        public static bool ToBool(this byte[] source, int at)
        {
            return source[at] != 0;
        }

        public static int ToInt32(this byte[] source, int start)
        {
            return (source[start] & 0xFF) << 24 | 
                (source[start + 1] & 0xFF) << 16 | 
                (source[start + 2] & 0xFF) << 8 | 
                source[start + 3] & 0xFF;
        }

        public static float ToFloat(this byte[] source, int start)
        {
            return BitConverter.ToSingle(source, start);
        }

        // TODO Double check this. Write tests.
        public static uint ToUint(this byte[] bytesAsUint, int position)
        {
            return ((uint)bytesAsUint[position++] << 0) |
                   ((uint)bytesAsUint[position++] << 8) |
                   ((uint)bytesAsUint[position++] << 16) |
                   ((uint)bytesAsUint[position] << 24);
        }
    }
}