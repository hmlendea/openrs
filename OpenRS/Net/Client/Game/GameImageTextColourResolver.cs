using System;

namespace OpenRS.Net.Client.Game
{
    internal static class GameImageTextColourResolver
    {
        private static int ColourMaxRgb => 16777215;

        internal static int Resolve(string colourCode, int currentColour)
            => colourCode switch
            {
                "red" => 0xff0000,
                "lre" => 0xff9040,
                "yel" => 0xffff00,
                "gre" => 0x00ff00,
                "blu" => 0x0000ff,
                "cya" => 0x00ffff,
                "mag" => 0xff00ff,
                "whi" => 0xffffff,
                "nor" => 0,
                "dre" => 0xc00000,
                "ora" => 0xff9040,
                "ran" => (int)(new Random().NextDouble() * ColourMaxRgb),
                "or1" => 0xffb000,
                "or2" => 0xff7000,
                "or3" => 0xff3000,
                "gr1" => 0xc0ff00,
                "gr2" => 0x80ff00,
                "gr3" => 0x40ff00,
                _ => currentColour
            };
    }
}