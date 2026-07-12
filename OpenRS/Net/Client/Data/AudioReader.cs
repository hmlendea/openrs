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
            for (int i = 0; i < byteCount; i += 1)
            {
                if (offset < length)
                {
                    outputBuffer[bufferOffset + i] = data[offset];
                    offset += 1;
                }
                else
                {
                    outputBuffer[bufferOffset + i] = 0;
                }
            }

            return byteCount;
        }

        public int Read()
        {
            sbyte[] abyte0 = new sbyte[1];
            Read(abyte0, 0, 1);
            return abyte0[0];
        }

        private sbyte[] data;
        private int offset;
        private int length;
    }
}
