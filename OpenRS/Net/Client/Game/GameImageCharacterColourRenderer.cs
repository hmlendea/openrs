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
                DrawPrimaryColour(pictureIndex, primaryColour, characterClip);

                return;
            }

            DrawPrimaryAndSecondaryColours(
                pictureIndex,
                primaryColour,
                secondaryColour,
                characterClip);
        }

        private void DrawPrimaryColour(
            int pictureIndex,
            int primaryColour,
            GameImageCharacterClip characterClip)
        {
            if (gameImage.PictureColours[pictureIndex] is not null)
            {
                GameImageScaledSpriteBlitter.DrawSpriteFlatShaded(
                    gameImage.Pixels,
                    gameImage.PictureColours[pictureIndex],
                    0,
                    characterClip.SourceX,
                    characterClip.SourceY,
                    characterClip.DestinationOffset,
                    characterClip.Width,
                    characterClip.Height,
                    characterClip.ScaleX,
                    characterClip.ScaleY,
                    characterClip.SourceWidth,
                    primaryColour,
                    characterClip.XPosition,
                    characterClip.XStep,
                    characterClip.ScanlineMode,
                    gameImage.ImageX,
                    gameImage.ImageWidth,
                    gameImage.GameWidth);

                return;
            }

            GameImageScaledSpriteBlitter.DrawSpriteFlatShadedTextured(
                gameImage.Pixels,
                gameImage.PictureColourIndexes[pictureIndex],
                gameImage.PictureColour[pictureIndex],
                0,
                characterClip.SourceX,
                characterClip.SourceY,
                characterClip.DestinationOffset,
                characterClip.Width,
                characterClip.Height,
                characterClip.ScaleX,
                characterClip.ScaleY,
                characterClip.SourceWidth,
                primaryColour,
                characterClip.XPosition,
                characterClip.XStep,
                characterClip.ScanlineMode,
                gameImage.ImageX,
                gameImage.ImageWidth,
                gameImage.GameWidth);
        }

        private void DrawPrimaryAndSecondaryColours(
            int pictureIndex,
            int primaryColour,
            int secondaryColour,
            GameImageCharacterClip characterClip)
        {
            if (gameImage.PictureColours[pictureIndex] is not null)
            {
                GameImageScaledSpriteBlitter.DrawSpriteFlatShadedAlt(
                    gameImage.Pixels,
                    gameImage.PictureColours[pictureIndex],
                    0,
                    characterClip.SourceX,
                    characterClip.SourceY,
                    characterClip.DestinationOffset,
                    characterClip.Width,
                    characterClip.Height,
                    characterClip.ScaleX,
                    characterClip.ScaleY,
                    characterClip.SourceWidth,
                    primaryColour,
                    secondaryColour,
                    characterClip.XPosition,
                    characterClip.XStep,
                    characterClip.ScanlineMode,
                    gameImage.ImageX,
                    gameImage.ImageWidth,
                    gameImage.GameWidth);

                return;
            }

            GameImageScaledSpriteBlitter.DrawSpriteFlatShadedTexturedAlt(
                gameImage.Pixels,
                gameImage.PictureColourIndexes[pictureIndex],
                gameImage.PictureColour[pictureIndex],
                0,
                characterClip.SourceX,
                characterClip.SourceY,
                characterClip.DestinationOffset,
                characterClip.Width,
                characterClip.Height,
                characterClip.ScaleX,
                characterClip.ScaleY,
                characterClip.SourceWidth,
                primaryColour,
                secondaryColour,
                characterClip.XPosition,
                characterClip.XStep,
                characterClip.ScanlineMode,
                gameImage.ImageX,
                gameImage.ImageWidth,
                gameImage.GameWidth);
        }
    }
}