namespace OpenRS.Net.Client.Data
{
    public sealed class AudioReader
    {
        public AudioReader()
        {
           // AudioPlayer.player.start(this);
        }

        public void stop()
        {
          //  AudioPlayer.player.stop(this);
        }

        public void play(sbyte[] audioData, int startOffset, int byteCount)
        {
            data = audioData;
            offset = startOffset;
            length = startOffset + byteCount;
        }

        public int read(sbyte[] outputBuffer, int bufferOffset, int byteCount)
        {
            for (int i = 0; i < byteCount; i++)
            {
                if (offset < length)
                {
                    outputBuffer[bufferOffset + i] = data[offset += 1];
                }
                else
                {
                    outputBuffer[bufferOffset + i] = 0;
                }
            }

            return byteCount;
        }

        public int read()
        {
            sbyte[] abyte0 = new sbyte[1];
            read(abyte0, 0, 1);
            return abyte0[0];
        }

        private sbyte[] data;
        private int offset;
        private int length;
    }
}
