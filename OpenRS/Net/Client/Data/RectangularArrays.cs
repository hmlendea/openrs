//----------------------------------------------------------------------------------------
//	Copyright � 2007 - 2012 Tangible Software Solutions Inc.
//	This class can be used by anyone provided that the copyright notice remains intact.
//
//	This class provides the logic to simulate Java rectangular arrays, which are jagged
//	arrays with inner arrays of the same length.
//----------------------------------------------------------------------------------------
namespace OpenRS.Net.Client.Data
{
    internal static class RectangularArrays
    {
        internal static sbyte[][] ReturnRectangularSbyteArray(int outerSize, int innerSize)
        {
            sbyte[][] result = new sbyte[outerSize][];

            for (int outerIndex = 0; outerIndex < outerSize; outerIndex += 1)
            {
                result[outerIndex] = new sbyte[innerSize];
            }

            return result;
        }

        internal static int[][] ReturnRectangularIntArray(int outerSize, int innerSize)
        {
            int[][] result = new int[outerSize][];

            for (int outerIndex = 0; outerIndex < outerSize; outerIndex += 1)
            {
                result[outerIndex] = new int[innerSize];
            }

            return result;
        }
    }
}