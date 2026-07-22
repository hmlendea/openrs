namespace OpenRS.Net.Client.Data
{
    internal static class RectangularArrays
    {
        internal static T[][] Create<T>(int outerSize, int innerSize)
        {
            T[][] result = new T[outerSize][];

            for (int outerIndex = 0; outerIndex < outerSize; outerIndex += 1)
            {
                result[outerIndex] = new T[innerSize];
            }

            return result;
        }
    }
}
