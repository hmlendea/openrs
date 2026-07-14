using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Primitives;

using NuciLog.Core;

using OpenRS.Logging;
using OpenRS.Net.Client.Data;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Game
{
    public sealed class GraphicsEngine
    {
        public Size2D GameSize { get; set; }

        private Rectangle2D imageRectangle;

        public GraphicsEngine(int width, int height, int size)
        {
            GameSize = new(width, height);

            imageRectangle = new Rectangle2D(0, 0, width, height);

            IsLoggedIn = false;
            pixels = new int[width * height];
            pictureColors = new int[size][];
            hasTransparentBackground = new bool[size];
            pictureColorIndexes = new sbyte[size][];
            pictureColor = new int[size][];
            pictureWidth = new int[size];
            pictureHeight = new int[size];
            pictureAssumedWidth = new int[size];
            pictureAssumedHeight = new int[size];
            pictureOffsetX = new int[size];
            pictureOffsetY = new int[size];

            if (width > 1 && height > 1)
            {
                for (int k = 0; k < GameSize.Area; k += 1)
                {
                    pixels[k] = 0;
                }
            }
        }

        static GraphicsEngine()
        {
            string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!\"!$%^&*()-_=+[{]};:'@#~,<.>/?\\| ";
            characterFontOffsetTable = new int[256];

            for (int i = 0; i < 256; i += 1)
            {
                int charIndex = s.IndexOf((char)i);

                if (charIndex == -1)
                {
                    charIndex = 74;
                }

                characterFontOffsetTable[i] = charIndex * 9;
            }
        }

        public void SetDimensions(int x, int y, int width, int height)
        {
            if (x < 0)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = 0;
            }

            if (width > GameSize.Width)
            {
                width = GameSize.Width;
            }

            if (height > GameSize.Height)
            {
                height = GameSize.Height;
            }

            imageRectangle = new Rectangle2D(x, y, width, height);
        }

        public void ResetDimensions() => imageRectangle = new Rectangle2D(0, 0, GameSize.Width, GameSize.Height);

        public void ClearScreen()
        {
            for (int i = 0; i < GameSize.Area; i += 1)
            {
                pixels[i] = 0;
            }
        }

        public void DrawCircle(int x, int y, int radius, int colour, int alpha)
        {
            int i = 256 - alpha;
            int k = (colour >> 16 & 0xff) * alpha;
            int l = (colour >> 8 & 0xff) * alpha;
            int i1 = (colour & 0xff) * alpha;
            int i2 = y - radius;

            if (i2 < 0)
            {
                i2 = 0;
            }

            int j2 = y + radius;

            if (j2 >= GameSize.Height)
            {
                j2 = GameSize.Height - 1;
            }

            byte byte0 = 1;

            for (int k2 = i2; k2 <= j2; k2 += byte0)
            {
                int l2 = k2 - y;
                int i3 = (int)Math.Sqrt(radius * radius - l2 * l2);
                int j3 = x - i3;

                if (j3 < 0)
                {
                    j3 = 0;
                }

                int k3 = x + i3;

                if (k3 >= GameSize.Width)
                {
                    k3 = GameSize.Width - 1;
                }

                int l3 = j3 + k2 * GameSize.Width;

                for (int i4 = j3; i4 <= k3; i4 += 1)
                {
                    int j1 = (pixels[l3] >> 16 & 0xff) * i;
                    int k1 = (pixels[l3] >> 8 & 0xff) * i;
                    int l1 = (pixels[l3] & 0xff) * i;
                    int j4 = ((k + j1 >> 8) << 16) + ((l + k1 >> 8) << 8) + (i1 + l1 >> 8);
                    pixels[l3++] = j4;
                }
            }
        }

        public void DrawBoxAlpha(int x, int y, int w, int h, int colour, int alpha)
        {
            if (x < imageRectangle.X)
            {
                w -= imageRectangle.X - x;
                x = imageRectangle.X;
            }
            if (y < imageRectangle.Y)
            {
                h -= imageRectangle.Y - y;
                y = imageRectangle.Y;
            }
            if (x + w > imageRectangle.Width)
            {
                w = imageRectangle.Width - x;
            }

            if (y + h > imageRectangle.Height)
            {
                h = imageRectangle.Height - y;
            }

            int i = 256 - alpha;
            int k = (colour >> 16 & 0xff) * alpha;
            int l = (colour >> 8 & 0xff) * alpha;
            int i1 = (colour & 0xff) * alpha;
            int i2 = GameSize.Width - w;
            byte byte0 = 1;

            int j2 = x + y * GameSize.Width;
            for (int k2 = 0; k2 < h; k2 += byte0)
            {
                for (int l2 = -w; l2 < 0; l2 += 1)
                {
                    int j1 = (pixels[j2] >> 16 & 0xff) * i;
                    int k1 = (pixels[j2] >> 8 & 0xff) * i;
                    int l1 = (pixels[j2] & 0xff) * i;
                    int i3 = ((k + j1 >> 8) << 16) + ((l + k1 >> 8) << 8) + (i1 + l1 >> 8);
                    pixels[j2++] = i3;
                }

                j2 += i2;
            }

        }

        public void DrawBox(int x, int y, int width, int height, int colour)
        {
            if (x < imageRectangle.X)
            {
                width -= imageRectangle.X - x;
                x = imageRectangle.X;
            }

            if (y < imageRectangle.Y)
            {
                height -= imageRectangle.Y - y;
                y = imageRectangle.Y;
            }

            if (x + width > imageRectangle.Width)
            {
                width = imageRectangle.Width - x;
            }

            if (y + height > imageRectangle.Height)
            {
                height = imageRectangle.Height - y;
            }

            int i = GameSize.Width - width;
            byte byte0 = 1;

            int k = x + y * GameSize.Width;

            for (int l = -height; l < 0; l += byte0)
            {
                for (int i1 = -width; i1 < 0; i1 += 1)
                {
                    pixels[k++] = colour;
                }

                k += i;
            }

        }

        public void DrawBoxEdge(int x, int y, int width, int height, int colour)
        {
            DrawHorizontalLine(x, y, width, colour);
            DrawHorizontalLine(x, y + height - 1, width, colour);
            DrawVerticalLine(x, y, height, colour);
            DrawVerticalLine(x + width - 1, y, height, colour);
        }

        public void DrawHorizontalLine(int x, int y, int length, int colour)
        {
            if (y < imageRectangle.Y || y >= imageRectangle.Height)
            {
                return;
            }

            if (x < imageRectangle.X)
            {
                length -= imageRectangle.X - x;
                x = imageRectangle.X;
            }

            if (x + length > imageRectangle.Width)
            {
                length = imageRectangle.Width - x;
            }

            int i = x + y * GameSize.Width;

            for (int k = 0; k < length; k += 1)
            {
                pixels[i + k] = colour;
            }
        }

        public void DrawVerticalLine(int x, int y, int length, int colour)
        {
            if (x < imageRectangle.X || x >= imageRectangle.Width)
            {
                return;
            }

            if (y < imageRectangle.Y)
            {
                length -= imageRectangle.Y - y;
                y = imageRectangle.Y;
            }

            if (y + length > imageRectangle.Width)
            {
                length = imageRectangle.Height - y;
            }

            int i = x + y * GameSize.Width;

            for (int k = 0; k < length; k += 1)
            {
                pixels[i + k * GameSize.Width] = colour;
            }
        }

        public void DrawMinimapPixel(int x, int y, int color)
        {
            if (!imageRectangle.Contains(x, y))
            {
                return;
            }

            pixels[x + y * GameSize.Width] = color;
        }

        public void FadeScreenToBlack()
        {
            for (int i = 0; i < GameSize.Area; i += 1)
            {
                int pixel = pixels[i] & 0xffffff;

                pixels[i] = (int)(
                    ((uint)pixel >> 1 & 0x7f7f7f) +
                    ((uint)pixel >> 2 & 0x3f3f3f) +
                    ((uint)pixel >> 3 & 0x1f1f1f) +
                    ((uint)pixel >> 4 & 0xf0f0f));
            }
        }

        public void DrawTransparentLine(int x, int y, int destX, int destY, int length, int color)
        {
            for (int i = destX; i < destX + length; i += 1)
            {
                for (int k = destY; k < destY + color; k += 1)
                {
                    int l = 0;
                    int i1 = 0;
                    int j1 = 0;
                    int k1 = 0;

                    for (int l1 = i - x; l1 <= i + x; l1 += 1)
                    {
                        if (l1 < 0 || l1 >= GameSize.Width)
                        {
                            continue;
                        }

                        for (int i2 = k - y; i2 <= k + y; i2 += 1)
                        {
                            if (i2 < 0 || i2 >= GameSize.Height)
                            {
                                continue;
                            }

                            int j2 = pixels[l1 + GameSize.Width * i2];
                            l += j2 >> 16 & 0xff;
                            i1 += j2 >> 8 & 0xff;
                            j1 += j2 & 0xff;
                            k1 += 1;
                        }
                    }

                    pixels[i + GameSize.Width * k] = (l / k1 << 16) + (i1 / k1 << 8) + j1 / k1;
                }
            }
        }

        public static uint RgbaToUInt(int r, int g, int b, int a)
        {
            if (((r | g | b | a) & -256) != 0)
            {
                r = ClampToByte32(r);
                g = ClampToByte32(g);
                b = ClampToByte32(b);
                a = ClampToByte32(a);
            }
            g <<= 8;
            b <<= 0x10;
            a <<= 0x18;
            return (uint)(r | g | b | a);
            //return (r << 24) + (g << 16) + (b << 8) + a;
        }

        private static int ClampToByte32(int value)
        {
            if (value < 0)
            {
                return 0;
            }

            if (value > 255)
            {
                return 255;
            }

            return value;
        }

        public void UnloadContent()
        {
            for (int i = 0; i < pictureColors.Length; i += 1)
            {
                pictureColors[i] = null;
                pictureWidth[i] = 0;
                pictureHeight[i] = 0;
                pictureColorIndexes[i] = null;
                pictureColor[i] = null;
            }
        }

        public void UnpackImageData(int startIndex, sbyte[] imageData, sbyte[] metaData, int count)
        {
            int i = DataOperations.GetInt16(imageData, 0);
            int k = DataOperations.GetInt16(metaData, i);
            i += 2;
            int l = DataOperations.GetInt16(metaData, i);
            i += 2;
            int i1 = metaData[i++] & 0xff;
            int[] ai = new int[i1];

            //      List<Color> clr = new List<Color>();
            ai[0] = 0xff00ff;
            for (int j1 = 0; j1 < i1 - 1; j1 += 1)
            {
                //var r = destX[x] & 0xff;
                //var g = destX[x + 1] & 0xff;
                //var b = destX[x + 2] & 0xff;
                //clr.Add(new Color(r, g, b, 255));

                ai[j1 + 1] = ((metaData[i] & 0xff) << 16) + ((metaData[i + 1] & 0xff) << 8) + (metaData[i + 2] & 0xff);
                i += 3;
            }

            int k1 = 2;
            for (int l1 = startIndex; l1 < startIndex + count; l1 += 1)
            {
                if (l1 >= pictureOffsetX.Length)
                {
                    break;
                }

                pictureOffsetX[l1] = metaData[i++] & 0xff;
                pictureOffsetY[l1] = metaData[i++] & 0xff;
                pictureWidth[l1] = DataOperations.GetInt16(metaData, i);
                i += 2;
                pictureHeight[l1] = DataOperations.GetInt16(metaData, i);
                i += 2;
                int i2 = metaData[i++] & 0xff;
                int j2 = pictureWidth[l1] * pictureHeight[l1];
                pictureColorIndexes[l1] = new sbyte[j2];
                pictureColor[l1] = ai;
                pictureAssumedWidth[l1] = k;
                pictureAssumedHeight[l1] = l;
                pictureColors[l1] = null;
                hasTransparentBackground[l1] = false;
                if (pictureOffsetX[l1] != 0 || pictureOffsetY[l1] != 0)
                {
                    hasTransparentBackground[l1] = true;
                }

                if (i2 == 0)
                {
                    for (int k2 = 0; k2 < j2; k2 += 1)
                    {
                        // clr[k2] = y[k1];
                        pictureColorIndexes[l1][k2] = imageData[k1++];
                        if (pictureColorIndexes[l1][k2] == 0)
                        {
                            hasTransparentBackground[l1] = true;
                        }
                    }

                }
                else if (i2 == 1)
                {
                    for (int l2 = 0; l2 < pictureWidth[l1]; l2 += 1)
                    {
                        for (int i3 = 0; i3 < pictureHeight[l1]; i3 += 1)
                        {

                            pictureColorIndexes[l1][l2 + i3 * pictureWidth[l1]] = imageData[k1++];
                            if (pictureColorIndexes[l1][l2 + i3 * pictureWidth[l1]] == 0)
                            {
                                hasTransparentBackground[l1] = true;
                            }
                        }

                    }

                }
            }
        }

        public void ApplyImage(int pictureIndex)
        {
            int i = pictureWidth[pictureIndex] * pictureHeight[pictureIndex];
            int[] ai = pictureColors[pictureIndex];
            int[] ai1 = new int[32768];
            for (int k = 0; k < i; k += 1)
            {
                int l = ai[k];
                ai1[((l & 0xf80000) >> 9) + ((l & 0xf800) >> 6) + ((l & 0xf8) >> 3)] += 1;
            }

            int[] ai2 = new int[256];
            ai2[0] = 0xff00ff;
            int[] ai3 = new int[256];
            for (int i1 = 0; i1 < 32768; i1 += 1)
            {
                int j1 = ai1[i1];
                if (j1 > ai3[255])
                {
                    for (int k1 = 1; k1 < 256; k1 += 1)
                    {
                        if (j1 <= ai3[k1])
                        {
                            continue;
                        }

                        for (int i2 = 255; i2 > k1; i2 -= 1)
                        {
                            ai2[i2] = ai2[i2 - 1];
                            ai3[i2] = ai3[i2 - 1];
                        }

                        ai2[k1] = ((i1 & 0x7c00) << 9) + ((i1 & 0x3e0) << 6) + ((i1 & 0x1f) << 3) + 0x40404;
                        ai3[k1] = j1;
                        break;
                    }

                }
                ai1[i1] = -1;
            }

            sbyte[] abyte0 = new sbyte[i];
            //  Color[] colors = new Color[x];
            for (int l1 = 0; l1 < i; l1 += 1)
            {
                int j2 = ai[l1];
                int k2 = ((j2 & 0xf80000) >> 9) + ((j2 & 0xf800) >> 6) + ((j2 & 0xf8) >> 3);
                int l2 = ai1[k2];
                if (l2 == -1)
                {
                    int i3 = 0x3b9ac9ff;
                    int b = j2 >> 16 & 0xff;
                    int g = j2 >> 8 & 0xff;
                    int r = j2 & 0xff;
                    // colors[width] = new Color(j3, k3, l3, 255);

                    for (int i4 = 0; i4 < 256; i4 += 1)
                    {
                        int j4 = ai2[i4];
                        int b1 = j4 >> 16 & 0xff;
                        int g1 = j4 >> 8 & 0xff;
                        int r1 = j4 & 0xff;

                        int j5 = (b - b1) * (b - b1) + (g - g1) * (g - g1) + (r - r1) * (r - r1);
                        if (j5 < i3)
                        {
                            i3 = j5;
                            l2 = i4;
                        }
                    }

                    ai1[k2] = l2;
                }
                abyte0[l1] = (sbyte)l2;
            }

            pictureColorIndexes[pictureIndex] = abyte0;
            pictureColor[pictureIndex] = ai2;
            pictureColors[pictureIndex] = null;
        }

        public void LoadImage(int pictureIndex)
        {
            if (pictureColorIndexes[pictureIndex] is null)
            {
                return;
            }

            int i = pictureWidth[pictureIndex] * pictureHeight[pictureIndex];
            sbyte[] abyte0 = pictureColorIndexes[pictureIndex];
            int[] ai = pictureColor[pictureIndex];
            int[] ai1 = new int[i];
            for (int k = 0; k < i; k += 1)
            {
                int l = ai[abyte0[k] & 0xff];
                if (l == 0)
                {
                    l = 1;
                }
                else
                    if (l == 0xff00ff)
                {
                    l = 0;
                }

                ai1[k] = l;
            }

            pictureColors[pictureIndex] = ai1;
            pictureColorIndexes[pictureIndex] = null;
            pictureColor[pictureIndex] = null;
        }

        public void FillPicture(int pictureIndex, int x, int y, int width, int height)
        {
            pictureWidth[pictureIndex] = width;
            pictureHeight[pictureIndex] = height;
            hasTransparentBackground[pictureIndex] = false;
            pictureOffsetX[pictureIndex] = 0;
            pictureOffsetY[pictureIndex] = 0;
            pictureAssumedWidth[pictureIndex] = width;
            pictureAssumedHeight[pictureIndex] = height;
            int i = width * height;
            int k = 0;
            pictureColors[pictureIndex] = new int[i];

            for (int x1 = x; x1 < x + width; x1 += 1)
            {
                for (int y1 = y; y1 < y + height; y1 += 1)
                {
                    pictureColors[pictureIndex][k++] = pixels[x1 + y1 * GameSize.Width];
                }
            }
        }

        public void DrawImage(int pictureIndex, int x, int y, int width, int height)
        {
            pictureWidth[pictureIndex] = width;
            pictureHeight[pictureIndex] = height;
            hasTransparentBackground[pictureIndex] = false;
            pictureOffsetX[pictureIndex] = 0;
            pictureOffsetY[pictureIndex] = 0;
            pictureAssumedWidth[pictureIndex] = width;
            pictureAssumedHeight[pictureIndex] = height;
            int i = width * height;
            int k = 0;
            pictureColors[pictureIndex] = new int[i];

            for (int l = y; l < y + height; l += 1)
            {
                for (int i1 = x; i1 < x + width; i1 += 1)
                {
                    pictureColors[pictureIndex][k++] = pixels[i1 + l * GameSize.Width];

                }
            }
        }

        public void DrawImage(int x, int y, int width, int height, int j1, int k1, int l1, int i2, bool flag)
        {
            try
            {
                if (k1 == 0)
                {
                    k1 = 0xffffff;
                }

                if (l1 == 0)
                {
                    l1 = 0xffffff;
                }

                int j2 = pictureWidth[j1];
                int k2 = pictureHeight[j1];
                int l2 = 0;
                int i3 = 0;
                int j3 = i2 << 16;
                int k3 = (j2 << 16) / width;
                int l3 = (k2 << 16) / height;
                int i4 = -(i2 << 16) / height;
                if (hasTransparentBackground[j1])
                {
                    int j4 = pictureAssumedWidth[j1];
                    int l4 = pictureAssumedHeight[j1];
                    k3 = (j4 << 16) / width;
                    l3 = (l4 << 16) / height;
                    int k5 = pictureOffsetX[j1];
                    int l5 = pictureOffsetY[j1];
                    if (flag)
                    {
                        k5 = j4 - pictureWidth[j1] - k5;
                    }

                    x += (k5 * width + j4 - 1) / j4;
                    int i6 = (l5 * height + l4 - 1) / l4;
                    y += i6;
                    j3 += i6 * i4;
                    if (k5 * width % j4 != 0)
                    {
                        l2 = (j4 - k5 * width % j4 << 16) / width;
                    }

                    if (l5 * height % l4 != 0)
                    {
                        i3 = (l4 - l5 * height % l4 << 16) / height;
                    }

                    width = ((pictureWidth[j1] << 16) - l2 + k3 - 1) / k3;
                    height = ((pictureHeight[j1] << 16) - i3 + l3 - 1) / l3;
                }
                int k4 = y * GameSize.Width;
                j3 += x << 16;
                if (y < imageRectangle.Y)
                {
                    int i5 = imageRectangle.Y - y;
                    height -= i5;
                    y = imageRectangle.Y;
                    k4 += i5 * GameSize.Width;
                    i3 += l3 * i5;
                    j3 += i4 * i5;
                }
                if (y + height >= imageRectangle.Height)
                {
                    height -= y + height - imageRectangle.Height + 1;
                }

                int j5 = 2;

                if (l1 == 0xffffff)
                {
                    if (pictureColors[j1] is not null)
                    {
                        if (!flag)
                        {
                            Cde(pixels, pictureColors[j1], 0, l2, i3, k4, width, height, k3, l3, j2, k1, j3, i4, j5);
                            return;
                        }

                        Cde(pixels, pictureColors[j1], 0, (pictureWidth[j1] << 16) - l2 - 1, i3, k4, width, height, -k3, l3, j2, k1, j3, i4, j5);
                        return;
                    }

                    if (!flag)
                    {
                        DrawTransparentIndexedSpriteInterlaced(pixels, pictureColorIndexes[j1], pictureColor[j1], 0, l2, i3, k4, width, height, k3, l3, j2, k1, j3, i4, j5);
                        return;
                    }

                    DrawTransparentIndexedSpriteInterlaced(pixels, pictureColorIndexes[j1], pictureColor[j1], 0, (pictureWidth[j1] << 16) - l2 - 1, i3, k4, width, height, -k3, l3, j2, k1, j3, i4, j5);
                    return;
                }
                if (pictureColors[j1] is not null)
                {
                    if (!flag)
                    {
                        DrawTransparentSpriteInterlaced(pixels, pictureColors[j1], 0, l2, i3, k4, width, height, k3, l3, j2, k1, l1, j3, i4, j5);
                        return;
                    }
                    DrawTransparentSpriteInterlaced(pixels, pictureColors[j1], 0, (pictureWidth[j1] << 16) - l2 - 1, i3, k4, width, height, -k3, l3, j2, k1, l1, j3, i4, j5);
                    return;
                }

                if (!flag)
                {
                    DrawTransparentMixedSpriteInterlaced(pixels, pictureColorIndexes[j1], pictureColor[j1], 0, l2, i3, k4, width, height, k3, l3, j2, k1, l1, j3, i4, j5);
                    return;
                }
                DrawTransparentMixedSpriteInterlaced(pixels, pictureColorIndexes[j1], pictureColor[j1], 0, (pictureWidth[j1] << 16) - l2 - 1, i3, k4, width, height, -k3, l3, j2, k1, l1, j3, i4, j5);
                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderImage, "Error in sprite clipping routine.");
            }
        }

        public void DrawImageTransparent(int i, int k, int l, int i1, int j1, int k1)
        {
            try
            {
                int l1 = pictureWidth[j1];
                int i2 = pictureHeight[j1];
                int j2 = 0;
                int k2 = 0;
                int l2 = (l1 << 16) / l;
                int i3 = (i2 << 16) / i1;

                if (hasTransparentBackground[j1])
                {
                    int j3 = pictureAssumedWidth[j1];
                    int l3 = pictureAssumedHeight[j1];
                    l2 = (j3 << 16) / l;
                    i3 = (l3 << 16) / i1;
                    i += (pictureOffsetX[j1] * l + j3 - 1) / j3;
                    k += (pictureOffsetY[j1] * i1 + l3 - 1) / l3;

                    if (pictureOffsetX[j1] * l % j3 != 0)
                    {
                        j2 = (j3 - pictureOffsetX[j1] * l % j3 << 16) / l;
                    }

                    if (pictureOffsetY[j1] * i1 % l3 != 0)
                    {
                        k2 = (l3 - pictureOffsetY[j1] * i1 % l3 << 16) / i1;
                    }

                    l = l * (pictureWidth[j1] - (j2 >> 16)) / j3;
                    i1 = i1 * (pictureHeight[j1] - (k2 >> 16)) / l3;
                }

                int k3 = i + k * GameSize.Width;
                int i4 = GameSize.Width - l;

                if (k < imageRectangle.Y)
                {
                    int j4 = imageRectangle.Y - k;
                    i1 -= j4;
                    k = 0;
                    k3 += j4 * GameSize.Width;
                    k2 += i3 * j4;
                }

                if (k + i1 >= imageRectangle.Height)
                {
                    i1 -= k + i1 - imageRectangle.Height + 1;
                }

                if (i < imageRectangle.X)
                {
                    int k4 = imageRectangle.X - i;
                    l -= k4;
                    i = 0;
                    k3 += k4;
                    j2 += l2 * k4;
                    i4 += k4;
                }

                if (i + l >= imageRectangle.Width)
                {
                    int l4 = i + l - imageRectangle.Width + 1;
                    l -= l4;
                    i4 += l4;
                }

                byte byte0 = 1;

                Ccl(ref pixels, pictureColors[j1], 0, j2, k2, k3, i4, l, i1, l2, i3, l1, byte0, k1);
                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderImage, "Error in sprite clipping routine.");
            }
        }

        public void DrawPicture(int x, int y, int pictureIndex)
        {
            if (hasTransparentBackground[pictureIndex])
            {
                x += pictureOffsetX[pictureIndex];
                y += pictureOffsetY[pictureIndex];
            }
            int i1 = x + y * GameSize.Width;
            int j1 = 0;
            int k1 = pictureHeight[pictureIndex];
            int l1 = pictureWidth[pictureIndex];
            int i2 = GameSize.Width - l1;
            int j2 = 0;

            if (y < imageRectangle.Y)
            {
                int k2 = imageRectangle.Y - y;
                k1 -= k2;
                y = imageRectangle.Y;
                j1 += k2 * l1;
                i1 += k2 * GameSize.Width;
            }
            if (y + k1 >= imageRectangle.Height)
            {
                k1 -= y + k1 - imageRectangle.Height + 1;
            }

            if (x < imageRectangle.X)
            {
                int l2 = imageRectangle.X - x;
                l1 -= l2;
                x = imageRectangle.X;
                j1 += l2;
                i1 += l2;
                j2 += l2;
                i2 += l2;
            }
            if (x + l1 >= imageRectangle.Width)
            {
                int i3 = x + l1 - imageRectangle.Width + 1;
                l1 -= i3;
                j2 += i3;
                i2 += i3;
            }
            if (l1 <= 0 || k1 <= 0)
            {
                return;
            }

            byte byte0 = 1;

            if (pictureColors[pictureIndex] is null)
            {
                Cch(ref pixels, pictureColorIndexes[pictureIndex], pictureColor[pictureIndex], j1, i1, l1, k1, i2, j2, byte0);
                return;
            }
            Ccg(ref pixels, pictureColors[pictureIndex], 0, j1, i1, l1, k1, i2, j2, byte0);
            return;
        }

        public void DrawPicture(int x, int y, int index, int i1)
        {
            if (hasTransparentBackground[index])
            {
                x += pictureOffsetX[index];
                y += pictureOffsetY[index];
            }

            int j1 = x + y * GameSize.Width;
            int k1 = 0;
            int l1 = pictureHeight[index];
            int i2 = pictureWidth[index];
            int j2 = GameSize.Width - i2;
            int k2 = 0;

            if (y < imageRectangle.Y)
            {
                int l2 = imageRectangle.Y - y;
                l1 -= l2;
                y = imageRectangle.Y;
                k1 += l2 * i2;
                j1 += l2 * GameSize.Width;
            }
            if (y + l1 >= imageRectangle.Height)
            {
                l1 -= y + l1 - imageRectangle.Height + 1;
            }

            if (x < imageRectangle.X)
            {
                int i3 = imageRectangle.X - x;
                i2 -= i3;
                x = imageRectangle.X;
                k1 += i3;
                j1 += i3;
                k2 += i3;
                j2 += i3;
            }
            if (x + i2 >= imageRectangle.Width)
            {
                int j3 = x + i2 - imageRectangle.Width + 1;
                i2 -= j3;
                k2 += j3;
                j2 += j3;
            }
            if (i2 <= 0 || l1 <= 0)
            {
                return;
            }

            byte byte0 = 1;

            if (pictureColors[index] is null)
            {
                Cck(ref pixels, pictureColorIndexes[index], pictureColor[index], k1, j1, i2, l1, j2, k2, byte0, i1);
                return;
            }
            Ccj(ref pixels, pictureColors[index], 0, k1, j1, i2, l1, j2, k2, byte0, i1);
            return;
        }

        public void DrawEntity(int x, int y, int width, int height, int index)
        {
            try
            {
                int k1 = pictureWidth[index];
                int l1 = pictureHeight[index];
                int i2 = 0;
                int j2 = 0;
                int k2 = (k1 << 16) / width;
                int l2 = (l1 << 16) / height;

                if (hasTransparentBackground[index])
                {
                    int i3 = pictureAssumedWidth[index];
                    int k3 = pictureAssumedHeight[index];

                    k2 = (i3 << 16) / width;
                    l2 = (k3 << 16) / height;
                    x += (pictureOffsetX[index] * width + i3 - 1) / i3;
                    y += (pictureOffsetY[index] * height + k3 - 1) / k3;

                    if (pictureOffsetX[index] * width % i3 != 0)
                    {
                        i2 = (i3 - pictureOffsetX[index] * width % i3 << 16) / width;
                    }

                    if (pictureOffsetY[index] * height % k3 != 0)
                    {
                        j2 = (k3 - pictureOffsetY[index] * height % k3 << 16) / height;
                    }

                    width = width * (pictureWidth[index] - (i2 >> 16)) / i3;
                    height = height * (pictureHeight[index] - (j2 >> 16)) / k3;
                }

                int j3 = x + y * GameSize.Width;
                int l3 = GameSize.Width - width;

                if (y < imageRectangle.Y)
                {
                    int i4 = imageRectangle.Y - y;

                    height -= i4;
                    y = 0;
                    j3 += i4 * GameSize.Width;
                    j2 += l2 * i4;
                }

                if (y + height >= imageRectangle.Height)
                {
                    height -= y + height - imageRectangle.Height + 1;
                }

                if (x < imageRectangle.X)
                {
                    int j4 = imageRectangle.X - x;
                    width -= j4;
                    x = 0;
                    j3 += j4;
                    i2 += k2 * j4;
                    l3 += j4;
                }

                if (x + width >= imageRectangle.Width)
                {
                    int k4 = x + width - imageRectangle.Width + 1;

                    width -= k4;
                    l3 += k4;
                }

                byte byte0 = 1;

                Cci(ref pixels, pictureColors[index], 0, i2, j2, j3, l3, width, height, k2, l2, k1, byte0);
                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderEntity, "Error in sprite clipping routine.");
            }
        }

        public void DrawCharacterLegs(int i, int k, int l, int i1, int animationNumber, int colour)
        {
            try
            {
                int l1 = pictureWidth[animationNumber];
                int i2 = pictureHeight[animationNumber];
                int j2 = 0;
                int k2 = 0;
                int l2 = (l1 << 16) / l;
                int i3 = (i2 << 16) / i1;

                if (hasTransparentBackground[animationNumber])
                {
                    int j3 = pictureAssumedWidth[animationNumber];
                    int l3 = pictureAssumedHeight[animationNumber];
                    l2 = (j3 << 16) / l;
                    i3 = (l3 << 16) / i1;
                    i += (pictureOffsetX[animationNumber] * l + j3 - 1) / j3;
                    k += (pictureOffsetY[animationNumber] * i1 + l3 - 1) / l3;
                    if (pictureOffsetX[animationNumber] * l % j3 != 0)
                    {
                        j2 = (j3 - pictureOffsetX[animationNumber] * l % j3 << 16) / l;
                    }

                    if (pictureOffsetY[animationNumber] * i1 % l3 != 0)
                    {
                        k2 = (l3 - pictureOffsetY[animationNumber] * i1 % l3 << 16) / i1;
                    }

                    l = l * (pictureWidth[animationNumber] - (j2 >> 16)) / j3;
                    i1 = i1 * (pictureHeight[animationNumber] - (k2 >> 16)) / l3;
                }

                int k3 = i + k * GameSize.Width;
                int i4 = GameSize.Width - l;

                if (k < imageRectangle.Y)
                {
                    int j4 = imageRectangle.Y - k;
                    i1 -= j4;
                    k = 0;
                    k3 += j4 * GameSize.Width;
                    k2 += i3 * j4;
                }
                if (k + i1 >= imageRectangle.Height)
                {
                    i1 -= k + i1 - imageRectangle.Height + 1;
                }

                if (i < imageRectangle.X)
                {
                    int k4 = imageRectangle.X - i;
                    l -= k4;
                    i = 0;
                    k3 += k4;
                    j2 += l2 * k4;
                    i4 += k4;
                }

                if (i + l >= imageRectangle.Width)
                {
                    int l4 = i + l - imageRectangle.Width + 1;
                    l -= l4;
                    i4 += l4;
                }

                byte byte0 = 1;

                Ccm(ref pixels, pictureColors[animationNumber], 0, j2, k2, k3, i4, l, i1, l2, i3, l1, byte0, colour);

                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderCharacter, "Error in sprite clipping routine.");
            }
        }

        private void Ccg(ref int[] pixels, int[] colours, int currentColour, int sourceOffset, int destOffset, int width, int height,
                int destStride, int sourceStride, int arg9)
        {
            int i = -(width >> 2);
            width = -(width & 3);
            for (int k = -height; k < 0; k += arg9)
            {
                for (int l = i; l < 0; l += 1)
                {
                    currentColour = colours[sourceOffset++];
                    if (currentColour != 0)
                    {
                        pixels[destOffset++] = currentColour;
                    }
                    else
                    {
                        destOffset += 1;
                    }

                    currentColour = colours[sourceOffset++];
                    if (currentColour != 0)
                    {
                        pixels[destOffset++] = currentColour;
                    }
                    else
                    {
                        destOffset += 1;
                    }

                    currentColour = colours[sourceOffset++];
                    if (currentColour != 0)
                    {
                        pixels[destOffset++] = currentColour;
                    }
                    else
                    {
                        destOffset += 1;
                    }

                    currentColour = colours[sourceOffset++];
                    if (currentColour != 0)
                    {
                        pixels[destOffset++] = currentColour;
                    }
                    else
                    {
                        destOffset += 1;
                    }
                }

                for (int i1 = width; i1 < 0; i1 += 1)
                {
                    currentColour = colours[sourceOffset++];
                    if (currentColour != 0)
                    {
                        pixels[destOffset++] = currentColour;
                    }
                    else
                    {
                        destOffset += 1;
                    }
                }

                destOffset += destStride;
                sourceOffset += sourceStride;
            }

        }

        private void Cch(
            ref int[] pixels,
            sbyte[] colourIndexes,
            int[] arg2,
            int arg3,
            int arg4,
            int arg5,
            int arg6,
            int arg7,
            int arg8,
            int arg9)
        {
            int i = -(arg5 >> 2);
            arg5 = -(arg5 & 3);
            for (int k = -arg6; k < 0; k += arg9)
            {
                for (int l = i; l < 0; l += 1)
                {
                    sbyte byte0 = colourIndexes[arg3++];
                    if (byte0 != 0)
                    {
                        pixels[arg4++] = arg2[byte0 & 0xff];
                    }
                    else
                    {
                        arg4 += 1;
                    }

                    byte0 = colourIndexes[arg3++];
                    if (byte0 != 0)
                    {
                        pixels[arg4++] = arg2[byte0 & 0xff];
                    }
                    else
                    {
                        arg4 += 1;
                    }

                    byte0 = colourIndexes[arg3++];
                    if (byte0 != 0)
                    {
                        pixels[arg4++] = arg2[byte0 & 0xff];
                    }
                    else
                    {
                        arg4 += 1;
                    }

                    byte0 = colourIndexes[arg3++];
                    if (byte0 != 0)
                    {
                        pixels[arg4++] = arg2[byte0 & 0xff];
                    }
                    else
                    {
                        arg4 += 1;
                    }
                }

                for (int i1 = arg5; i1 < 0; i1 += 1)
                {
                    sbyte byte1 = colourIndexes[arg3++];
                    if (byte1 != 0)
                    {
                        pixels[arg4++] = arg2[byte1 & 0xff];
                    }
                    else
                    {
                        arg4 += 1;
                    }
                }

                arg4 += arg7;
                arg3 += arg8;
            }

        }

        private void Cci(ref int[] pixels, int[] colours, int currentColour, int sourceOffset, int destOffset, int width, int height,
                int destStride, int sourceStride, int arg9, int arg10, int arg11, int arg12)
        {
            try
            {
                int i = sourceOffset;
                for (int k = -sourceStride; k < 0; k += arg12)
                {
                    int l = (destOffset >> 16) * arg11;
                    for (int i1 = -destStride; i1 < 0; i1 += 1)
                    {
                        currentColour = colours[(sourceOffset >> 16) + l];
                        if (currentColour != 0)
                        {
                            pixels[width++] = currentColour;
                        }
                        else
                        {
                            width += 1;
                        }

                        sourceOffset += arg9;
                    }

                    destOffset += arg10;
                    sourceOffset = i;
                    width += height;
                }

                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderSprite, "Error in the plot_scale routine.");
            }
        }

        private void Ccj(ref int[] pixels, int[] colours, int currentColour, int sourceOffset, int destOffset, int width, int height,
                int destStride, int sourceStride, int arg9, int arg10)
        {
            int i = 256 - arg10;
            for (int k = -height; k < 0; k += arg9)
            {
                for (int l = -width; l < 0; l += 1)
                {
                    currentColour = colours[sourceOffset++];
                    if (currentColour != 0)
                    {
                        int i1 = pixels[destOffset];
                        pixels[destOffset++] = (int)(((currentColour & 0xff00ff) * arg10 + (i1 & 0xff00ff) * i & 0xff00ff00) + ((currentColour & 0xff00) * arg10 + (i1 & 0xff00) * i & 0xff0000) >> 8);
                    }
                    else
                    {
                        destOffset += 1;
                    }
                }

                destOffset += destStride;
                sourceOffset += sourceStride;
            }

        }

        private void Cck(ref int[] pixels, sbyte[] colourIndexes, int[] colourLookup, int currentColour, int sourceOffset, int destOffset, int width,
                int height, int destStride, int sourceStride, int arg10)
        {
            int i = 256 - arg10;
            for (int k = -width; k < 0; k += sourceStride)
            {
                for (int l = -destOffset; l < 0; l += 1)
                {
                    int i1 = colourIndexes[currentColour++];
                    if (i1 != 0)
                    {
                        i1 = colourLookup[i1 & 0xff];
                        int j1 = pixels[sourceOffset];
                        pixels[sourceOffset++] = (int)(((i1 & 0xff00ff) * arg10 + (j1 & 0xff00ff) * i & 0xff00ff00) + ((i1 & 0xff00) * arg10 + (j1 & 0xff00) * i & 0xff0000) >> 8);
                    }
                    else
                    {
                        sourceOffset += 1;
                    }
                }

                sourceOffset += height;
                currentColour += destStride;
            }

        }

        private void Ccl(ref int[] pixels, int[] colours, int currentColour, int sourceOffset, int destOffset, int width, int height,
                int destStride, int sourceStride, int arg9, int arg10, int arg11, int arg12, int arg13)
        {
            int i = 256 - arg13;
            try
            {
                int k = sourceOffset;
                for (int l = -sourceStride; l < 0; l += arg12)
                {
                    int i1 = (destOffset >> 16) * arg11;
                    for (int j1 = -destStride; j1 < 0; j1 += 1)
                    {
                        currentColour = colours[(sourceOffset >> 16) + i1];
                        if (currentColour != 0)
                        {
                            int k1 = pixels[width];
                            pixels[width++] = (int)(((currentColour & 0xff00ff) * arg13 + (k1 & 0xff00ff) * i & 0xff00ff00) + ((currentColour & 0xff00) * arg13 + (k1 & 0xff00) * i & 0xff0000) >> 8);
                        }
                        else
                        {
                            width += 1;
                        }
                        sourceOffset += arg9;
                    }

                    destOffset += arg10;
                    sourceOffset = k;
                    width += height;
                }

                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderSprite, "Error in the tran_scale routine.");
            }
        }

        private void Ccm(ref int[] pixels, int[] colours, int currentColour, int sourceOffset, int destOffset, int width, int height,
                int destStride, int sourceStride, int arg9, int arg10, int arg11, int arg12, int color)
        {
            int red = color >> 16 & 0xff;
            int green = color >> 8 & 0xff;
            int blue = color & 0xff;

            try
            {
                int i1 = sourceOffset;
                for (int j1 = -sourceStride; j1 < 0; j1 += arg12)
                {
                    int k1 = (destOffset >> 16) * arg11;
                    for (int l1 = -destStride; l1 < 0; l1 += 1)
                    {
                        currentColour = colours[(sourceOffset >> 16) + k1];
                        if (currentColour != 0)
                        {
                            int i2 = currentColour >> 16 & 0xff;
                            int j2 = currentColour >> 8 & 0xff;
                            int k2 = currentColour & 0xff;
                            if (i2 == j2 && j2 == k2)
                            {
                                pixels[width++] = ((i2 * red >> 8) << 16) + ((j2 * green >> 8) << 8) + (k2 * blue >> 8);
                            }
                            else
                            {
                                pixels[width++] = currentColour;
                            }
                        }
                        else
                        {
                            width += 1;
                        }
                        sourceOffset += arg9;
                    }

                    destOffset += arg10;
                    sourceOffset = i1;
                    width += height;
                }

                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderSprite, "Error in the plot_scale routine.");
            }
        }

        public void DrawMinimapPic(int centreX, int centreY, int pictureIndex, int rotation, int scale)
        {
            if (characterRotationTable is null)
            {
                characterRotationTable = new int[512];

                for (int l = 0; l < 256; l += 1)
                {
                    characterRotationTable[l] = (int)(Math.Sin(l * 0.02454369D) * 32768D);
                    characterRotationTable[l + 256] = (int)(Math.Cos(l * 0.02454369D) * 32768D);
                }
            }

            int i1 = -pictureAssumedWidth[pictureIndex] / 2;
            int j1 = -pictureAssumedHeight[pictureIndex] / 2;

            if (hasTransparentBackground[pictureIndex])
            {
                i1 += pictureOffsetX[pictureIndex];
                j1 += pictureOffsetY[pictureIndex];
            }

            int k1 = i1 + pictureWidth[pictureIndex];
            int l1 = j1 + pictureHeight[pictureIndex];
            int i2 = k1;
            int j2 = j1;
            int k2 = i1;
            int l2 = l1;
            rotation &= 0xff;
            int i3 = characterRotationTable[rotation] * scale;
            int j3 = characterRotationTable[rotation + 256] * scale;
            int k3 = centreX + (j1 * i3 + i1 * j3 >> 22);
            int l3 = centreY + (j1 * j3 - i1 * i3 >> 22);
            int i4 = centreX + (j2 * i3 + i2 * j3 >> 22);
            int j4 = centreY + (j2 * j3 - i2 * i3 >> 22);
            int k4 = centreX + (l1 * i3 + k1 * j3 >> 22);
            int l4 = centreY + (l1 * j3 - k1 * i3 >> 22);
            int i5 = centreX + (l2 * i3 + k2 * j3 >> 22);
            int j5 = centreY + (l2 * j3 - k2 * i3 >> 22);

            if (scale == 192 && (rotation & 0x3f) == (lastCharacterRotation & 0x3f))
            {
                spiralDrawCount += 1;
            }
            else if (scale == 128)
            {
                lastCharacterRotation = rotation;
            }
            else
            {
                characterDrawCount += 1;
            }

            int k5 = l3;
            int l5 = l3;

            if (j4 < k5)
            {
                k5 = j4;
            }
            else if (j4 > l5)
            {
                l5 = j4;
            }

            if (l4 < k5)
            {
                k5 = l4;
            }
            else if (l4 > l5)
            {
                l5 = l4;
            }

            if (j5 < k5)
            {
                k5 = j5;
            }
            else if (j5 > l5)
            {
                l5 = j5;
            }

            if (k5 < imageRectangle.Y)
            {
                k5 = imageRectangle.Y;
            }

            if (l5 > imageRectangle.Height)
            {
                l5 = imageRectangle.Height;
            }

            if (entityScanlineMinX is null || entityScanlineMinX.Length != GameSize.Height + 1)
            {
                entityScanlineMinX = new int[GameSize.Height + 1];
                entityScanlineMaxX = new int[GameSize.Height + 1];
                entityScanlineMinValue = new int[GameSize.Height + 1];
                entityScanlineMaxValue = new int[GameSize.Height + 1];
                entityScanlineMinExtra = new int[GameSize.Height + 1];
                entityScanlineMaxExtra = new int[GameSize.Height + 1];
            }

            for (int i6 = k5; i6 <= l5; i6 += 1)
            {
                entityScanlineMinX[i6] = 0x5f5e0ff;
                entityScanlineMaxX[i6] = -entityScanlineMinX[i6];//0xfa0a1f01;
            }

            int i7 = 0;
            int k7 = 0;
            int i8 = 0;
            int j8 = pictureWidth[pictureIndex];
            int k8 = pictureHeight[pictureIndex];

            i1 = 0;
            j1 = 0;
            i2 = j8 - 1;
            j2 = 0;
            k1 = j8 - 1;
            l1 = k8 - 1;
            k2 = 0;
            l2 = k8 - 1;

            if (j5 != l3)
            {
                i7 = (i5 - k3 << 8) / (j5 - l3);
                i8 = (l2 - j1 << 8) / (j5 - l3);
            }

            int j6;
            int k6;
            int l6;
            int l7;

            if (l3 > j5)
            {
                l6 = i5 << 8;
                l7 = l2 << 8;
                j6 = j5;
                k6 = l3;
            }
            else
            {
                l6 = k3 << 8;
                l7 = j1 << 8;
                j6 = l3;
                k6 = j5;
            }

            if (j6 < 0)
            {
                l6 -= i7 * j6;
                l7 -= i8 * j6;
                j6 = 0;
            }

            if (k6 > GameSize.Height - 1)
            {
                k6 = GameSize.Height - 1;
            }

            for (int l8 = j6; l8 <= k6; l8 += 1)
            {
                entityScanlineMinX[l8] = entityScanlineMaxX[l8] = l6;
                l6 += i7;
                entityScanlineMinValue[l8] = entityScanlineMaxValue[l8] = 0;
                entityScanlineMinExtra[l8] = entityScanlineMaxExtra[l8] = l7;
                l7 += i8;
            }

            if (j4 != l3)
            {
                i7 = (i4 - k3 << 8) / (j4 - l3);
                k7 = (i2 - i1 << 8) / (j4 - l3);
            }

            int j7;

            if (l3 > j4)
            {
                l6 = i4 << 8;
                j7 = i2 << 8;
                j6 = j4;
                k6 = l3;
            }
            else
            {
                l6 = k3 << 8;
                j7 = i1 << 8;
                j6 = l3;
                k6 = j4;
            }

            if (j6 < 0)
            {
                l6 -= i7 * j6;
                j7 -= k7 * j6;
                j6 = 0;
            }

            if (k6 > GameSize.Height - 1)
            {
                k6 = GameSize.Height - 1;
            }

            for (int i9 = j6; i9 <= k6; i9 += 1)
            {
                if (l6 < entityScanlineMinX[i9])
                {
                    entityScanlineMinX[i9] = l6;
                    entityScanlineMinValue[i9] = j7;
                    entityScanlineMinExtra[i9] = 0;
                }

                if (l6 > entityScanlineMaxX[i9])
                {
                    entityScanlineMaxX[i9] = l6;
                    entityScanlineMaxValue[i9] = j7;
                    entityScanlineMaxExtra[i9] = 0;
                }

                l6 += i7;
                j7 += k7;
            }

            if (l4 != j4)
            {
                i7 = (k4 - i4 << 8) / (l4 - j4);
                i8 = (l1 - j2 << 8) / (l4 - j4);
            }

            if (j4 > l4)
            {
                l6 = k4 << 8;
                j7 = k1 << 8;
                l7 = l1 << 8;
                j6 = l4;
                k6 = j4;
            }
            else
            {
                l6 = i4 << 8;
                j7 = i2 << 8;
                l7 = j2 << 8;
                j6 = j4;
                k6 = l4;
            }

            if (j6 < 0)
            {
                l6 -= i7 * j6;
                l7 -= i8 * j6;
                j6 = 0;
            }

            if (k6 > GameSize.Height - 1)
            {
                k6 = GameSize.Height - 1;
            }

            for (int j9 = j6; j9 <= k6; j9 += 1)
            {
                if (l6 < entityScanlineMinX[j9])
                {
                    entityScanlineMinX[j9] = l6;
                    entityScanlineMinValue[j9] = j7;
                    entityScanlineMinExtra[j9] = l7;
                }

                if (l6 > entityScanlineMaxX[j9])
                {
                    entityScanlineMaxX[j9] = l6;
                    entityScanlineMaxValue[j9] = j7;
                    entityScanlineMaxExtra[j9] = l7;
                }

                l6 += i7;
                l7 += i8;
            }

            if (j5 != l4)
            {
                i7 = (i5 - k4 << 8) / (j5 - l4);
                k7 = (k2 - k1 << 8) / (j5 - l4);
            }

            if (l4 > j5)
            {
                l6 = i5 << 8;
                j7 = k2 << 8;
                l7 = l2 << 8;
                j6 = j5;
                k6 = l4;
            }
            else
            {
                l6 = k4 << 8;
                j7 = k1 << 8;
                l7 = l1 << 8;
                j6 = l4;
                k6 = j5;
            }

            if (j6 < 0)
            {
                l6 -= i7 * j6;
                j7 -= k7 * j6;
                j6 = 0;
            }

            if (k6 > GameSize.Height - 1)
            {
                k6 = GameSize.Height - 1;
            }

            for (int k9 = j6; k9 <= k6; k9 += 1)
            {
                if (l6 < entityScanlineMinX[k9])
                {
                    entityScanlineMinX[k9] = l6;
                    entityScanlineMinValue[k9] = j7;
                    entityScanlineMinExtra[k9] = l7;
                }

                if (l6 > entityScanlineMaxX[k9])
                {
                    entityScanlineMaxX[k9] = l6;
                    entityScanlineMaxValue[k9] = j7;
                    entityScanlineMaxExtra[k9] = l7;
                }

                l6 += i7;
                j7 += k7;
            }

            int l9 = k5 * GameSize.Width;
            int[] ai = pictureColors[pictureIndex];

            for (int i10 = k5; i10 < l5; i10 += 1)
            {
                int j10 = entityScanlineMinX[i10] >> 8;
                int k10 = entityScanlineMaxX[i10] >> 8;

                if (k10 - j10 <= 0)
                {
                    l9 += GameSize.Width;
                }
                else
                {
                    int l10 = entityScanlineMinValue[i10] << 9;
                    int i11 = ((entityScanlineMaxValue[i10] << 9) - l10) / (k10 - j10);
                    int j11 = entityScanlineMinExtra[i10] << 9;
                    int k11 = ((entityScanlineMaxExtra[i10] << 9) - j11) / (k10 - j10);
                    if (j10 < imageRectangle.X)
                    {
                        l10 += (imageRectangle.X - j10) * i11;
                        j11 += (imageRectangle.X - j10) * k11;
                        j10 = imageRectangle.X;
                    }
                    if (k10 > imageRectangle.Width)
                    {
                        k10 = imageRectangle.Width;
                    }

                    if (!hasTransparentBackground[pictureIndex])
                    {
                        Cda(ai, 0, l9 + j10, l10, j11, i11, k11, j10 - k10, j8);
                    }
                    else
                    {
                        Cdb(ai, 0, l9 + j10, l10, j11, i11, k11, j10 - k10, j8);
                    }

                    l9 += GameSize.Width;
                }
            }
        }

        private void Cda(int[] colours, int currentColour, int sourceOffset, int destOffset, int width, int height, int destStride, int sourceStride, int interlaceFlag)
        {
            for (currentColour = sourceStride; currentColour < 0; currentColour += 1)
            {
                pixels[sourceOffset++] = colours[(destOffset >> 17) + (width >> 17) * interlaceFlag];

                destOffset += height;
                width += destStride;
            }
        }

        private void Cdb(int[] colours, int currentColour, int sourceOffset, int destOffset, int width, int height,
                int destStride, int sourceStride, int interlaceFlag)
        {
            for (int i = sourceStride; i < 0; i += 1)
            {
                currentColour = colours[(destOffset >> 17) + (width >> 17) * interlaceFlag];

                if (currentColour != 0)
                {
                    this.pixels[sourceOffset++] = currentColour;
                }
                else
                {
                    sourceOffset += 1;
                }

                destOffset += height;
                width += destStride;
            }
        }

        public void DrawVisibleEntity(
            int x,
            int y,
            int width,
            int height,
            int objectId,
            int unknownParam1,
            int unknownParam2)
            => DrawEntity(x, y, width, height, objectId);

        private void Cde(int[] pixels, int[] colours, int currentColour, int sourceOffset, int destOffset, int width, int height,
                int destStride, int sourceStride, int interlaceFlag, int arg10, int arg11, int arg12, int arg13,
                int arg14)
        {
            int i1 = arg11 >> 16 & 0xff;
            int j1 = arg11 >> 8 & 0xff;
            int k1 = arg11 & 0xff;
            try
            {
                int l1 = sourceOffset;
                for (int i2 = -destStride; i2 < 0; i2 += 1)
                {
                    int j2 = (destOffset >> 16) * arg10;
                    int k2 = arg12 >> 16;
                    int l2 = height;
                    if (k2 < imageRectangle.X)
                    {
                        int i3 = imageRectangle.X - k2;
                        l2 -= i3;
                        k2 = imageRectangle.X;
                        sourceOffset += sourceStride * i3;
                    }

                    if (k2 + l2 >= imageRectangle.Width)
                    {
                        int j3 = k2 + l2 - imageRectangle.Width;
                        l2 -= j3;
                    }

                    arg14 = 1 - arg14;

                    if (arg14 != 0)
                    {
                        for (int k3 = k2; k3 < k2 + l2; k3 += 1)
                        {
                            currentColour = colours[(sourceOffset >> 16) + j2];
                            if (currentColour != 0)
                            {
                                int i = currentColour >> 16 & 0xff;
                                int k = currentColour >> 8 & 0xff;
                                int l = currentColour & 0xff;
                                if (i == k && k == l)
                                {
                                    pixels[k3 + width] = ((i * i1 >> 8) << 16) + ((k * j1 >> 8) << 8) + (l * k1 >> 8);
                                }
                                else
                                {
                                    pixels[k3 + width] = currentColour;
                                }
                            }
                            sourceOffset += sourceStride;
                        }

                    }
                    destOffset += interlaceFlag;
                    sourceOffset = l1;
                    width += GameSize.Width;
                    arg12 += arg13;
                }

                return;
            }
            catch (Exception)
            {
                //Console.WriteLine("error in transparent sprite plot routine");
            }
        }

        private void DrawTransparentSpriteInterlaced(int[] pixels, int[] colours, int currentColour, int sourceX, int sourceY, int destOffset, int width,
                int height, int destXStep, int destYStep, int sourceWidth, int colourData1, int colourData2, int scanlineStart,
                int scanlineStep, int interlaceFlag)
        {
            int i1 = colourData1 >> 16 & 0xff;
            int j1 = colourData1 >> 8 & 0xff;
            int k1 = colourData1 & 0xff;
            int l1 = colourData2 >> 16 & 0xff;
            int i2 = colourData2 >> 8 & 0xff;
            int j2 = colourData2 & 0xff;
            try
            {
                int k2 = sourceX;
                for (int l2 = -height; l2 < 0; l2 += 1)
                {
                    int i3 = (sourceY >> 16) * sourceWidth;
                    int j3 = scanlineStart >> 16;
                    int k3 = width;
                    if (j3 < imageRectangle.X)
                    {
                        int l3 = imageRectangle.X - j3;
                        k3 -= l3;
                        j3 = imageRectangle.X;
                        sourceX += destXStep * l3;
                    }
                    if (j3 + k3 >= imageRectangle.Width)
                    {
                        int i4 = j3 + k3 - imageRectangle.Width;
                        k3 -= i4;
                    }
                    interlaceFlag = 1 - interlaceFlag;
                    if (interlaceFlag != 0)
                    {
                        for (int j4 = j3; j4 < j3 + k3; j4 += 1)
                        {
                            currentColour = colours[(sourceX >> 16) + i3];
                            if (currentColour != 0)
                            {
                                int i = currentColour >> 16 & 0xff;
                                int k = currentColour >> 8 & 0xff;
                                int l = currentColour & 0xff;
                                if (i == k && k == l)
                                {
                                    pixels[j4 + destOffset] = ((i * i1 >> 8) << 16) + ((k * j1 >> 8) << 8) + (l * k1 >> 8);
                                }
                                else
                                    if (i == 255 && k == l)
                                {
                                    pixels[j4 + destOffset] = ((i * l1 >> 8) << 16) + ((k * i2 >> 8) << 8) + (l * j2 >> 8);
                                }
                                else
                                {
                                    pixels[j4 + destOffset] = currentColour;
                                }
                            }
                            sourceX += destXStep;
                        }

                    }
                    sourceY += destYStep;
                    sourceX = k2;
                    destOffset += GameSize.Width;
                    scanlineStart += scanlineStep;
                }

                return;
            }
            catch (Exception)
            {
                //Console.WriteLine("error in transparent sprite plot routine");
            }
        }

        private void DrawTransparentIndexedSpriteInterlaced(int[] pixels, sbyte[] colours, int[] currentColour, int sourceX, int sourceY, int destOffset, int width,
                int height, int destXStep, int destYStep, int sourceWidth, int colourData1, int colourData2, int scanlineStart,
                int scanlineStep, int interlaceFlag)
        {
            int i1 = colourData2 >> 16 & 0xff;
            int j1 = colourData2 >> 8 & 0xff;
            int k1 = colourData2 & 0xff;
            try
            {
                int l1 = sourceY;
                for (int i2 = -destXStep; i2 < 0; i2 += 1)
                {
                    int j2 = (destOffset >> 16) * colourData1;
                    int k2 = scanlineStart >> 16;
                    int l2 = height;
                    if (k2 < imageRectangle.X)
                    {
                        int i3 = imageRectangle.X - k2;
                        l2 -= i3;
                        k2 = imageRectangle.X;
                        sourceY += destYStep * i3;
                    }
                    if (k2 + l2 >= imageRectangle.Width)
                    {
                        int j3 = k2 + l2 - imageRectangle.Width;
                        l2 -= j3;
                    }
                    interlaceFlag = 1 - interlaceFlag;
                    if (interlaceFlag != 0)
                    {
                        for (int k3 = k2; k3 < k2 + l2; k3 += 1)
                        {
                            sourceX = colours[(sourceY >> 16) + j2] & 0xff;
                            if (sourceX != 0)
                            {
                                sourceX = currentColour[sourceX];
                                int i = sourceX >> 16 & 0xff;
                                int k = sourceX >> 8 & 0xff;
                                int l = sourceX & 0xff;
                                if (i == k && k == l)
                                {
                                    pixels[k3 + width] = ((i * i1 >> 8) << 16) + ((k * j1 >> 8) << 8) + (l * k1 >> 8);
                                }
                                else
                                {
                                    pixels[k3 + width] = sourceX;
                                }
                            }
                            sourceY += destYStep;
                        }

                    }
                    destOffset += sourceWidth;
                    sourceY = l1;
                    width += GameSize.Width;
                    scanlineStart += scanlineStep;
                }

                return;
            }
            catch (Exception)
            {
                //Console.WriteLine("error in transparent sprite plot routine");
            }
        }

        private void DrawTransparentMixedSpriteInterlaced(int[] pixels, sbyte[] colours, int[] currentColour, int sourceX, int sourceY, int destOffset, int width,
                int height, int destXStep, int destYStep, int sourceWidth, int colourData1, int colourData2, int scanlineStart,
                int scanlineStep, int interlaceFlag, int secondaryColour)
        {
            int i1 = colourData2 >> 16 & 0xff;
            int j1 = colourData2 >> 8 & 0xff;
            int k1 = colourData2 & 0xff;
            int l1 = scanlineStart >> 16 & 0xff;
            int i2 = scanlineStart >> 8 & 0xff;
            int j2 = scanlineStart & 0xff;
            try
            {
                int k2 = sourceY;
                for (int l2 = -destXStep; l2 < 0; l2 += 1)
                {
                    int i3 = (destOffset >> 16) * colourData1;
                    int j3 = scanlineStep >> 16;
                    int k3 = height;
                    if (j3 < imageRectangle.X)
                    {
                        int l3 = imageRectangle.X - j3;
                        k3 -= l3;
                        j3 = imageRectangle.X;
                        sourceY += destYStep * l3;
                    }
                    if (j3 + k3 >= imageRectangle.Width)
                    {
                        int i4 = j3 + k3 - imageRectangle.Width;
                        k3 -= i4;
                    }
                    secondaryColour = 1 - secondaryColour;
                    if (secondaryColour != 0)
                    {
                        for (int j4 = j3; j4 < j3 + k3; j4 += 1)
                        {
                            sourceX = colours[(sourceY >> 16) + i3] & 0xff;
                            if (sourceX != 0)
                            {
                                sourceX = currentColour[sourceX];
                                int i = sourceX >> 16 & 0xff;
                                int k = sourceX >> 8 & 0xff;
                                int l = sourceX & 0xff;
                                if (i == k && k == l)
                                {
                                    pixels[j4 + width] = ((i * i1 >> 8) << 16) + ((k * j1 >> 8) << 8) + (l * k1 >> 8);
                                }
                                else
                                    if (i == 255 && k == l)
                                {
                                    pixels[j4 + width] = ((i * l1 >> 8) << 16) + ((k * i2 >> 8) << 8) + (l * j2 >> 8);
                                }
                                else
                                {
                                    pixels[j4 + width] = sourceX;
                                }
                            }
                            sourceY += destYStep;
                        }

                    }
                    destOffset += sourceWidth;
                    sourceY = k2;
                    width += GameSize.Width;
                    scanlineStep += interlaceFlag;
                }

                return;
            }
            catch (Exception)
            {
                //Console.WriteLine("error in transparent sprite plot routine");
            }
        }

        public void DrawLabel(string text, int x, int y, int fontIndex, int colour) => DrawString(text, x - GetTextWidth(text, fontIndex), y, fontIndex, colour);

        public void DrawText(string text, int x, int y, int fontIndex, int colour) => DrawString(text, x - GetTextWidth(text, fontIndex) / 2, y, fontIndex, colour);

        public void DrawFloatingText(string text, int x, int y, int fontIndex, int colour, int maxWidth)
        {
            try
            {
                int i = 0;
                sbyte[] abyte0 = gameFonts[fontIndex];
                int k = 0;
                int l = 0;
                for (int i1 = 0; i1 < text.Length; i1 += 1)
                {
                    if (text[i1] == '@' && i1 + 4 < text.Length && text[i1 + 4] == '@')
                    {
                        i1 += 4;
                    }
                    else
                        if (text[i1] == '~' && i1 + 4 < text.Length && text[i1 + 4] == '~')
                    {
                        i1 += 4;
                    }
                    else
                    {
                        i += abyte0[characterFontOffsetTable[text[i1]] + 7];
                    }

                    if (text[i1] == ' ')
                    {
                        l = i1;
                    }

                    if (text[i1] == '%')
                    {
                        l = i1;
                        i = 1000;
                    }
                    if (i > maxWidth)
                    {
                        if (l <= k)
                        {
                            l = i1;
                        }

                        DrawText(text.Substring(k, l), x, y, fontIndex, colour);
                        i = 0;
                        k = i1 = l + 1;
                        y += GetTextHeight(fontIndex);
                    }
                }

                if (i > 0)
                {
                    DrawText(text[k..], x, y, fontIndex, colour);
                    return;
                }
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderText, "Error in the centrepara routine.", exception);
            }
        }

        public static List<StringDraw> stringsToDraw = [];

        public void DrawString(string text, int x, int y, int fontIndex, int colour)
        {
            try
            {
#warning fix real draw string
                sbyte[] abyte0 = gameFonts[fontIndex];
                try
                {
                    for (int i = 0; i < text.Length; i += 1)
                    {
                        char currentChar = text[i];
                        int lookAheadIndex = i + 4;
                        int textLength = text.Length;

                        if (text[i] == '@' && lookAheadIndex < textLength)
                        {
                            char lookAheadChar = text[i + 4];
                            string colourCode = text.Substring(i + 1, 3).ToLower();
                        }
                        if (text[i] == '@' && i + 4 < text.Length && text[i + 4] == '@')
                        {
                            if (text.Substring(i + 1, 3).ToLower() == "red")
                            {
                                colour = 0xff0000;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "lre")
                            {
                                colour = 0xff9040;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "yel")
                            {
                                colour = 0xffff00;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "gre")
                            {
                                colour = 65280;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "blu")
                            {
                                colour = 255;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "cya")
                            {
                                colour = 65535;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "mag")
                            {
                                colour = 0xff00ff;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "whi")
                            {
                                colour = 0xffffff;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "normalZ")
                            {
                                colour = 0;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "dre")
                            {
                                colour = 0xc00000;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "ora")
                            {
                                colour = 0xff9040;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "ran")
                            {
                                colour = (int)(new Random().NextDouble() * 16777215D);
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "or1")
                            {
                                colour = 0xffb000;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "or2")
                            {
                                colour = 0xff7000;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "or3")
                            {
                                colour = 0xff3000;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "gr1")
                            {
                                colour = 0xc0ff00;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "gr2")
                            {
                                colour = 0x80ff00;
                            }
                            else if (text.Substring(i + 1, 3).ToLower() == "gr3")
                            {
                                colour = 0x40ff00;
                            }

                            i += 3;
                            continue;
                        }
                        if (text[i] == '~' && i + 4 < text.Length && text[i + 4] == '~')
                        {
                            char c = text[i + 1];
                            char c1 = text[i + 2];
                            char c2 = text[i + 3];
                            if (c >= '0' && c <= '9' && c1 >= '0' && c1 <= '9' && c2 >= '0' && c2 <= '9')
                            {
                                x = int.Parse(text.Substring(i + 1, i + 4));
                            }

                            i += 3;
                        }
                        else if (text[i] != '@' && text[i] != '~')
                        {
                            int k = characterFontOffsetTable[text[i]];
                            if (IsLoggedIn && !fontShadowEnabled[fontIndex] && colour != 0)
                            {
                                PlotCharacter(k, x + 1, y, 0, abyte0, fontShadowEnabled[fontIndex]);
                            }

                            if (IsLoggedIn && !fontShadowEnabled[fontIndex] && colour != 0)
                            {
                                PlotCharacter(k, x, y + 1, 0, abyte0, fontShadowEnabled[fontIndex]);
                            }

                            PlotCharacter(k, x, y, colour, abyte0, fontShadowEnabled[fontIndex]);
                            x += abyte0[k + 7];
                        }
                    }
                }
                catch
                {
                    logger.Error(GameOperation.RenderText, "An error has occurred while drawing.");
                }

                //stringsToDraw.Add(new stringDrawDef
                //{
                //    text = _pixels,
                //    pos = new Vector2(y, destX),
                //    forecolor = new Color((startColor & 0xff0000), (startColor & 0x00ff00), (startColor & 0x0000ff), 255),
                //});

                //else if (_pixels[x] == '~' && x + 4 < _pixels.Length && _pixels[x + 4] == '~')
                //{
                //    char c = _pixels[x + 1];
                //    char c1 = _pixels[x + 2];
                //    char c2 = _pixels[x + 3];
                //    if (c >= '0' && c <= '9' && c1 >= '0' && c1 <= '9' && c2 >= '0' && c2 <= '9')
                //        y = int.Parse(_pixels.Substring(x + 1, x + 4));
                //    x += 4;
                //}
                //else

                return;
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderText, "Error in the drawstring routine.", exception);

                return;
            }
        }

        private void PlotCharacter(int charOffset, int x, int y, int colour, sbyte[] fontData, bool useShadow)
        {
            int j1 = x + fontData[charOffset + 5];
            int k1 = y - fontData[charOffset + 6];
            int l1 = fontData[charOffset + 3];
            int i2 = fontData[charOffset + 4];
            int j2 = fontData[charOffset] * 16384 + fontData[charOffset + 1] * 128 + fontData[charOffset + 2];
            int k2 = j1 + k1 * GameSize.Width;
            int l2 = GameSize.Width - l1;
            int i3 = 0;
            if (k1 < imageRectangle.Y)
            {
                int j3 = imageRectangle.Y - k1;
                i2 -= j3;
                k1 = imageRectangle.Y;
                j2 += j3 * l1;
                k2 += j3 * GameSize.Width;
            }
            if (k1 + i2 >= imageRectangle.Height)
            {
                i2 -= k1 + i2 - imageRectangle.Height + 1;
            }

            if (j1 < imageRectangle.X)
            {
                int k3 = imageRectangle.X - j1;
                l1 -= k3;
                j1 = imageRectangle.X;
                j2 += k3;
                k2 += k3;
                i3 += k3;
                l2 += k3;
            }
            if (j1 + l1 >= imageRectangle.Width)
            {
                int l3 = j1 + l1 - imageRectangle.Width + 1;
                l1 -= l3;
                i3 += l3;
                l2 += l3;
            }
            if (l1 > 0 && i2 > 0)
            {
                if (useShadow)
                {
                    PlotAlphaLetter(ref pixels, fontData, colour, j2, k2, l1, i2, l2, i3);
                    return;
                }
                PlotLetter(ref pixels, fontData, colour, j2, k2, l1, i2, l2, i3);
            }
        }

        private void PlotLetter(ref int[] screenPixels, sbyte[] fontData, int colour, int glyphOffset, int pixelOffset, int glyphWidth, int glyphHeight,
                int screenStride, int fontStride)
        {
            try
            {
                int i = -(glyphWidth >> 2);
                glyphWidth = -(glyphWidth & 3);
                for (int k = -glyphHeight; k < 0; k += 1)
                {
                    for (int l = i; l < 0; l += 1)
                    {
                        if (fontData[glyphOffset++] != 0)
                        {
                            screenPixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            pixelOffset += 1;
                        }

                        if (fontData[glyphOffset++] != 0)
                        {
                            screenPixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            pixelOffset += 1;
                        }

                        if (fontData[glyphOffset++] != 0)
                        {
                            screenPixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            pixelOffset += 1;
                        }

                        if (fontData[glyphOffset++] != 0)
                        {
                            screenPixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            pixelOffset += 1;
                        }
                    }

                    for (int i1 = glyphWidth; i1 < 0; i1 += 1)
                    {
                        if (fontData[glyphOffset++] != 0)
                        {
                            screenPixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            pixelOffset += 1;
                        }
                    }

                    pixelOffset += screenStride;
                    glyphOffset += fontStride;
                }

                return;
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderText, "Error in the plotletter routine.", exception);

                return;
            }
        }

        private void PlotAlphaLetter(ref int[] pixels, sbyte[] fontData, int colour, int glyphOffset, int pixelOffset, int glyphWidth, int glyphHeight,
                int screenStride, int fontStride)
        {
            for (int i = -glyphHeight; i < 0; i += 1)
            {
                for (int k = -glyphWidth; k < 0; k += 1)
                {
                    int l = fontData[glyphOffset++] & 0xff;
                    if (l > 30)
                    {
                        if (l >= 230)
                        {
                            pixels[pixelOffset++] = colour;
                        }
                        else
                        {
                            int i1 = pixels[pixelOffset];
                            pixels[pixelOffset++] = (int)(((colour & 0xff00ff) * l + (i1 & 0xff00ff) * (256 - l) & 0xff00ff00) + ((colour & 0xff00) * l + (i1 & 0xff00) * (256 - l) & 0xff0000) >> 8);
                        }
                    }
                    else
                    {
                        pixelOffset += 1;
                    }
                }

                pixelOffset += screenStride;
                glyphOffset += fontStride;
            }

        }

        public int GetTextHeight(int i)
        {
            //return (int)mudclient.gameFont12.MeasureString("A").Y;

            if (i == 0)
            {
                return 12;
            }

            if (i == 1)
            {
                return 14;
            }

            if (i == 2)
            {
                return 14;
            }

            if (i == 3)
            {
                return 15;
            }

            if (i == 4)
            {
                return 15;
            }

            if (i == 5)
            {
                return 19;
            }

            if (i == 6)
            {
                return 24;
            }

            if (i == 7)
            {
                return 29;
            }

            return GetFontLineHeight(i);
        }

        public int GetFontLineHeight(int i)
        {
            if (i == 0)
            {
                return gameFonts[i][8] - 2;
            }

            return gameFonts[i][8] - 1;
        }

        public int GetTextWidth(string text, int fontIndex)
        {
            int i = 0;
            sbyte[] abyte0 = gameFonts[fontIndex];

            for (int k = 0; k < text.Length; k += 1)
            {
                if (text[k] == '@' && k + 4 < text.Length && text[k + 4] == '@')
                {
                    k += 4;
                }
                else
                    if (text[k] == '~' && k + 4 < text.Length && text[k + 4] == '~')
                {
                    k += 4;
                }
                else
                {
                    i += abyte0[characterFontOffsetTable[text[k]] + 7];
                }
            }

            return i;
        }

        public void DrawPixels(int[][] pixels, int drawX, int drawY, int width, int height)
        {
            for (int x = drawX; x < drawX + width; x += 1)
            {
                for (int y = drawY; y < drawY + height; y += 1)
                {
                    this.pixels[x + y * GameSize.Width] = pixels[x - drawX][y - drawY];
                }
            }
        }

        public static int addFont(sbyte[] bytes)
        {
            gameFonts[currentFont] = bytes;
            currentFont += 1;
            return currentFont - 1;
        }

        public int[] pixels;
        public Texture2D imageTexture;
        public int[][] pictureColors;
        public sbyte[][] pictureColorIndexes;
        public int[][] pictureColor;
        public int[] pictureWidth;
        public int[] pictureHeight;
        public int[] pictureOffsetX;
        public int[] pictureOffsetY;
        public int[] pictureAssumedWidth;
        public int[] pictureAssumedHeight;
        public bool[] hasTransparentBackground;
        private static readonly sbyte[][] gameFonts = new sbyte[50][];
        private static readonly int[] characterFontOffsetTable;
        public bool IsLoggedIn;

        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GraphicsEngine>();
        public int[] characterRotationTable;
        public int[] entityScanlineMinX;
        public int[] entityScanlineMaxX;
        public int[] entityScanlineMinValue;
        public int[] entityScanlineMaxValue;
        public int[] entityScanlineMinExtra;
        public int[] entityScanlineMaxExtra;
        public static int spiralDrawCount;
        public static int characterDrawCount;
        public static int lastCharacterRotation;
        private static readonly bool[] fontShadowEnabled = new bool[12];
        private static int currentFont;
    }
}
