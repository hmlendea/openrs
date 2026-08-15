namespace OpenRS.Net.Client.Game.Cameras
{
    public sealed class CameraDepthSorter
    {
        private readonly CameraModelDependencyResolver dependencyResolver = new();

        public void SortByDepth(CameraModel[] models, int startIndex, int endIndex)
            => CameraDepthQuickSorter.Sort(models, startIndex, endIndex);

        public void ResolveRenderOrder(int maxLookAheadCount, CameraModel[] models, int modelCount)
            => dependencyResolver.Resolve(maxLookAheadCount, models, modelCount);
    }
}
