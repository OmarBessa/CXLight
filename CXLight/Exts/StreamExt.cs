namespace CXLight.Exts
{
    using System.IO;

    public static class StreamExt
    {
        public static long SetPositionFast(this Stream stream, long newPosition)
        {
            if (stream.CanSeek)
                return newPosition == 0 ? stream.Seek(0, SeekOrigin.Begin) : stream.Seek(newPosition, SeekOrigin.Current);

            stream.Position = newPosition;
            return newPosition;
        }

        public static int AtomicRead(this Stream stream, byte[] buffer, int offset, int amount)
        {
            var prevPosition = stream.Position;
            var result = stream.Read(buffer, offset, amount);
            SetPositionFast(stream, prevPosition);

            return result;
        }
    }
}
