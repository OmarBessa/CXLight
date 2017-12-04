namespace CXLight.Kits
{
    using System;
    using System.IO;

    public static class IoKit
    {
        public static void MakeEmptyFile(string filePath)
        {
            File.Create(filePath).Dispose();
        }

        public static void Write(string filePath, int writeBufferSize, Func<byte[], bool> writingFn, Action onStart = null, Action onEnd = null)
        {
            if (File.Exists(filePath)) File.Delete(filePath);

            using (var writer = new FileStream(filePath, FileMode.CreateNew))
            {
                var writeBuffer = new byte[writeBufferSize];

                onStart?.Invoke();
                while (writingFn(writeBuffer))
                {
                    writer.Write(writeBuffer, 0, writeBufferSize);
                }
                onEnd?.Invoke();
            }
        }
    }
}
