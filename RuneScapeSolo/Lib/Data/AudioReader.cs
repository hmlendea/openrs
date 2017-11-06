namespace RuneScapeSolo.Lib.Data
{
    public class AudioReader
    {
        sbyte[] data;
        int offset;
        int length;

        public AudioReader()
        {
           // AudioPlayer.player.start(this);
        }

        public void Stop()
        {
          //  AudioPlayer.player.stop(this);
        }

        public void Play(sbyte[] data, int offset, int length)
        {
            this.data = data;
            this.offset = offset;
            this.length = offset + length;
        }

        public int Read(sbyte[] data, int offset, int length)
        {
            for (int i = 0; i < length; i++)
            {
                if (this.offset < this.length)
                {
                    data[offset + i] = this.data[this.offset++];
                }
                else
                {
                    data[offset + i] = 0;
                }
            }

            return length;
        }

        public int Read()
        {
            sbyte[] data = new sbyte[1];

            Read(data, 0, 1);

            return data[0];
        }
    }
}
