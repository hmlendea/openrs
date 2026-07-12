namespace OpenRS.Net.Client.Data
{
    internal sealed class BZip2BlockEntry
    {
        internal BZip2BlockEntry()
        {
            unzftab = new int[256];
            cumulativeCounts = new int[257];
            inUse = new bool[256];
            symbolGroupFlags = new bool[16];
            seqToUnseq = new int[256];
            moveToFrontBuffer = new int[4096];
            groupPositions = new int[16];
            selector = new sbyte[18002];
            selectorMtf = new sbyte[18002];
            huffmanCodeLengths = RectangularArrays.ReturnRectangularSbyteArray(6, 258);
            limit = RectangularArrays.ReturnRectangularIntArray(6, 258);
            huffmanBaseValues = RectangularArrays.ReturnRectangularIntArray(6, 258);
            huffmanPermutations = RectangularArrays.ReturnRectangularIntArray(6, 258);
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
        internal static int[] transformVector;
        internal int inUseOffset;
        internal bool[] inUse;
        internal bool[] symbolGroupFlags;
        internal int[] seqToUnseq;
        internal int[] moveToFrontBuffer;
        internal int[] groupPositions;
        internal sbyte[] selector;
        internal sbyte[] selectorMtf;
        internal sbyte[][] huffmanCodeLengths;
        internal int[][] limit;
        internal int[][] huffmanBaseValues;
        internal int[][] huffmanPermutations;
        internal int[] minLengths;
        internal int lastSymbolIndex;
    }
}