namespace OpenRS.Net.Client.Data
{
    internal sealed class BZip2BlockEntry
    {
        internal BZip2BlockEntry()
        {
            unzftab = new int[256];
            cumulativeCounts = new int[257];
            afn = new int[257];
            inUse = new bool[256];
            inUse16 = new bool[16];
            seqToUnseq = new int[256];
            yy = new int[4096];
            groupPositions = new int[16];
            selector = new sbyte[18002];
            selectorMtf = new sbyte[18002];
            len = RectangularArrays.ReturnRectangularSbyteArray(6, 258);
            limit = RectangularArrays.ReturnRectangularIntArray(6, 258);
            _base = RectangularArrays.ReturnRectangularIntArray(6, 258);
            perm = RectangularArrays.ReturnRectangularIntArray(6, 258);
            minLengths = new int[6];
        }

        internal sbyte[] inputBuffer;
        internal int offset;
        internal int compressedSize;
        internal int bytesReadLow;
        internal int bytesReadHigh;
        internal sbyte[] outputBuffer;
        internal int outputIndex;
        internal int decompressedSize;
        internal int bytesWrittenLow;
        internal int bytesWrittenHigh;
        internal sbyte lastOutputByte;
        internal int runLength;
        internal bool isRandomised;
        internal int currentBitWord;
        internal int bitsAvailable;
        internal int blockSize100k;
        internal int blocksRead;
        internal int origPtr;
        internal int linkedListNode;
        internal int currentByteValue;
        internal int[] unzftab;
        internal int symbolIndex;
        internal int[] cumulativeCounts;
        internal int[] afn;
        public static int[] transformVector;
        internal int inUseOffset;
        internal bool[] inUse;
        internal bool[] inUse16;
        internal int[] seqToUnseq;
        internal int[] yy;
        internal int[] groupPositions;
        internal sbyte[] selector;
        internal sbyte[] selectorMtf;
        internal sbyte[][] len;
        internal int[][] limit;
        internal int[][] _base;
        internal int[][] perm;
        internal int[] minLengths;
        internal int lastSymbolIndex;
    }
}