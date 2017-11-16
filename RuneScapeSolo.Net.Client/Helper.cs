using System;

namespace RuneScapeSolo.Net.Client
{
    public static class Helper
    {
        static Random rnd;

        public static Random Random
        {
            get
            {
                if (rnd == null)
                {
                    rnd = new Random();
                }

                return rnd;
            }
        }
    }
}
