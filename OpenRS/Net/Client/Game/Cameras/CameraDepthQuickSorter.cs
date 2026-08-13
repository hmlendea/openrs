namespace OpenRS.Net.Client.Game.Cameras
{
    internal static class CameraDepthQuickSorter
    {
        internal static void Sort(
            CameraModel[] models,
            int startIndex,
            int endIndex)
        {
            if (startIndex >= endIndex)
            {
                return;
            }

            int partitionIndex = Partition(models, startIndex, endIndex);
            Sort(models, startIndex, partitionIndex);
            Sort(models, partitionIndex + 1, endIndex);
        }

        private static int Partition(
            CameraModel[] models,
            int startIndex,
            int endIndex)
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
    }
}