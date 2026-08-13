using System;

namespace OpenRS.Net.Client.Game
{
    internal sealed class PathSearchQueue
    {
        private readonly int[] bufferX;
        private readonly int[] bufferY;

        private int readIndex;
        private int writeIndex;
        private int count;

        internal bool IsEmpty => count == 0;

        internal int CurrentX => bufferX[readIndex];

        internal int CurrentY => bufferY[readIndex];

        internal PathSearchQueue(int[] bufferX, int[] bufferY)
        {
            this.bufferX = bufferX;
            this.bufferY = bufferY;
            readIndex = 0;
            writeIndex = 0;
            count = 0;
        }

        internal void Enqueue(int x, int y)
        {
            if (count > 0 && count == bufferX.Length)
            {
                throw new InvalidOperationException("The path search queue is full.");
            }

            bufferX[writeIndex] = x;
            bufferY[writeIndex] = y;
            writeIndex = (writeIndex + 1) % bufferX.Length;
            count += 1;
        }

        internal void Advance()
        {
            readIndex = (readIndex + 1) % bufferX.Length;
            count -= 1;
        }
    }
}
