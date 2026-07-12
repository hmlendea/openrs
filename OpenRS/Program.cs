using System;

namespace OpenRS
{
    internal static class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using GameWindow game = new();
            game.Run();
        }
    }
}
