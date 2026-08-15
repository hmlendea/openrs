namespace OpenRS.Net.Client.Game.Cameras
{
    internal readonly struct CameraTextureScanlineState
    {
        internal int DenominatorColumnStep { get; }

        internal int DenominatorOrigin { get; }

        internal int DenominatorRowStep { get; }

        internal int PixelOffset { get; }

        internal int RowStride { get; }

        internal int ScanlineStep { get; }

        internal int UColumnStep { get; }

        internal int UOrigin { get; }

        internal int URowStep { get; }

        internal int VColumnStep { get; }

        internal int VOrigin { get; }

        internal int VRowStep { get; }

        internal CameraTextureScanlineState(
            CameraTextureProjection projection,
            CameraPolygonRasteriser rasteriser,
            int scanlineBufferCentre,
            int defaultScreenHalfWidth,
            int screenMouseOffsetX,
            bool isRenderingInterlaced)
        {
            int uOrigin = projection.UOrigin;
            int uColumnStep = projection.UColumnStep;
            int vOrigin = projection.VOrigin;
            int vColumnStep = projection.VColumnStep;
            int denominatorOrigin = projection.DenominatorOrigin;
            int denominatorColumnStep = projection.DenominatorColumnStep;
            int scanlineOffset =
                rasteriser.MinVisibleScanline - scanlineBufferCentre;
            int rowStride = defaultScreenHalfWidth;
            int pixelOffset =
                screenMouseOffsetX +
                rasteriser.MinVisibleScanline * rowStride;
            int scanlineStep = 1;
            uOrigin += uColumnStep * scanlineOffset;
            vOrigin += vColumnStep * scanlineOffset;
            denominatorOrigin += denominatorColumnStep * scanlineOffset;

            if (isRenderingInterlaced)
            {
                if ((rasteriser.MinVisibleScanline & 1) == 1)
                {
                    rasteriser.MinVisibleScanline += 1;
                    uOrigin += uColumnStep;
                    vOrigin += vColumnStep;
                    denominatorOrigin += denominatorColumnStep;
                    pixelOffset += rowStride;
                }

                uColumnStep <<= 1;
                vColumnStep <<= 1;
                denominatorColumnStep <<= 1;
                rowStride <<= 1;
                scanlineStep = 2;
            }

            DenominatorColumnStep = denominatorColumnStep;
            DenominatorOrigin = denominatorOrigin;
            DenominatorRowStep = projection.DenominatorRowStep;
            PixelOffset = pixelOffset;
            RowStride = rowStride;
            ScanlineStep = scanlineStep;
            UColumnStep = uColumnStep;
            UOrigin = uOrigin;
            URowStep = projection.URowStep;
            VColumnStep = vColumnStep;
            VOrigin = vOrigin;
            VRowStep = projection.VRowStep;
        }
    }
}