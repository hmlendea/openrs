namespace OpenRS.Net.Client.Game.Cameras
{
    internal sealed class CameraModelDependencyResolver
    {
        private int reorderedRangeStart;
        private int reorderedRangeEnd;

        internal void Resolve(
            int maximumLookAheadCount,
            CameraModel[] models,
            int modelCount)
        {
            InitialiseRenderOrderState(models, modelCount);
            int searchIndex = 0;

            while (true)
            {
                searchIndex = FindNextUnsortedModel(
                    models,
                    searchIndex,
                    modelCount);

                if (searchIndex == modelCount)
                {
                    return;
                }

                ResolveModelDependencies(
                    maximumLookAheadCount,
                    models,
                    modelCount,
                    searchIndex);
            }
        }

        private static void InitialiseRenderOrderState(
            CameraModel[] models,
            int modelCount)
        {
            for (int modelIndex = 0; modelIndex < modelCount; modelIndex += 1)
            {
                models[modelIndex].IsSorted = false;
                models[modelIndex].SortIndex = modelIndex;
                models[modelIndex].DependencyIndex = -1;
            }
        }

        private static int FindNextUnsortedModel(
            CameraModel[] models,
            int searchIndex,
            int modelCount)
        {
            while (searchIndex < modelCount && models[searchIndex].IsSorted)
            {
                searchIndex += 1;
            }

            return searchIndex;
        }

        private void ResolveModelDependencies(
            int maximumLookAheadCount,
            CameraModel[] models,
            int modelCount,
            int searchIndex)
        {
            CameraModel currentModel = models[searchIndex];
            currentModel.IsSorted = true;
            int rangeStart = searchIndex;
            int rangeEnd = searchIndex + maximumLookAheadCount;

            if (rangeEnd >= modelCount)
            {
                rangeEnd = modelCount - 1;
            }

            for (int compareIndex = rangeEnd;
                compareIndex >= rangeStart + 1;
                compareIndex -= 1)
            {
                CameraModel compareModel = models[compareIndex];

                if (currentModel.BoundsMinX < compareModel.BoundsMaxX &&
                    compareModel.BoundsMinX < currentModel.BoundsMaxX &&
                    currentModel.BoundsMinY < compareModel.BoundsMaxY &&
                    compareModel.BoundsMinY < currentModel.BoundsMaxY &&
                    currentModel.SortIndex != compareModel.DependencyIndex &&
                    !PolygonIntersectionCalculator.AreBoundsDisjoint(
                        currentModel,
                        compareModel) &&
                    PolygonIntersectionCalculator.IsModelBehind(
                        compareModel,
                        currentModel))
                {
                    TryReorderOverlappingModels(
                        models,
                        rangeStart,
                        compareIndex);

                    if (models[compareIndex] != compareModel)
                    {
                        compareIndex += 1;
                    }

                    rangeStart = reorderedRangeStart;
                    compareModel.DependencyIndex = currentModel.SortIndex;
                }
            }
        }

        private bool TryReorderOverlappingModels(
            CameraModel[] models,
            int start,
            int stop)
        {
            while (true)
            {
                CameraModel frontModel = models[start];

                for (int forwardIndex = start + 1;
                    forwardIndex <= stop;
                    forwardIndex += 1)
                {
                    CameraModel forwardCandidate = models[forwardIndex];

                    if (!PolygonIntersectionCalculator.AreBoundsDisjoint(
                        forwardCandidate,
                        frontModel))
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

                for (int backwardIndex = stop - 1;
                    backwardIndex >= start;
                    backwardIndex -= 1)
                {
                    CameraModel backwardCandidate = models[backwardIndex];

                    if (!PolygonIntersectionCalculator.AreBoundsDisjoint(
                        backModel,
                        backwardCandidate))
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