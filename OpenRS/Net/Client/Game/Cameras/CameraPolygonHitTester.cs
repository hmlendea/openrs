namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraPolygonHitTester(
        CameraSceneObjectTracker sceneObjectTracker)
    {
        internal void Check(
            GameObject gameObject,
            int faceVertexIndex,
            CameraVariable[] scanlineVariables,
            int minimumVisibleScanline,
            int maximumVisibleScanline)
        {
            if (sceneObjectTracker.IsHitCandidate &&
                sceneObjectTracker.MouseAdjustedY >= minimumVisibleScanline &&
                sceneObjectTracker.MouseAdjustedY < maximumVisibleScanline)
            {
                CameraVariable scanlineAtHitY =
                    scanlineVariables[sceneObjectTracker.MouseAdjustedY];
                bool isWithinScanlineX =
                    sceneObjectTracker.MouseAdjustedX >=
                    scanlineAtHitY.LeftX >> 8 &&
                    sceneObjectTracker.MouseAdjustedX <=
                    scanlineAtHitY.RightX >> 8;
                bool hasScanlineSpan =
                    scanlineAtHitY.LeftX <= scanlineAtHitY.RightX;
                bool isHittablePolygon =
                    !gameObject.DoesShareEntityArrays &&
                    gameObject.PolygonTypeData[faceVertexIndex] == 0;

                if (isWithinScanlineX &&
                    hasScanlineSpan &&
                    isHittablePolygon)
                {
                    sceneObjectTracker.RecordHit(gameObject, faceVertexIndex);
                }
            }
        }
    }
}