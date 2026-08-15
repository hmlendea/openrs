namespace OpenRS.Net.Client.Game.Cameras
{
    internal readonly struct CameraTextureProjection
    {
        internal int DenominatorColumnStep { get; }

        internal int DenominatorOrigin { get; }

        internal int DenominatorRowStep { get; }

        internal int UColumnStep { get; }

        internal int UOrigin { get; }

        internal int URowStep { get; }

        internal int VColumnStep { get; }

        internal int VOrigin { get; }

        internal int VRowStep { get; }

        private CameraTextureProjection(
            int denominatorColumnStep,
            int denominatorOrigin,
            int denominatorRowStep,
            int uColumnStep,
            int uOrigin,
            int uRowStep,
            int vColumnStep,
            int vOrigin,
            int vRowStep)
        {
            DenominatorColumnStep = denominatorColumnStep;
            DenominatorOrigin = denominatorOrigin;
            DenominatorRowStep = denominatorRowStep;
            UColumnStep = uColumnStep;
            UOrigin = uOrigin;
            URowStep = uRowStep;
            VColumnStep = vColumnStep;
            VOrigin = vOrigin;
            VRowStep = vRowStep;
        }

        internal static CameraTextureProjection Calculate(
            int originX,
            int originY,
            int originZ,
            int firstEdgeX,
            int firstEdgeY,
            int firstEdgeZ,
            int secondEdgeX,
            int secondEdgeY,
            int secondEdgeZ,
            int screenProjectionShift,
            int textureCoordinateShift)
        {
            int textureOriginShift = textureCoordinateShift + 5;
            int textureRowShift =
                5 - screenProjectionShift + textureCoordinateShift + 4;
            int textureColumnShift =
                5 - screenProjectionShift + textureCoordinateShift;

            return new CameraTextureProjection(
                (firstEdgeX * secondEdgeZ - firstEdgeZ * secondEdgeX) >>
                    (screenProjectionShift - 5),
                (firstEdgeY * secondEdgeX - firstEdgeX * secondEdgeY) << 5,
                (firstEdgeZ * secondEdgeY - firstEdgeY * secondEdgeZ) <<
                    (5 - screenProjectionShift + 4),
                (secondEdgeZ * originX - secondEdgeX * originZ) <<
                    textureColumnShift,
                (secondEdgeX * originY - secondEdgeY * originX) <<
                    textureOriginShift,
                (secondEdgeY * originZ - secondEdgeZ * originY) <<
                    textureRowShift,
                (firstEdgeZ * originX - firstEdgeX * originZ) <<
                    textureColumnShift,
                (firstEdgeX * originY - firstEdgeY * originX) <<
                    textureOriginShift,
                (firstEdgeY * originZ - firstEdgeZ * originY) <<
                    textureRowShift);
        }
    }
}