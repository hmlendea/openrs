using NuciXNA.Primitives;

namespace OpenRS.Settings
{
    public sealed class GraphicsSettings
    {
        public Size2D Resolution { get; set; }
        public bool Fullscreen { get; set; }

        public bool FogOfWar { get; set; }

        public bool ShowRoofs { get; set; }

        public GraphicsSettings()
        {
            Resolution = new Size2D(1024, 480);
            FogOfWar = true;
            ShowRoofs = true;
        }
    }
}
