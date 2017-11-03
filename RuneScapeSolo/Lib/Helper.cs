using System;

namespace RuneScapeSolo.Lib
{
    public class Helper
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
