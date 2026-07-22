namespace OpenRS.Net.Client.Game
{
    internal sealed class PathSearchQueue
    {
        private readonly int[] bufferX;
        private readonly int[] bufferY;

        private int readIndex;
        private int writeIndex;

        internal bool IsEmpty => readIndex == writeIndex;

        internal int CurrentX => bufferX[readIndex];

        internal int CurrentY => bufferY[readIndex];

        internal PathSearchQueue(int[] bufferX, int[] bufferY)
        {
            this.bufferX = bufferX;
            this.bufferY = bufferY;
            readIndex = 0;
            writeIndex = 0;
        }

        internal void Enqueue(int x, int y)
        {
            bufferX[writeIndex] = x;
            bufferY[writeIndex] = y;
            writeIndex = (writeIndex + 1) % bufferX.Length;
        }

        internal void Advance()
        {
            readIndex = (readIndex + 1) % bufferX.Length;
        }
    }
}
