namespace OpenRS.Net.Client.Game.Cameras
{
    public sealed class CameraDepthSorter
    {
        private int reorderedRangeStart;
        private int reorderedRangeEnd;

        public void SortByDepth(CameraModel[] models, int startIndex, int endIndex)
        {
            if (startIndex >= endIndex)
            {
                return;
            }

            int partitionIndex = PartitionByDepth(models, startIndex, endIndex);
            SortByDepth(models, startIndex, partitionIndex);
            SortByDepth(models, partitionIndex + 1, endIndex);
        }

        private static int PartitionByDepth(CameraModel[] models, int startIndex, int endIndex)
        {
            int leftPartition = startIndex - 1;
            int rightPartition = endIndex + 1;
            int middleIndex = (startIndex + endIndex) / 2;
            CameraModel pivotModel = models[middleIndex];
            models[middleIndex] = models[startIndex];
            models[startIndex] = pivotModel;
            int pivotScale = pivotModel.Scale;

            while (leftPartition < rightPartition)
            {
                do
                {
                    rightPartition -= 1;
                }
                while (models[rightPartition].Scale < pivotScale);

                do
                {
                    leftPartition += 1;
                }
                while (models[leftPartition].Scale > pivotScale);

                if (leftPartition < rightPartition)
                {
                    CameraModel swapModel = models[leftPartition];
                    models[leftPartition] = models[rightPartition];
                    models[rightPartition] = swapModel;
                }
            }

            return rightPartition;
        }

        public void ResolveRenderOrder(int maxLookAheadCount, CameraModel[] models, int modelCount)
        {
            for (int modelIndex = 0; modelIndex < modelCount; modelIndex += 1)
            {
                models[modelIndex].IsSorted = false;
                models[modelIndex].SortIndex = modelIndex;
                models[modelIndex].DependencyIndex = -1;
            }

            int searchIndex = 0;

            while (true)
            {
                while (searchIndex < modelCount && models[searchIndex].IsSorted)
                {
                    searchIndex += 1;
                }

                if (searchIndex == modelCount)
                {
                    return;
                }

                CameraModel currentModel = models[searchIndex];
                currentModel.IsSorted = true;
                int rangeStart = searchIndex;
                int rangeEnd = searchIndex + maxLookAheadCount;

                if (rangeEnd >= modelCount)
                {
                    rangeEnd = modelCount - 1;
                }

                for (int compareIndex = rangeEnd; compareIndex >= rangeStart + 1; compareIndex -= 1)
                {
                    CameraModel compareModel = models[compareIndex];

                    if (currentModel.BoundsMinX < compareModel.BoundsMaxX &&
                        compareModel.BoundsMinX < currentModel.BoundsMaxX &&
                        currentModel.BoundsMinY < compareModel.BoundsMaxY &&
                        compareModel.BoundsMinY < currentModel.BoundsMaxY &&
                        currentModel.SortIndex != compareModel.DependencyIndex &&
                        !PolygonIntersectionCalculator.AreBoundsDisjoint(currentModel, compareModel) &&
                        PolygonIntersectionCalculator.IsModelBehind(compareModel, currentModel))
                    {
                        TryReorderOverlappingModels(models, rangeStart, compareIndex);

                        if (models[compareIndex] != compareModel)
                        {
                            compareIndex += 1;
                        }

                        rangeStart = reorderedRangeStart;
                        compareModel.DependencyIndex = currentModel.SortIndex;
                    }
                }
            }
        }

        private bool TryReorderOverlappingModels(CameraModel[] models, int start, int stop)
        {
            while (true)
            {
                CameraModel frontModel = models[start];

                for (int forwardIndex = start + 1; forwardIndex <= stop; forwardIndex += 1)
                {
                    CameraModel forwardCandidate = models[forwardIndex];

                    if (!PolygonIntersectionCalculator.AreBoundsDisjoint(forwardCandidate, frontModel))
                    {
                        break;
                    }

                    models[start] = forwardCandidate;
                    models[forwardIndex] = frontModel;
                    start = forwardIndex;

                    if (start == stop)
                    {
                        reorderedRangeStart = start;
                        reorderedRangeEnd = start - 1;

                        return true;
                    }
                }

                CameraModel backModel = models[stop];

                for (int backwardIndex = stop - 1; backwardIndex >= start; backwardIndex -= 1)
                {
                    CameraModel backwardCandidate = models[backwardIndex];

                    if (!PolygonIntersectionCalculator.AreBoundsDisjoint(backModel, backwardCandidate))
                    {
                        break;
                    }

                    models[stop] = backwardCandidate;
                    models[backwardIndex] = backModel;
                    stop = backwardIndex;

                    if (start == stop)
                    {
                        reorderedRangeStart = stop + 1;
                        reorderedRangeEnd = stop;

                        return true;
                    }
                }

                if (start + 1 >= stop)
                {
                    reorderedRangeStart = start;
                    reorderedRangeEnd = stop;

                    return false;
                }

                if (!TryReorderOverlappingModels(models, start + 1, stop))
                {
                    reorderedRangeStart = start;

                    return false;
                }

                stop = reorderedRangeEnd;
            }
        }
    }
}
