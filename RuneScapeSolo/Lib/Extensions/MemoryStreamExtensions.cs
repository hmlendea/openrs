using System.IO;

namespace RuneScapeSolo.Lib.Extensions
{
    public static class MemoryStreamExtensions
    {
        public static void Clear(this MemoryStream stream)
        {
            stream.SetLength(0);
        }

        public static int Remaining(this MemoryStream stream)
        {
            return unchecked((int)(stream.Length - stream.Position));
        }
    }
}
