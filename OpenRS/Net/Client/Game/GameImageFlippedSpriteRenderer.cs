namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageFlippedSpriteRenderer(GameImage gameImage)
    {
        internal void Draw(
            int x,
            int y,
            int width,
            int height,
            int pictureIndex,
            int colourTint,
            bool isColourShifted)
        {
            GameImageEntityClip entityClip = GameImageEntityClip.Calculate(
                gameImage,
                x,
                y,
                width,
                height,
                pictureIndex);

            if (isColourShifted)
            {
                GameImageSpriteBlitter.DrawSpriteFlippedColorShifted(
                    gameImage.Pixels,
                    gameImage.PictureColours[pictureIndex],
                    entityClip.SourceX,
                    entityClip.SourceY,
                    entityClip.DestinationOffset,
                    entityClip.DestinationRowStride,
                    entityClip.Width,
                    entityClip.Height,
                    entityClip.ScaleX,
                    entityClip.ScaleY,
                    entityClip.SourceWidth,
                    entityClip.RowStep,
                    colourTint);

                return;
            }

            GameImageSpriteBlitter.DrawSpriteFlipped(
                gameImage.Pixels,
                gameImage.PictureColours[pictureIndex],
                entityClip.SourceX,
                entityClip.SourceY,
                entityClip.DestinationOffset,
                entityClip.DestinationRowStride,
                entityClip.Width,
                entityClip.Height,
                entityClip.ScaleX,
                entityClip.ScaleY,
                entityClip.SourceWidth,
                entityClip.RowStep,
                colourTint);
        }
    }
}