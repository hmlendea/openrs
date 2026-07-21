using System;
using System.IO;

using OpenRS.Settings;

namespace OpenRS.Net.Client.Game
{
    internal sealed class SectorLoader
    {
        private static int ByteMask => 0xff;

        private static int RunLengthMask => 0x7f;

        private static int ElevationInitialRunLengthValue => 64;

        private static int TextureInitialRunLengthValue => 35;

        private static sbyte GroundOverlayWaterValue => -6;

        private static sbyte GroundOverlayRoofValue => 8;

        private readonly EngineHandle engineHandle;

        internal SectorLoader(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal void LoadSector(int sectionX, int sectionY, int height, int sector)
        {
            string fileName = BuildSectorFileName(sectionX, sectionY, height);

            try
            {
                LoadHeightData(fileName, sector);

                if (!LoadWallData(fileName, sector))
                {
                    return;
                }

                LoadLocationData(fileName, sector);
            }
            catch (IOException)
            {
            }

            ResetSectorData(sector, height);
        }

        private static string BuildSectorFileName(int sectionX, int sectionY, int height)
            =>
                "m" + height +
                sectionX / 10 + sectionX % 10 +
                sectionY / 10 + sectionY % 10;

        private static sbyte[] LoadMapFile(string fileName)
        {
            string filePath = Path.Combine(ApplicationPaths.MapsDirectory, fileName);

            if (!File.Exists(filePath))
            {
                return null;
            }

            return (sbyte[])(Array)File.ReadAllBytes(filePath);
        }

        private void LoadHeightData(string fileName, int sector)
        {
            sbyte[] data = LoadMapFile(fileName + ".hei");

            if (data is null || data.Length == 0)
            {
                ResetGroundData(sector);
                return;
            }

            int readOffset = 0;
            readOffset = DecodeRepeatedSbyteValues(
                data,
                readOffset,
                engineHandle.TileGroundElevation[sector]);
            AccumulateSbyteColumns(
                engineHandle.TileGroundElevation[sector],
                ElevationInitialRunLengthValue);
            readOffset = DecodeRepeatedIntValues(
                data,
                readOffset,
                engineHandle.TileGroundTexture[sector]);
            AccumulateIntColumns(
                engineHandle.TileGroundTexture[sector],
                TextureInitialRunLengthValue);
        }

        private bool LoadWallData(string fileName, int sector)
        {
            sbyte[] data = LoadMapFile(fileName + ".dat");

            if (data is null || data.Length == 0)
            {
                return false;
            }

            int readOffset = 0;
            readOffset = CopySignedIntValues(
                data,
                readOffset,
                engineHandle.TileVerticalWall[sector]);
            readOffset = CopySignedIntValues(
                data,
                readOffset,
                engineHandle.TileHorizontalWall[sector]);
            readOffset = CopyMaskedIntValues(
                data,
                readOffset,
                engineHandle.TileDiagonalWall[sector]);
            readOffset = ApplyBackDiagonalWalls(data, readOffset, sector);
            readOffset = DecodeZeroRunLengthValues(
                data,
                readOffset,
                engineHandle.TileRoofType[sector]);
            readOffset = DecodeRepeatedIntValues(
                data,
                readOffset,
                engineHandle.TileGroundOverlay[sector]);
            DecodeZeroRunLengthValues(
                data,
                readOffset,
                engineHandle.TileObjectRotation[sector]);

            return true;
        }

        private void LoadLocationData(string fileName, int sector)
        {
            sbyte[] data = LoadMapFile(fileName + ".loc");

            if (data is null || data.Length == 0)
            {
                return;
            }

            int readOffset = 0;

            for (int tile = 0; tile < EngineHandle.TilesPerSector;)
            {
                int rawByte = data[readOffset++] & ByteMask;

                if (rawByte < EngineHandle.RunLengthThreshold)
                {
                    engineHandle.TileDiagonalWall[sector][tile] =
                        rawByte + EngineHandle.LocationEntityBase;
                    tile += 1;
                    continue;
                }

                tile += rawByte - EngineHandle.RunLengthThreshold;
            }
        }

        private static int CopySignedIntValues(sbyte[] data, int readOffset, int[] destination)
        {
            for (int tile = 0; tile < EngineHandle.TilesPerSector; tile += 1)
            {
                destination[tile] = data[readOffset++];
            }

            return readOffset;
        }

        private static int CopyMaskedIntValues(sbyte[] data, int readOffset, int[] destination)
        {
            for (int tile = 0; tile < EngineHandle.TilesPerSector; tile += 1)
            {
                destination[tile] = data[readOffset++] & ByteMask;
            }

            return readOffset;
        }

        private int ApplyBackDiagonalWalls(sbyte[] data, int readOffset, int sector)
        {
            for (int tile = 0; tile < EngineHandle.TilesPerSector; tile += 1)
            {
                int diagonalWallValue = data[readOffset++] & ByteMask;

                if (diagonalWallValue > 0)
                {
                    engineHandle.TileDiagonalWall[sector][tile] =
                        diagonalWallValue + EngineHandle.DiagonalWallBackOffset;
                }
            }

            return readOffset;
        }

        private static int DecodeRepeatedSbyteValues(
            sbyte[] data,
            int readOffset,
            sbyte[] destination)
        {
            int repeatedValue = 0;

            for (int tile = 0; tile < EngineHandle.TilesPerSector;)
            {
                int rawByte = data[readOffset++] & ByteMask;

                if (rawByte < EngineHandle.RunLengthThreshold)
                {
                    destination[tile] = (sbyte)rawByte;
                    repeatedValue = rawByte;
                    tile += 1;
                    continue;
                }

                tile = RepeatSbyteValue(destination, tile, repeatedValue, rawByte);
            }

            return readOffset;
        }

        private static int DecodeRepeatedIntValues(sbyte[] data, int readOffset, int[] destination)
        {
            int repeatedValue = 0;

            for (int tile = 0; tile < EngineHandle.TilesPerSector;)
            {
                int rawByte = data[readOffset++] & ByteMask;

                if (rawByte < EngineHandle.RunLengthThreshold)
                {
                    destination[tile] = rawByte;
                    repeatedValue = rawByte;
                    tile += 1;
                    continue;
                }

                tile = RepeatIntValue(destination, tile, repeatedValue, rawByte);
            }

            return readOffset;
        }

        private static int DecodeZeroRunLengthValues(sbyte[] data, int readOffset, int[] destination)
        {
            for (int tile = 0; tile < EngineHandle.TilesPerSector;)
            {
                int rawByte = data[readOffset++] & ByteMask;
                tile = DecodeZeroRunLengthValue(rawByte, destination, tile);
            }

            return readOffset;
        }

        private static int DecodeZeroRunLengthValue(int rawByte, int[] destination, int tile)
        {
            if (rawByte < EngineHandle.RunLengthThreshold)
            {
                destination[tile] = rawByte;
                return tile + 1;
            }

            return SkipOrClearIntValues(destination, tile, rawByte);
        }

        private static int RepeatSbyteValue(
            sbyte[] destination,
            int tile,
            int repeatedValue,
            int rawByte)
        {
            int repeatedCount = rawByte - EngineHandle.RunLengthThreshold;

            for (int runIndex = 0; runIndex < repeatedCount; runIndex += 1)
            {
                destination[tile++] = (sbyte)repeatedValue;
            }

            return tile;
        }

        private static int RepeatIntValue(int[] destination, int tile, int repeatedValue, int rawByte)
        {
            int repeatedCount = rawByte - EngineHandle.RunLengthThreshold;

            for (int runIndex = 0; runIndex < repeatedCount; runIndex += 1)
            {
                destination[tile++] = repeatedValue;
            }

            return tile;
        }

        private static int SkipOrClearIntValues(int[] destination, int tile, int rawByte)
        {
            int repeatedCount = rawByte - EngineHandle.RunLengthThreshold;

            for (int runIndex = 0; runIndex < repeatedCount; runIndex += 1)
            {
                destination[tile++] = 0;
            }

            return tile;
        }

        private static void AccumulateSbyteColumns(sbyte[] destination, int initialValue)
        {
            int accumulatedValue = initialValue;

            for (int column = 0; column < EngineHandle.SectorSize; column += 1)
            {
                accumulatedValue = AccumulateSbyteColumn(destination, column, accumulatedValue);
            }
        }

        private static void AccumulateIntColumns(int[] destination, int initialValue)
        {
            int accumulatedValue = initialValue;

            for (int column = 0; column < EngineHandle.SectorSize; column += 1)
            {
                accumulatedValue = AccumulateIntColumn(destination, column, accumulatedValue);
            }
        }

        private static int AccumulateSbyteColumn(sbyte[] destination, int column, int accumulatedValue)
        {
            for (int row = 0; row < EngineHandle.SectorSize; row += 1)
            {
                int tileIndex = row * EngineHandle.SectorSize + column;
                accumulatedValue = destination[tileIndex] + accumulatedValue & RunLengthMask;
                destination[tileIndex] = (sbyte)(accumulatedValue * 2);
            }

            return accumulatedValue;
        }

        private static int AccumulateIntColumn(int[] destination, int column, int accumulatedValue)
        {
            for (int row = 0; row < EngineHandle.SectorSize; row += 1)
            {
                int tileIndex = row * EngineHandle.SectorSize + column;
                accumulatedValue = destination[tileIndex] + accumulatedValue & RunLengthMask;
                destination[tileIndex] = accumulatedValue * 2;
            }

            return accumulatedValue;
        }

        private void ResetGroundData(int sector)
        {
            for (int tile = 0; tile < EngineHandle.TilesPerSector; tile += 1)
            {
                engineHandle.TileGroundElevation[sector][tile] = 0;
                engineHandle.TileGroundTexture[sector][tile] = 0;
            }
        }

        private void ResetSectorData(int sector, int height)
        {
            for (int tile = 0; tile < EngineHandle.TilesPerSector; tile += 1)
            {
                engineHandle.TileGroundElevation[sector][tile] = 0;
                engineHandle.TileGroundTexture[sector][tile] = 0;
                engineHandle.TileVerticalWall[sector][tile] = 0;
                engineHandle.TileHorizontalWall[sector][tile] = 0;
                engineHandle.TileDiagonalWall[sector][tile] = 0;
                engineHandle.TileRoofType[sector][tile] = 0;
                engineHandle.TileGroundOverlay[sector][tile] = GetGroundOverlayDefault(height);
                engineHandle.TileObjectRotation[sector][tile] = 0;
            }
        }

        private static sbyte GetGroundOverlayDefault(int height)
        {
            if (height == 0)
            {
                return GroundOverlayWaterValue;
            }

            if (height == 3)
            {
                return GroundOverlayRoofValue;
            }

            return 0;
        }
    }
}
