namespace OpenRS.Net.Client.Game
{
    internal sealed class GroundTileTextureResolver
    {
        private readonly EngineHandle engineHandle;

        private static int WaterTileType => 4;

        private static int DiagonalTransitionTileType => 5;

        private static int TransparentTileType => 2;

        private static int RoofTileType => 3;

        private static int SpecialWaterTileIndex => 12;

        private static int WaterDefaultTexture => 1;

        private static int WaterSpecialTexture => 31;

        private static int DiagonalWallMaxIndex => 24000;

        private static int UnknownTileFlag => 0x40;

        private static int TransparentTileFlag => 0x80;

        private static int FirstUpperFloorHeight => 1;

        private static int SecondUpperFloorHeight => 2;

        private static int DefaultTriangleOrientation => 0;

        private static int AlternateTriangleOrientation => 1;

        internal GroundTileTextureResolver(EngineHandle engineHandle)
        {
            this.engineHandle = engineHandle;
        }

        internal TileTextureResult InitialiseGroundTextureResult(
            int tileX,
            int tileY,
            int height)
        {
            int textureIndex = engineHandle.GetTileGroundTextureIndex(tileX, tileY);
            int defaultTexture = engineHandle.GroundTexture[textureIndex];

            if (IsUpperFloor(height))
            {
                defaultTexture = EngineHandle.EmptyTileColour;
            }

            return CreateUniformTextureResult(defaultTexture);
        }

        internal TileTextureResult ResolveOverlayTextures(
            int tileX,
            int tileY,
            int height,
            int defaultTexture)
        {
            int tileIndex = engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height);
            int tileType = GetOverlayTileType(tileIndex);
            int tileColour = GetOverlayTileColour(tileIndex);

            if (tileType == WaterTileType)
            {
                return ResolveWaterTextures(tileIndex);
            }

            TileTextureResult result = CreateUniformTextureResult(tileColour);

            if (tileType == DiagonalTransitionTileType)
            {
                if (!IsDiagonalWallPresent(tileX, tileY))
                {
                    return result;
                }

                return ResolveDiagonalTransitionTextures(
                    tileX,
                    tileY,
                    height,
                    defaultTexture,
                    tileColour);
            }

            if (tileType != TransparentTileType || IsDiagonalWallPresent(tileX, tileY))
            {
                int elevationMinimum = engineHandle.GetElevationMinimum(tileX, tileY, height);

                return ResolveElevationTextures(
                    tileX,
                    tileY,
                    height,
                    defaultTexture,
                    elevationMinimum,
                    tileColour);
            }

            return result;
        }

        internal void ApplyOverlayTileFlags(int tileX, int tileY, int overlayIndex)
        {
            if (HasUnknownOverlayFlag(overlayIndex))
            {
                engineHandle.Tiles[tileX][tileY] |= UnknownTileFlag;
            }

            if (GetOverlayTileType(overlayIndex) == TransparentTileType)
            {
                engineHandle.Tiles[tileX][tileY] |= TransparentTileFlag;
            }
        }

        internal bool IsWaterOverlay(int tileX, int tileY, int height)
        {
            int overlayIndex = engineHandle.GetTileGroundOverlayIndex(tileX, tileY, height);

            return overlayIndex > 0 && IsWaterOverlay(overlayIndex);
        }

        internal bool IsWaterOverlay(int overlayIndex)
            => GetOverlayTileType(overlayIndex) == WaterTileType;

        internal bool IsRoofOverlay(int overlayIndex)
            => GetOverlayTileType(overlayIndex) == RoofTileType;

        internal int GetOverlayTileColour(int overlayIndex)
            => engineHandle.entityManager.GetTile(overlayIndex - 1).Colour;

        private TileTextureResult ResolveWaterTextures(int tileIndex)
        {
            int primaryTexture = WaterDefaultTexture;
            int secondaryTexture = WaterDefaultTexture;

            if (tileIndex == SpecialWaterTileIndex)
            {
                primaryTexture = WaterSpecialTexture;
                secondaryTexture = WaterSpecialTexture;
            }

            return new()
            {
                PrimaryTexture = primaryTexture,
                SecondaryTexture = secondaryTexture,
                TriangleOrientation = DefaultTriangleOrientation,
            };
        }

        private TileTextureResult ResolveDiagonalTransitionTextures(
            int tileX,
            int tileY,
            int height,
            int defaultTexture,
            int tileColour)
        {
            TileTextureResult result = CreateUniformTextureResult(tileColour);

            int leftTexture = engineHandle.GetTileGroundOverlayTextureOrDefault(
                tileX - 1,
                tileY,
                height,
                defaultTexture);
            int rightTexture = engineHandle.GetTileGroundOverlayTextureOrDefault(
                tileX + 1,
                tileY,
                height,
                defaultTexture);
            int upTexture = engineHandle.GetTileGroundOverlayTextureOrDefault(
                tileX,
                tileY - 1,
                height,
                defaultTexture);
            int downTexture = engineHandle.GetTileGroundOverlayTextureOrDefault(
                tileX,
                tileY + 1,
                height,
                defaultTexture);

            if (leftTexture != EngineHandle.EmptyTileColour &&
                upTexture != EngineHandle.EmptyTileColour)
            {
                result.PrimaryTexture = leftTexture;

                return result;
            }

            if (rightTexture != EngineHandle.EmptyTileColour &&
                downTexture != EngineHandle.EmptyTileColour)
            {
                result.SecondaryTexture = rightTexture;

                return result;
            }

            if (rightTexture != EngineHandle.EmptyTileColour &&
                upTexture != EngineHandle.EmptyTileColour)
            {
                result.SecondaryTexture = rightTexture;

                return SetTriangleOrientation(
                    result,
                    AlternateTriangleOrientation);
            }

            if (leftTexture != EngineHandle.EmptyTileColour &&
                downTexture != EngineHandle.EmptyTileColour)
            {
                result.PrimaryTexture = leftTexture;

                return SetTriangleOrientation(
                    result,
                    AlternateTriangleOrientation);
            }

            return result;
        }

        private TileTextureResult ResolveElevationTextures(
            int tileX,
            int tileY,
            int height,
            int defaultTexture,
            int elevationMinimum,
            int tileColour)
        {
            TileTextureResult result = CreateUniformTextureResult(tileColour);

            if (engineHandle.GetElevationMinimum(tileX - 1, tileY, height) != elevationMinimum &&
                engineHandle.GetElevationMinimum(tileX, tileY - 1, height) != elevationMinimum)
            {
                result.PrimaryTexture = defaultTexture;

                return result;
            }

            if (engineHandle.GetElevationMinimum(tileX + 1, tileY, height) != elevationMinimum &&
                engineHandle.GetElevationMinimum(tileX, tileY + 1, height) != elevationMinimum)
            {
                result.SecondaryTexture = defaultTexture;

                return result;
            }

            if (engineHandle.GetElevationMinimum(tileX + 1, tileY, height) != elevationMinimum &&
                engineHandle.GetElevationMinimum(tileX, tileY - 1, height) != elevationMinimum)
            {
                result.SecondaryTexture = defaultTexture;

                return SetTriangleOrientation(
                    result,
                    AlternateTriangleOrientation);
            }

            if (engineHandle.GetElevationMinimum(tileX - 1, tileY, height) != elevationMinimum &&
                engineHandle.GetElevationMinimum(tileX, tileY + 1, height) != elevationMinimum)
            {
                result.PrimaryTexture = defaultTexture;

                return SetTriangleOrientation(
                    result,
                    AlternateTriangleOrientation);
            }

            return result;
        }

        private bool IsDiagonalWallPresent(int tileX, int tileY)
        {
            int diagonalWall = engineHandle.GetDiagonalWall(tileX, tileY);

            return diagonalWall > 0 && diagonalWall < DiagonalWallMaxIndex;
        }

        private bool IsUpperFloor(int height)
            => height == FirstUpperFloorHeight || height == SecondUpperFloorHeight;

        private TileTextureResult CreateUniformTextureResult(int texture) => new()
        {
            PrimaryTexture = texture,
            SecondaryTexture = texture,
            TriangleOrientation = DefaultTriangleOrientation,
        };

        private TileTextureResult SetTriangleOrientation(
            TileTextureResult result,
            int triangleOrientation)
        {
            result.TriangleOrientation = triangleOrientation;

            return result;
        }

        private int GetOverlayTileType(int overlayIndex)
            => engineHandle.entityManager.GetTile(overlayIndex - 1).Type;

        private bool HasUnknownOverlayFlag(int overlayIndex)
            => engineHandle.entityManager.GetTile(overlayIndex - 1).Unknown != 0;
    }
}