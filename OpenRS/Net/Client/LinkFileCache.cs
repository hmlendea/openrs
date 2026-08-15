namespace OpenRS.Net.Client
{
    internal static class LinkFileCache
    {
        private static int Capacity => 50;

        private static int currentFile;
        private static readonly string[] fileNames = new string[Capacity];
        private static readonly sbyte[][] fileData = new sbyte[Capacity][];

        internal static void Add(string fileName, sbyte[] data)
        {
            fileNames[currentFile] = fileName;
            fileData[currentFile] = data;
            currentFile += 1;
        }

        internal static int FindIndex(string fileName)
        {
            for (int fileIndex = 0; fileIndex < currentFile; fileIndex += 1)
            {
                if (string.Equals(fileNames[fileIndex], fileName))
                {
                    return fileIndex;
                }
            }

            return -1;
        }

        internal static sbyte[] GetData(int fileIndex) => fileData[fileIndex];
    }
}