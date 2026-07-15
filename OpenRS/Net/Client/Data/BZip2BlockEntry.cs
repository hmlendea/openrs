namespace OpenRS.Net.Client.Data
{
    internal sealed class BZip2BlockEntry
    {
        internal BZip2BlockEntry()
        {
            SymbolFrequencyTable = new int[256];
            CumulativeCounts = new int[257];
            InUse = new bool[256];
            SymbolGroupFlags = new bool[16];
            SequenceToSymbol = new int[256];
            MoveToFrontBuffer = new int[4096];
            GroupPositions = new int[16];
            Selector = new sbyte[18002];
            SelectorMoveToFront = new sbyte[18002];
            HuffmanCodeLengths = RectangularArrays.ReturnRectangularSbyteArray(6, 258);
            HuffmanLimits = RectangularArrays.ReturnRectangularIntArray(6, 258);
            HuffmanBaseValues = RectangularArrays.ReturnRectangularIntArray(6, 258);
            HuffmanPermutations = RectangularArrays.ReturnRectangularIntArray(6, 258);
            HuffmanMinLengths = new int[6];
        }

        internal sbyte[] InputBuffer { get; set; }
        internal int Offset { get; set; }
        internal int CompressedSize { get; set; }
        internal int BytesReadLow { get; set; }
        internal int BytesReadHigh { get; set; }
        internal sbyte[] OutputBuffer { get; set; }
        internal int OutputIndex { get; set; }
        internal int DecompressedSize { get; set; }
        internal int BytesWrittenLow { get; set; }
        internal int BytesWrittenHigh { get; set; }
        internal sbyte LastOutputByte { get; set; }
        internal int RunLength { get; set; }
        internal bool IsRandomised { get; set; }
        internal int CurrentBitWord { get; set; }
        internal int BitsAvailable { get; set; }
        internal int BlockSize100k { get; set; }
        internal int BlocksRead { get; set; }
        internal int OriginPointer { get; set; }
        internal int LinkedListNode { get; set; }
        internal int CurrentByteValue { get; set; }
        internal int[] SymbolFrequencyTable { get; set; }
        internal int SymbolIndex { get; set; }
        internal int[] CumulativeCounts { get; set; }
        internal static int[] TransformVector { get; set; }
        internal int ActiveSymbolCount { get; set; }
        internal bool[] InUse { get; set; }
        internal bool[] SymbolGroupFlags { get; set; }
        internal int[] SequenceToSymbol { get; set; }
        internal int[] MoveToFrontBuffer { get; set; }
        internal int[] GroupPositions { get; set; }
        internal sbyte[] Selector { get; set; }
        internal sbyte[] SelectorMoveToFront { get; set; }
        internal sbyte[][] HuffmanCodeLengths { get; set; }
        internal int[][] HuffmanLimits { get; set; }
        internal int[][] HuffmanBaseValues { get; set; }
        internal int[][] HuffmanPermutations { get; set; }
        internal int[] HuffmanMinLengths { get; set; }
        internal int LastSymbolIndex { get; set; }
    }
}