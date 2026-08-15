using System;

using NuciLog.Core;

using OpenRS.Logging;

namespace OpenRS.Net.Client.Game
{
    internal sealed class GameImageSpriteBlitter
    {
        private static readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImageSpriteBlitter>();

        private static string TransparentSpriteRenderingFailureMessage
            => "The transparent sprite rendering has failed.";

        private static string FlippedSpriteRenderingFailureMessage
            => "The flipped sprite rendering has failed.";

        private static string ColourShiftedFlippedSpriteRenderingFailureMessage
            => "The colour-shifted flipped sprite rendering has failed.";

        internal static void DrawSpriteOpaque(
            int[] pixels,
            int[] colours,
            int srcOffset,
            int dstOffset,
            int width,
            int height,
            int dstStride,
            int srcStride,
            int rowStep)
            => DrawSpriteOpaque(
                pixels,
                new GameImageDirectSpriteColourSource(colours),
                srcOffset,
                dstOffset,
                width,
                height,
                dstStride,
                srcStride,
                rowStep);

        private static void DrawSpriteOpaque<TColourSource>(
            int[] pixels,
            TColourSource colourSource,
            int sourceOffset,
            int destinationOffset,
            int width,
            int height,
            int destinationStride,
            int sourceStride,
            int rowStep)
            where TColourSource : struct, IGameImageSpriteColourSource
        {
            int groupCount = -(width >> 2);
            int remainderCount = -(width & 3);

            for (int rowIteration = -height;
                rowIteration < 0;
                rowIteration += rowStep)
            {
                for (int groupIndex = groupCount;
                    groupIndex < 0;
                    groupIndex += 1)
                {
                    GameImageSpriteSample sample =
                        colourSource.Read(sourceOffset++);
                    DrawSample(pixels, sample, destinationOffset++);
                    sample = colourSource.Read(sourceOffset++);
                    DrawSample(pixels, sample, destinationOffset++);
                    sample = colourSource.Read(sourceOffset++);
                    DrawSample(pixels, sample, destinationOffset++);
                    sample = colourSource.Read(sourceOffset++);
                    DrawSample(pixels, sample, destinationOffset++);
                }

                for (int remainderIndex = remainderCount;
                    remainderIndex < 0;
                    remainderIndex += 1)
                {
                    GameImageSpriteSample sample =
                        colourSource.Read(sourceOffset++);
                    DrawSample(pixels, sample, destinationOffset++);
                }

                destinationOffset += destinationStride;
                sourceOffset += sourceStride;
            }
        }

        private static void DrawSample(
            int[] pixels,
            GameImageSpriteSample sample,
            int destinationOffset)
        {
            if (sample.IsVisible)
            {
                pixels[destinationOffset] = sample.Colour;
            }
        }

        internal static void DrawSpriteTextured(
            int[] pixels,
            sbyte[] colourIndexes,
            int[] colourLookup,
            int srcOffset,
            int dstOffset,
            int width,
            int height,
            int dstStride,
            int srcStride,
            int rowStep)
            => DrawSpriteOpaque(
                pixels,
                new GameImageIndexedSpriteColourSource(
                    colourIndexes,
                    colourLookup),
                srcOffset,
                dstOffset,
                width,
                height,
                dstStride,
                srcStride,
                rowStep);

        internal static void DrawSpriteTransparent(
            int[] pixels,
            int[] colours,
            int srcX,
            int srcY,
            int dstOffset,
            int dstStride,
            int width,
            int height,
            int xStep,
            int yStep,
            int srcWidth,
            int rowStep)
        {
            GameImageOpaqueSampledPixelComposer pixelComposer = new();

            DrawSampledSprite(
                pixels,
                colours,
                srcX,
                srcY,
                dstOffset,
                dstStride,
                width,
                height,
                xStep,
                yStep,
                srcWidth,
                rowStep,
                pixelComposer,
                TransparentSpriteRenderingFailureMessage);
        }

        internal static void DrawSpriteColorShifted(
            int[] pixels,
            int[] colours,
            int srcOffset,
            int dstOffset,
            int width,
            int height,
            int dstStride,
            int srcStride,
            int rowStep,
            int blendFactor)
            => DrawSpriteColorShifted(
                pixels,
                new GameImageDirectSpriteColourSource(colours),
                srcOffset,
                dstOffset,
                width,
                height,
                dstStride,
                srcStride,
                rowStep,
                blendFactor);

        private static void DrawSpriteColorShifted<TColourSource>(
            int[] pixels,
            TColourSource colourSource,
            int sourceOffset,
            int destinationOffset,
            int width,
            int height,
            int destinationStride,
            int sourceStride,
            int rowStep,
            int blendFactor)
            where TColourSource : struct, IGameImageSpriteColourSource
        {
            int blendComplement = 256 - blendFactor;

            for (int rowIteration = -height;
                rowIteration < 0;
                rowIteration += rowStep)
            {
                for (int columnIteration = -width;
                    columnIteration < 0;
                    columnIteration += 1)
                {
                    GameImageSpriteSample sample =
                        colourSource.Read(sourceOffset++);

                    if (sample.IsVisible)
                    {
                        pixels[destinationOffset] = GameImageColourBlender.Blend(
                            sample.Colour,
                            pixels[destinationOffset],
                            blendFactor,
                            blendComplement);
                    }

                    destinationOffset += 1;
                }

                destinationOffset += destinationStride;
                sourceOffset += sourceStride;
            }
        }

        internal static void DrawSpriteColorShiftedTextured(
            int[] pixels,
            sbyte[] colourIndexes,
            int[] colourLookup,
            int srcOffset,
            int dstOffset,
            int width,
            int height,
            int dstStride,
            int srcStride,
            int rowStep,
            int blendFactor)
            => DrawSpriteColorShifted(
                pixels,
                new GameImageIndexedSpriteColourSource(
                    colourIndexes,
                    colourLookup),
                srcOffset,
                dstOffset,
                width,
                height,
                dstStride,
                srcStride,
                rowStep,
                blendFactor);

        internal static void DrawSpriteFlipped(
            int[] pixels,
            int[] colours,
            int srcX,
            int srcY,
            int dstOffset,
            int dstStride,
            int width,
            int height,
            int xStep,
            int yStep,
            int srcWidth,
            int rowStep,
            int blendFactor)
        {
            int blendComplement = 256 - blendFactor;
            GameImageBlendedSampledPixelComposer pixelComposer =
                new(blendFactor, blendComplement);

            DrawSampledSprite(
                pixels,
                colours,
                srcX,
                srcY,
                dstOffset,
                dstStride,
                width,
                height,
                xStep,
                yStep,
                srcWidth,
                rowStep,
                pixelComposer,
                FlippedSpriteRenderingFailureMessage);
        }

        internal static void DrawSpriteFlippedColorShifted(
            int[] pixels,
            int[] colours,
            int srcX,
            int srcY,
            int dstOffset,
            int dstStride,
            int width,
            int height,
            int xStep,
            int yStep,
            int srcWidth,
            int rowStep,
            int colour)
        {
            GameImageTintedSampledPixelComposer pixelComposer = new(colour);

            DrawSampledSprite(
                pixels,
                colours,
                srcX,
                srcY,
                dstOffset,
                dstStride,
                width,
                height,
                xStep,
                yStep,
                srcWidth,
                rowStep,
                pixelComposer,
                ColourShiftedFlippedSpriteRenderingFailureMessage);
        }

        internal static void DrawSpriteAlpha(
            int[] pixels,
            int[] colours,
            int dstOffset,
            int srcX,
            int srcY,
            int xStep,
            int yStep,
            int count,
            int srcWidth)
            => DrawSpriteAlpha(
                pixels,
                colours,
                dstOffset,
                srcX,
                srcY,
                xStep,
                yStep,
                count,
                srcWidth,
                new GameImageOpaqueSampledPixelWriter());

        internal static void DrawSpriteAlphaColorShifted(
            int[] pixels,
            int[] colours,
            int dstOffset,
            int srcX,
            int srcY,
            int xStep,
            int yStep,
            int count,
            int srcWidth)
            => DrawSpriteAlpha(
                pixels,
                colours,
                dstOffset,
                srcX,
                srcY,
                xStep,
                yStep,
                count,
                srcWidth,
                new GameImageTransparentSampledPixelWriter());

        private static void DrawSpriteAlpha<TPixelWriter>(
            int[] pixels,
            int[] colours,
            int destinationOffset,
            int sourceX,
            int sourceY,
            int xStep,
            int yStep,
            int count,
            int sourceWidth,
            TPixelWriter pixelWriter)
            where TPixelWriter : struct, IGameImageSampledPixelWriter
        {
            for (int pixelIndex = count; pixelIndex < 0; pixelIndex += 1)
            {
                int sourceColour =
                    colours[(sourceX >> 17) + (sourceY >> 17) * sourceWidth];
                pixelWriter.Write(
                    sourceColour,
                    pixels,
                    destinationOffset);
                destinationOffset += 1;
                sourceX += xStep;
                sourceY += yStep;
            }
        }

        private static void DrawSampledSprite<TPixelComposer>(
            int[] pixels,
            int[] colours,
            int sourceX,
            int sourceY,
            int destinationOffset,
            int destinationStride,
            int width,
            int height,
            int xStep,
            int yStep,
            int sourceWidth,
            int rowStep,
            TPixelComposer pixelComposer,
            string failureMessage)
            where TPixelComposer : struct, IGameImageSampledPixelComposer
        {
            try
            {
                GameImageSampledSpriteBlitter.Draw(
                    pixels,
                    colours,
                    sourceX,
                    sourceY,
                    destinationOffset,
                    destinationStride,
                    width,
                    height,
                    xStep,
                    yStep,
                    sourceWidth,
                    rowStep,
                    pixelComposer);
            }
            catch (Exception exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    failureMessage,
                    exception);

                throw;
            }
        }
    }
}
