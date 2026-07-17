namespace OpenRS.Net.Client.Game
{
    internal sealed class WorldObjectManipulator
    {
        private readonly EngineHandle engineHandle;

        internal WorldObjectManipulator(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal void AddObjectToScene(int x, int y, int objWidth, int objHeight)
        {
            if (x < 1 || y < 1 || x + objWidth >= EngineHandle.GridSize || y + objHeight >= EngineHandle.GridSize)
            {
                return;
            }

            for (int tileX = x; tileX <= x + objWidth; tileX += 1)
            {
                for (int tileY = y; tileY <= y + objHeight; tileY += 1)
                {
                    if ((engineHandle.GetTile(tileX, tileY) & 0x63) != 0 ||
                        (engineHandle.GetTile(tileX - 1, tileY) & 0x59) != 0 ||
                        (engineHandle.GetTile(tileX, tileY - 1) & 0x56) != 0 ||
                        (engineHandle.GetTile(tileX - 1, tileY - 1) & 0x6c) != 0)
                    {
                        engineHandle.SetTileFlags(tileX, tileY, 35);
                    }
                    else
                    {
                        engineHandle.SetTileFlags(tileX, tileY, 0);
                    }
                }
            }
        }

        internal void RemoveWallObject(int x, int y, int wallDirection, int index)
        {
            if (x < 0 || y < 0 || x >= EngineHandle.GridSize - 1 || y >= EngineHandle.GridSize - 1)
            {
                return;
            }

            if (engineHandle.entityManager.GetWallObject(index).Type != 1)
            {
                return;
            }

            if (wallDirection == 0)
            {
                engineHandle.tiles[x][y] &= 0xfffe;

                if (y > 0)
                {
                    engineHandle.DrawObjectSprite(x, y - 1, 4);
                }
            }
            else if (wallDirection == 1)
            {
                engineHandle.tiles[x][y] &= 0xfffd;

                if (x > 0)
                {
                    engineHandle.DrawObjectSprite(x - 1, y, 8);
                }
            }
            else if (wallDirection == 2)
            {
                engineHandle.tiles[x][y] &= 0xffef;
            }
            else if (wallDirection == 3)
            {
                engineHandle.tiles[x][y] &= 0xffdf;
            }

            AddObjectToScene(x, y, 1, 1);
        }

        internal void CreateWall(int x, int y, int wallDirection, int index)
        {
            if (x < 0 || y < 0 || x >= EngineHandle.GridSize - 1 || y >= EngineHandle.GridSize - 1)
            {
                return;
            }

            if (engineHandle.entityManager.GetWallObject(index).Type != 1)
            {
                return;
            }

            if (wallDirection == 0)
            {
                engineHandle.tiles[x][y] |= 1;

                if (y > 0)
                {
                    engineHandle.SetTileFlags(x, y - 1, 4);
                }
            }
            else if (wallDirection == 1)
            {
                engineHandle.tiles[x][y] |= 2;

                if (x > 0)
                {
                    engineHandle.SetTileFlags(x - 1, y, 8);
                }
            }
            else if (wallDirection == 2)
            {
                engineHandle.tiles[x][y] |= 0x10;
            }
            else if (wallDirection == 3)
            {
                engineHandle.tiles[x][y] |= 0x20;
            }

            AddObjectToScene(x, y, 1, 1);
        }

        internal void RegisterObjectDir(int x, int y, int dir)
        {
            if (x < 0 || x >= EngineHandle.GridSize || y < 0 || y >= EngineHandle.GridSize)
            {
                return;
            }

            engineHandle.objectDirs[x][y] = dir;
        }

        internal void RemoveObject(int x, int y, int objType, int objDir)
        {
            if (x < 0 || y < 0 || x >= EngineHandle.GridSize - 1 || y >= EngineHandle.GridSize - 1)
            {
                return;
            }

            if (engineHandle.entityManager.GetWorldObject(objType).Type != 1 &&
                engineHandle.entityManager.GetWorldObject(objType).Type != 2)
            {
                return;
            }

            int objWidth;
            int objHeight;

            if (objDir == 0 || objDir == 4)
            {
                objWidth = engineHandle.entityManager.GetWorldObject(objType).Width;
                objHeight = engineHandle.entityManager.GetWorldObject(objType).Height;
            }
            else
            {
                objHeight = engineHandle.entityManager.GetWorldObject(objType).Width;
                objWidth = engineHandle.entityManager.GetWorldObject(objType).Height;
            }

            for (int tileX = x; tileX < x + objWidth; tileX += 1)
            {
                for (int tileY = y; tileY < y + objHeight; tileY += 1)
                {
                    if (engineHandle.entityManager.GetWorldObject(objType).Type == 1)
                    {
                        engineHandle.tiles[tileX][tileY] &= 0xffbf;
                    }
                    else if (objDir == 0)
                    {
                        engineHandle.tiles[tileX][tileY] &= 0xfffd;

                        if (tileX > 0)
                        {
                            engineHandle.DrawObjectSprite(tileX - 1, tileY, 8);
                        }
                    }
                    else if (objDir == 2)
                    {
                        engineHandle.tiles[tileX][tileY] &= 0xfffb;

                        if (tileY < EngineHandle.GridSize - 1)
                        {
                            engineHandle.DrawObjectSprite(tileX, tileY + 1, 1);
                        }
                    }
                    else if (objDir == 4)
                    {
                        engineHandle.tiles[tileX][tileY] &= 0xfff7;

                        if (tileX < EngineHandle.GridSize - 1)
                        {
                            engineHandle.DrawObjectSprite(tileX + 1, tileY, 2);
                        }
                    }
                    else if (objDir == 6)
                    {
                        engineHandle.tiles[tileX][tileY] &= 0xfffe;

                        if (tileY > 0)
                        {
                            engineHandle.DrawObjectSprite(tileX, tileY - 1, 4);
                        }
                    }
                }
            }

            AddObjectToScene(x, y, objWidth, objHeight);
        }

        internal void CreateObject(int x, int y, int index, int direction)
        {
            if (x < 0 || y < 0 || x >= EngineHandle.GridSize - 1 || y >= EngineHandle.GridSize - 1)
            {
                return;
            }

            if (engineHandle.entityManager.GetWorldObject(index).Type != 1 &&
                engineHandle.entityManager.GetWorldObject(index).Type != 2)
            {
                return;
            }

            int objectWidth;
            int objectHeight;

            if (direction == 0 || direction == 4)
            {
                objectWidth = engineHandle.entityManager.GetWorldObject(index).Width;
                objectHeight = engineHandle.entityManager.GetWorldObject(index).Height;
            }
            else
            {
                objectHeight = engineHandle.entityManager.GetWorldObject(index).Width;
                objectWidth = engineHandle.entityManager.GetWorldObject(index).Height;
            }

            for (int tileX = x; tileX < x + objectWidth; tileX += 1)
            {
                for (int tileY = y; tileY < y + objectHeight; tileY += 1)
                {
                    if (engineHandle.entityManager.GetWorldObject(index).Type == 1)
                    {
                        engineHandle.tiles[tileX][tileY] |= 0x40;
                    }
                    else if (direction == 0)
                    {
                        engineHandle.tiles[tileX][tileY] |= 2;

                        if (tileX > 0)
                        {
                            engineHandle.SetTileFlags(tileX - 1, tileY, 8);
                        }
                    }
                    else if (direction == 2)
                    {
                        engineHandle.tiles[tileX][tileY] |= 4;

                        if (tileY < EngineHandle.GridSize - 1)
                        {
                            engineHandle.SetTileFlags(tileX, tileY + 1, 1);
                        }
                    }
                    else if (direction == 4)
                    {
                        engineHandle.tiles[tileX][tileY] |= 8;

                        if (tileX < EngineHandle.GridSize - 1)
                        {
                            engineHandle.SetTileFlags(tileX + 1, tileY, 2);
                        }
                    }
                    else if (direction == 6)
                    {
                        engineHandle.tiles[tileX][tileY] |= 1;

                        if (tileY > 0)
                        {
                            engineHandle.SetTileFlags(tileX, tileY - 1, 4);
                        }
                    }
                }
            }

            AddObjectToScene(x, y, objectWidth, objectHeight);
        }

        internal void AddObjects(GameObject[] tileModels)
        {
            for (int x = 0; x < EngineHandle.GridSize - 2; x += 1)
            {
                for (int y = 0; y < EngineHandle.GridSize - 2; y += 1)
                {
                    if (engineHandle.GetDiagonalWall(x, y) <= EngineHandle.LocationEntityBase ||
                        engineHandle.GetDiagonalWall(x, y) >= 60000)
                    {
                        continue;
                    }

                    try
                    {
                        int objectIndex = engineHandle.GetDiagonalWall(x, y) - EngineHandle.LocationEntityBase - 1;
                        int objectRotation = engineHandle.objectDirs[x][y];
                        int objectWidth;
                        int objectHeight;

                        if (objectRotation == 0 || objectRotation == 4)
                        {
                            objectWidth = engineHandle.entityManager.GetWorldObject(objectIndex).Width;
                            objectHeight = engineHandle.entityManager.GetWorldObject(objectIndex).Height;
                        }
                        else
                        {
                            objectHeight = engineHandle.entityManager.GetWorldObject(objectIndex).Width;
                            objectWidth = engineHandle.entityManager.GetWorldObject(objectIndex).Height;
                        }

                        CreateObject(x, y, objectIndex, objectRotation);

                        GameObject objectModel = tileModels[engineHandle.entityManager.GetWorldObject(objectIndex).ModelIndex].CreateParent(false, true, false, false);
                        int worldCentreX = (x + x + objectWidth) * EngineHandle.TileWorldSize / 2;
                        int worldCentreZ = (y + y + objectHeight) * EngineHandle.TileWorldSize / 2;
                        objectModel.OffsetPosition(worldCentreX, -engineHandle.GetAveragedElevation(worldCentreX, worldCentreZ), worldCentreZ);
                        objectModel.SetRotation(0, engineHandle.GetTileRotation(x, y) * 32, 0);
                        objectModel.SetRotation(0, objectRotation * 32, 0);
                        engineHandle.camera.AddModel(objectModel);
                        objectModel.SetModelColours(48, 48, -50, -10, -50);

                        if (objectWidth <= 1 && objectHeight <= 1)
                        {
                            continue;
                        }

                        for (int occupiedX = x; occupiedX < x + objectWidth; occupiedX += 1)
                        {
                            for (int occupiedY = y; occupiedY < y + objectHeight; occupiedY += 1)
                            {
                                if ((occupiedX <= x && occupiedY <= y) ||
                                    engineHandle.GetDiagonalWall(occupiedX, occupiedY) - EngineHandle.LocationEntityBase - 1 != objectIndex)
                                {
                                    continue;
                                }

                                int sectorX = occupiedX;
                                int sectorY = occupiedY;
                                byte sectorLayer = 0;

                                if (sectorX >= EngineHandle.SectorSize && sectorY < EngineHandle.SectorSize)
                                {
                                    sectorLayer = 1;
                                    sectorX -= EngineHandle.SectorSize;
                                }
                                else if (sectorX < EngineHandle.SectorSize && sectorY >= EngineHandle.SectorSize)
                                {
                                    sectorLayer = 2;
                                    sectorY -= EngineHandle.SectorSize;
                                }
                                else if (sectorX >= EngineHandle.SectorSize && sectorY >= EngineHandle.SectorSize)
                                {
                                    sectorLayer = 3;
                                    sectorX -= EngineHandle.SectorSize;
                                    sectorY -= EngineHandle.SectorSize;
                                }

                                engineHandle.tileDiagonalWall[sectorLayer][sectorX * EngineHandle.SectorSize + sectorY] = 0;
                            }
                        }
                    }
                    catch { }
                }
            }
        }
    }
}
