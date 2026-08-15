namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImagePaletteConverter(GameImage gameImage)
    {
        internal static int TransparentColour => 0xff00ff;

        internal void ConvertToIndexed(int pictureIndex)
        {
            int pixelCount = gameImage.PictureWidth[pictureIndex] * gameImage.PictureHeight[pictureIndex];
            int[] sourceColours = gameImage.PictureColours[pictureIndex];
            GameImageIndexedPalette indexedPalette =
                GameImageIndexedPaletteBuilder.Build(
                sourceColours,
                pixelCount);

            gameImage.PictureColourIndexes[pictureIndex] =
                indexedPalette.ColourIndexes;
            gameImage.PictureColour[pictureIndex] = indexedPalette.Colours;
            gameImage.PictureColours[pictureIndex] = null;
        }

        internal void ConvertToDirect(int pictureIndex)
        {
            if (gameImage.PictureColourIndexes[pictureIndex] is null)
            {
                return;
            }

            int pixelCount = gameImage.PictureWidth[pictureIndex] * gameImage.PictureHeight[pictureIndex];
            sbyte[] colourIndexBuffer = gameImage.PictureColourIndexes[pictureIndex];
            int[] palette = gameImage.PictureColour[pictureIndex];
            int[] resolvedColours = GameImageDirectColourResolver.Resolve(
                colourIndexBuffer,
                palette,
                pixelCount);

            gameImage.PictureColours[pictureIndex] = resolvedColours;
            gameImage.PictureColourIndexes[pictureIndex] = null;
            gameImage.PictureColour[pictureIndex] = null;
        }

    }
}