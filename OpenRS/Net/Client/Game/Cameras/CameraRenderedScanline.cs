namespace OpenRS.Net.Client.Game.Cameras
{
    internal readonly struct CameraRenderedScanline
    {
        internal bool HasSourceSpan { get; }

        internal int Shade { get; }

        internal int ShadeStep { get; }

        internal int StartX { get; }

        internal int Width { get; }

        private CameraRenderedScanline(
            bool hasSourceSpan,
            int shade,
            int shadeStep,
            int startX,
            int width)
        {
            HasSourceSpan = hasSourceSpan;
            Shade = shade;
            ShadeStep = shadeStep;
            StartX = startX;
            Width = width;
        }

        internal static CameraRenderedScanline Calculate(
            CameraVariable scanline,
            int screenCentreX)
        {
            int startX = scanline.LeftX >> 8;
            int maximumX = scanline.RightX >> 8;
            int width = maximumX - startX;

            if (width <= 0)
            {
                return new CameraRenderedScanline(
                    false,
                    0,
                    0,
                    startX,
                    width);
            }

            int shade = scanline.LeftShade;
            int shadeStep = (scanline.RightShade - shade) / width;

            if (startX < -screenCentreX)
            {
                shade += (-screenCentreX - startX) * shadeStep;
                startX = -screenCentreX;
                width = maximumX - startX;
            }

            if (maximumX > screenCentreX)
            {
                width = screenCentreX - startX;
            }

            return new CameraRenderedScanline(
                true,
                shade,
                shadeStep,
                startX,
                width);
        }
    }
}