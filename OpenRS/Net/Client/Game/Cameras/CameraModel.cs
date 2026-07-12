namespace OpenRS.Net.Client.Game.Cameras
{
    public sealed class CameraModel
    {

        public CameraModel()
        {
            isSorted = false;
            dependencyIndex = -1;
        }

        public int boundsMinX;
        public int boundsMinY;
        public int boundsMaxX;
        public int boundsMaxY;
        public int boundsMinZ;
        public int boundsMaxZ;
        public GameObject Object;
        public int faceVertCountIndex1;
        public int Scale;
        public int normalX;
        public int normalY;
        public int normalZ;
        public int visibilityDot;
        public int currentTextureIndex;
        public bool isSorted;
        public int sortIndex;
        public int dependencyIndex;
    }
}
