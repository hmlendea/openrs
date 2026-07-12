using System;

namespace OpenRS
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (GameWindow game = new GameWindow())
            {
                game.Run();
            }
        }
    }
}

