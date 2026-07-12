namespace OpenRS.Net.Client.Data
{
    public sealed class AudioReader
    {
        public AudioReader()
        {
           // AudioPlayer.player.start(this);
        }

        public void Stop()
        {
          //  AudioPlayer.player.stop(this);
        }

        public void Play(sbyte[] audioData, int startOffset, int byteCount)
        {
            data = audioData;
            offset = startOffset;
            length = startOffset + byteCount;
        }

        public int Read(sbyte[] outputBuffer, int bufferOffset, int byteCount)
        {
            for (int byteIndex = 0; byteIndex < byteCount; byteIndex += 1)
            {
                if (offset < length)
                {
                    outputBuffer[bufferOffset + byteIndex] = data[offset];
                    offset += 1;
                }
                else
                {
                    outputBuffer[bufferOffset + byteIndex] = 0;
                }
            }

            return byteCount;
        }

        public int Read()
        {
            sbyte[] singleByte = new sbyte[1];
            Read(singleByte, 0, 1);

            return singleByte[0];
        }

        private sbyte[] data;
        private int offset;
        private int length;
    }
}
