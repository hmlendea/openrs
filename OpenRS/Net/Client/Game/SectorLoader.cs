using System;
using System.IO;

using OpenRS.Settings;

namespace OpenRS.Net.Client.Game
{
    internal sealed class SectorLoader
    {
        private readonly EngineHandle engineHandle;

        internal SectorLoader(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal void LoadSector(int sectionX, int sectionY, int height, int sector)
        {
            string filename =
                "m" + height +
                sectionX / 10 + sectionX % 10 +
                sectionY / 10 + sectionY % 10;

            try
            {
                sbyte[] data = LoadMapFile(filename + ".hei");

                if (data is not null && data.Length > 0)
                {
                    int readOffset = 0;
                    int runLengthValue = 0;

                    for (int tile = 0; tile < EngineHandle.TilesPerSector;)
                    {
                        int rawByte = data[readOffset++] & 0xff;

                        if (rawByte < EngineHandle.RunLengthThreshold)
                        {
                            engineHandle.tileGroundElevation[sector][tile++] = (sbyte)rawByte;
                            runLengthValue = rawByte;
                        }

                        if (rawByte >= EngineHandle.RunLengthThreshold)
                        {
                            for (int runIndex = 0; runIndex < rawByte - EngineHandle.RunLengthThreshold; runIndex += 1)
                            {
                                engineHandle.tileGroundElevation[sector][tile++] = (sbyte)runLengthValue;
                            }
                        }
                    }

                    runLengthValue = 64;

                    for (int column = 0; column < EngineHandle.SectorSize; column += 1)
                    {
                        for (int row = 0; row < EngineHandle.SectorSize; row += 1)
                        {
                            runLengthValue = engineHandle.tileGroundElevation[sector][row * EngineHandle.SectorSize + column] + runLengthValue & 0x7f;
                            engineHandle.tileGroundElevation[sector][row * EngineHandle.SectorSize + column] = (sbyte)(runLengthValue * 2);
                        }
                    }

                    runLengthValue = 0;

                    for (int tile = 0; tile < EngineHandle.TilesPerSector;)
                    {
                        int rawByte = data[readOffset++] & 0xff;

                        if (rawByte < EngineHandle.RunLengthThreshold)
                        {
                            engineHandle.tileGroundTexture[sector][tile++] = rawByte;
                            runLengthValue = rawByte;
                        }

                        if (rawByte >= EngineHandle.RunLengthThreshold)
                        {
                            for (int runIndex = 0; runIndex < rawByte - EngineHandle.RunLengthThreshold; runIndex += 1)
                            {
                                engineHandle.tileGroundTexture[sector][tile++] = runLengthValue;
                            }
                        }
                    }

                    runLengthValue = 35;

                    for (int column = 0; column < EngineHandle.SectorSize; column += 1)
                    {
                        for (int row = 0; row < EngineHandle.SectorSize; row += 1)
                        {
                            runLengthValue = engineHandle.tileGroundTexture[sector][row * EngineHandle.SectorSize + column] + runLengthValue & 0x7f;
                            engineHandle.tileGroundTexture[sector][row * EngineHandle.SectorSize + column] = runLengthValue * 2;
                        }
                    }
                }
                else
                {
                    for (int tile = 0; tile < EngineHandle.TilesPerSector; tile += 1)
                    {
                        engineHandle.tileGroundElevation[sector][tile] = 0;
                        engineHandle.tileGroundTexture[sector][tile] = 0;
                    }
                }

                data = LoadMapFile(filename + ".dat");

                if (data is null || data.Length == 0)
                {
                    return;
                }

                int wallReadOffset = 0;

                for (int tile = 0; tile < EngineHandle.TilesPerSector; tile += 1)
                {
                    engineHandle.tileVerticalWall[sector][tile] = data[wallReadOffset++];
                }

                for (int tile = 0; tile < EngineHandle.TilesPerSector; tile += 1)
                {
                    engineHandle.tileHorizontalWall[sector][tile] = data[wallReadOffset++];
                }

                for (int tile = 0; tile < EngineHandle.TilesPerSector; tile += 1)
                {
                    engineHandle.tileDiagonalWall[sector][tile] = data[wallReadOffset++] & 0xff;
                }

                for (int tile = 0; tile < EngineHandle.TilesPerSector; tile += 1)
                {
                    int secondDiagonalByte = data[wallReadOffset++] & 0xff;

                    if (secondDiagonalByte > 0)
                    {
                        engineHandle.tileDiagonalWall[sector][tile] = secondDiagonalByte + EngineHandle.DiagonalWallBackOffset;
                    }
                }

                for (int tile = 0; tile < EngineHandle.TilesPerSector;)
                {
                    int rawByte = data[wallReadOffset++] & 0xff;

                    if (rawByte < EngineHandle.RunLengthThreshold)
                    {
                        engineHandle.tileRoofType[sector][tile++] = rawByte;
                    }
                    else
                    {
                        for (int runIndex = 0; runIndex < rawByte - EngineHandle.RunLengthThreshold; runIndex += 1)
                        {
                            engineHandle.tileRoofType[sector][tile++] = 0;
                        }
                    }
                }

                // Adds water on lower heights.
                int lastOverlay = 0;

                for (int tile = 0; tile < EngineHandle.TilesPerSector;)
                {
                    int rawByte = data[wallReadOffset++] & 0xff;

                    if (rawByte < EngineHandle.RunLengthThreshold)
                    {
                        engineHandle.tileGroundOverlay[sector][tile++] = (sbyte)rawByte;
                        lastOverlay = rawByte;
                    }
                    else
                    {
                        for (int runIndex = 0; runIndex < rawByte - EngineHandle.RunLengthThreshold; runIndex += 1)
                        {
                            engineHandle.tileGroundOverlay[sector][tile++] = (sbyte)lastOverlay;
                        }
                    }
                }

                for (int tile = 0; tile < EngineHandle.TilesPerSector;)
                {
                    int rawByte = data[wallReadOffset++] & 0xff;

                    if (rawByte < EngineHandle.RunLengthThreshold)
                    {
                        engineHandle.tileObjectRotation[sector][tile++] = (sbyte)rawByte;
                    }
                    else
                    {
                        for (int runIndex = 0; runIndex < rawByte - EngineHandle.RunLengthThreshold; runIndex += 1)
                        {
                            engineHandle.tileObjectRotation[sector][tile++] = 0;
                        }
                    }
                }

                data = LoadMapFile(filename + ".loc");

                if (data is not null && data.Length > 0)
                {
                    int locReadOffset = 0;

                    for (int tile = 0; tile < EngineHandle.TilesPerSector;)
                    {
                        int rawByte = data[locReadOffset++] & 0xff;

                        if (rawByte < EngineHandle.RunLengthThreshold)
                        {
                            engineHandle.tileDiagonalWall[sector][tile++] = rawByte + EngineHandle.LocationEntityBase;
                        }
                        else
                        {
                            tile += rawByte - EngineHandle.RunLengthThreshold;
                        }
                    }

                    return;
                }
            }
            catch (IOException)
            {
            }

            for (int tile = 0; tile < EngineHandle.TilesPerSector; tile += 1)
            {
                engineHandle.tileGroundElevation[sector][tile] = 0;
                engineHandle.tileGroundTexture[sector][tile] = 0;
                engineHandle.tileVerticalWall[sector][tile] = 0;
                engineHandle.tileHorizontalWall[sector][tile] = 0;
                engineHandle.tileDiagonalWall[sector][tile] = 0;
                engineHandle.tileRoofType[sector][tile] = 0;
                engineHandle.tileGroundOverlay[sector][tile] = 0;

                if (height == 0)
                {
                    engineHandle.tileGroundOverlay[sector][tile] = -6;
                }

                if (height == 3)
                {
                    engineHandle.tileGroundOverlay[sector][tile] = 8;
                }

                engineHandle.tileObjectRotation[sector][tile] = 0;
            }
        }

        private static sbyte[] LoadMapFile(string fileName)
        {
            string filePath = System.IO.Path.Combine(ApplicationPaths.MapsDirectory, fileName);

            if (!File.Exists(filePath))
            {
                return null;
            }

            return (sbyte[])(Array)File.ReadAllBytes(filePath);
        }
    }
}
