namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageCharacterColourRenderer(GameImage gameImage)
    {
        private static int DefaultColour => 0xffffff;

        internal void Draw(
            int pictureIndex,
            int primaryColour,
            int secondaryColour,
            GameImageCharacterClip characterClip)
        {
            if (secondaryColour == DefaultColour)
            {
                GameImageTintedSampledPixelComposer pixelComposer =
                    new(primaryColour);
                Draw(
                    pictureIndex,
                    characterClip,
                    pixelComposer);

                return;
            }

            GameImageDualTintedSampledPixelComposer dualPixelComposer =
                new(primaryColour, secondaryColour);
            Draw(
                pictureIndex,
                characterClip,
                dualPixelComposer);
        }

        private void Draw<TPixelComposer>(
            int pictureIndex,
            GameImageCharacterClip characterClip,
            TPixelComposer pixelComposer)
            where TPixelComposer : struct, IGameImageSampledPixelComposer
        {
            int[] colours = gameImage.PictureColours[pictureIndex];

            if (colours is not null)
            {
                GameImageScaledSpriteBlitter.Draw(
                    gameImage.Pixels,
                    new GameImageDirectSpriteColourSource(colours),
                    characterClip.SourceX,
                    characterClip.SourceY,
                    characterClip.DestinationOffset,
                    characterClip.Width,
                    characterClip.Height,
                    characterClip.ScaleX,
                    characterClip.ScaleY,
                    characterClip.SourceWidth,
                    characterClip.XPosition,
                    characterClip.XStep,
                    characterClip.ScanlineMode,
                    gameImage.ImageX,
                    gameImage.ImageWidth,
                    gameImage.GameWidth,
                    pixelComposer);

                return;
            }

            GameImageScaledSpriteBlitter.Draw(
                gameImage.Pixels,
                new GameImageIndexedSpriteColourSource(
                    gameImage.PictureColourIndexes[pictureIndex],
                    gameImage.PictureColour[pictureIndex]),
                characterClip.SourceX,
                characterClip.SourceY,
                characterClip.DestinationOffset,
                characterClip.Width,
                characterClip.Height,
                characterClip.ScaleX,
                characterClip.ScaleY,
                characterClip.SourceWidth,
                characterClip.XPosition,
                characterClip.XStep,
                characterClip.ScanlineMode,
                gameImage.ImageX,
                gameImage.ImageWidth,
                gameImage.GameWidth,
                pixelComposer);
        }
    }
}