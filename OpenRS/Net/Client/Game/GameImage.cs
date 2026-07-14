using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NuciXNA.Primitives;

using NuciLog.Core;

using OpenRS.Logging;
using OpenRS.Net.Client.Data;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Game
{

    public class GameImage  //: org.moparscape.msc.client.GameImage
    /* implements ImageProducer, ImageObserver */{
        // public static Texture2D[] UnpackedImages = new Texture2D[5000];

        public GameImage(int width, int height, int size /*, java.awt.Component destY*/)
        //  : base(_pixels, y, destX, destY)
        {
            interlace = false;
            loggedIn = false;
            imageHeight = height;
            imageWidth = width;
            width = gameWidth = width;
            height = gameHeight = height;
            area = width * height;
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

            if (width > 1 && height > 1 /*&& destY is not null*/)
            {
                // colorModel = new DirectColorModel(32, 0xff0000, 65280, 255);
                int i = gameWidth * gameHeight;
                for (int k = 0; k < i; k += 1)
                {
                    pixels[k] = 0;
                }

                //  image = destY.createImage(this);
                //imageTexture = new Texture2D(graphics, gameWidth, gameHeight);
                //UpdateGameImage();
                //  destY.prepareImage(image, destY);
                // cag();
                //  destY.prepareImage(image, destY);
                // cag();
                //  destY.prepareImage(image, destY);
            }
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public void addConsumer(ImageConsumer imageconsumer)
        //{
        //    imageConsumer = imageconsumer;
        //    imageconsumer.SetDimensions(gameWidth, gameHeight);
        //    imageconsumer.setProperties(null);
        //    imageconsumer.setColorModel(colorModel);
        //    imageconsumer.setHints(14);
        //}

        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public bool isConsumer(ImageConsumer imageconsumer)
        //{
        //    return imageConsumer == imageconsumer;
        //}

        //[MethodImpl(MethodImplOptions.Synchronized)]
        //public void removeConsumer(ImageConsumer imageconsumer)
        //{
        //    if (imageConsumer == imageconsumer)
        //        imageConsumer = null;
        //}

        //public void startProduction(ImageConsumer imageconsumer)
        //{
        //    addConsumer(imageconsumer);
        //}

        //public void requestTopDownLeftRightResend(ImageConsumer imageconsumer)
        //{
        //    Console.WriteLine("TDLR");
        //}

        //public void cag()
        //{
        //    //base.cag();

        //    if (graphics is null)
        //        graphics = GameClient.graphics;

        //    if (GameClient.spriteBatch.BeginIsActive()) return;

        //    if (imageTexture is not null)
        //    {
        //        GameClient.graphics.Textures[0] = null;
        //        this.imageTexture.Dispose();
        //    }
        //    List<Color> clrs = new List<Color>();
        //    foreach (var c in this.pixels)
        //    {
        //        var bytes = BitConverter.GetBytes(c);
        //        var r = bytes[2];
        //        var g = bytes[1];
        //        var b = bytes[0];
        //        clrs.Add(new Color(r, g, b, 255));
        //    }
        //    this.imageTexture = new Texture2D(GameClient.graphics, gameWidth, gameHeight, false, SurfaceFormat.Color);
        //    this.imageTexture.SetData(clrs.ToArray());
        //}

        public void SetDimensions(int x, int y, int _w, int _h)
        {
            if (x < 0)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = 0;
            }

            if (_w > gameWidth)
            {
                _w = gameWidth;
            }

            if (_h > gameHeight)
            {
                _h = gameHeight;
            }

            imageX = x;
            imageY = y;
            imageWidth = _w;
            imageHeight = _h;
        }

        public void ResetDimensions()
        {
            imageX = 0;
            imageY = 0;
            imageWidth = gameWidth;
            imageHeight = gameHeight;
        }

        //public void DrawImage(SpriteBatch g, int x, int y)
        //{
        //    //UpdateGameImage();
        //    try
        //    {
        //        //  g.BeginSafe();
        //        //  if (g.BeginIsActive())
        //        // g.Draw(imageTexture, new Vector2(x, y), Color.White); // DrawImage(image, x, y, this);
        //        //  g.EndSafe();
        //    }
        //    catch { }
        //}

        public void ClearScreen()
        {
            int i = gameWidth * gameHeight;
            if (!interlace)
            {
                for (int k = 0; k < i; k += 1)
                {
                    pixels[k] = 0;
                }

                return;
            }
            int l = 0;
            for (int i1 = -gameHeight; i1 < 0; i1 += 2)
            {
                for (int j1 = -gameWidth; j1 < 0; j1 += 1)
                {
                    pixels[l++] = 0;
                }

                l += gameWidth;
            }

        }

        public void DrawCircle(int centreX, int centreY, int radius, int colour, int alpha)
        {
            int i = 256 - alpha;
            int k = (colour >> 16 & 0xff) * alpha;
            int l = (colour >> 8 & 0xff) * alpha;
            int i1 = (colour & 0xff) * alpha;
            int i2 = centreY - radius;
            if (i2 < 0)
            {
                i2 = 0;
            }

            int j2 = centreY + radius;
            if (j2 >= gameHeight)
            {
                j2 = gameHeight - 1;
            }

            byte byte0 = 1;
            if (interlace)
            {
                byte0 = 2;
                if ((i2 & 1) != 0)
                {
                    i2 += 1;
                }
            }
            for (int k2 = i2; k2 <= j2; k2 += byte0)
            {
                int l2 = k2 - centreY;
                int i3 = (int)Math.Sqrt(radius * radius - l2 * l2);
                int j3 = centreX - i3;
                if (j3 < 0)
                {
                    j3 = 0;
                }

                int k3 = centreX + i3;
                if (k3 >= gameWidth)
                {
                    k3 = gameWidth - 1;
                }

                int l3 = j3 + k2 * gameWidth;
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
            if (x < imageX)
            {
                w -= imageX - x;
                x = imageX;
            }
            if (y < imageY)
            {
                h -= imageY - y;
                y = imageY;
            }
            if (x + w > imageWidth)
            {
                w = imageWidth - x;
            }

            if (y + h > imageHeight)
            {
                h = imageHeight - y;
            }

            int i = 256 - alpha;
            int k = (colour >> 16 & 0xff) * alpha;
            int l = (colour >> 8 & 0xff) * alpha;
            int i1 = (colour & 0xff) * alpha;
            int i2 = gameWidth - w;
            byte byte0 = 1;
            if (interlace)
            {
                byte0 = 2;
                i2 += gameWidth;
                if ((y & 1) != 0)
                {
                    y += 1;
                    h -= 1;
                }
            }
            int j2 = x + y * gameWidth;
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

        public void DrawGradientBox(int x, int y, int w, int h, int startColor, int endColor)
        {
            if (x < imageX)
            {
                w -= imageX - x;
                x = imageX;
            }
            if (x + w > imageWidth)
            {
                w = imageWidth - x;
            }

            int eB = endColor >> 16 & 0xff;
            int eG = endColor >> 8 & 0xff;
            int eR = endColor & 0xff;

            int sB = startColor >> 16 & 0xff;
            int sG = startColor >> 8 & 0xff;
            int sR = startColor & 0xff;
            int l1 = gameWidth - w;
            byte byte0 = 1;
            if (interlace)
            {
                byte0 = 2;
                l1 += gameWidth;
                if ((y & 1) != 0)
                {
                    y += 1;
                    h -= 1;
                }
            }
            int i2 = x + y * gameWidth;
            for (int j2 = 0; j2 < h; j2 += byte0)
            {
                if (j2 + y >= imageY && j2 + y < imageHeight)
                {
                    int k2 = ((eB * j2 + sB * (h - j2)) / h << 16) + ((eG * j2 + sG * (h - j2)) / h << 8) + (eR * j2 + sR * (h - j2)) / h;
                    for (int l2 = -w; l2 < 0; l2 += 1)
                    {
                        pixels[i2++] = k2;
                    }

                    i2 += l1;
                }
                else
                {
                    i2 += gameWidth;
                }
            }
        }

        public void DrawBox(int x, int y, int w, int h, int color)
        {
            if (x < imageX)
            {
                w -= imageX - x;
                x = imageX;
            }
            if (y < imageY)
            {
                h -= imageY - y;
                y = imageY;
            }
            if (x + w > imageWidth)
            {
                w = imageWidth - x;
            }

            if (y + h > imageHeight)
            {
                h = imageHeight - y;
            }

            int i = gameWidth - w;
            byte byte0 = 1;
            if (interlace)
            {
                byte0 = 2;
                i += gameWidth;
                if ((y & 1) != 0)
                {
                    y += 1;
                    h -= 1;
                }
            }
            int k = x + y * gameWidth;
            for (int l = -h; l < 0; l += byte0)
            {
                for (int i1 = -w; i1 < 0; i1 += 1)
                {
                    pixels[k++] = color;
                }

                k += i;
            }

        }

        public void DrawBoxEdge(int x, int y, int w, int h, int color)
        {
            DrawLineX(x, y, w, color);
            DrawLineX(x, y + h - 1, w, color);
            DrawLineY(x, y, h, color);
            DrawLineY(x + w - 1, y, h, color);
        }

        public void DrawLineX(int x, int y, int length, int colour)
        {
            if (y < imageY || y >= imageHeight)
            {
                return;
            }

            if (x < imageX)
            {
                length -= imageX - x;
                x = imageX;
            }
            if (x + length > imageWidth)
            {
                length = imageWidth - x;
            }

            int i = x + y * gameWidth;
            for (int k = 0; k < length; k += 1)
            {
                pixels[i + k] = colour;
            }
        }

        public void DrawLineY(int x, int y, int length, int colour)
        {
            if (x < imageX || x >= imageWidth)
            {
                return;
            }

            if (y < imageY)
            {
                length -= imageY - y;
                y = imageY;
            }
            if (y + length > imageWidth)
            {
                length = imageHeight - y;
            }

            int i = x + y * gameWidth;
            for (int k = 0; k < length; k += 1)
            {
                pixels[i + k * gameWidth] = colour;
            }
        }

        public void DrawMinimapPixel(int x, int y, int color)
        {
            if (x < imageX || y < imageY || x >= imageWidth || y >= imageHeight)
            {
                return;
            }
            else
            {
                pixels[x + y * gameWidth] = color;
                return;
            }
        }

        public void ScreenFadeToBlack()
        {
            int l = gameWidth * gameHeight;
            for (int k = 0; k < l; k += 1)
            {
                int i = pixels[k] & 0xffffff;
                pixels[k] = (int)(((uint)i >> 1 & 0x7f7f7f) + ((uint)i >> 2 & 0x3f3f3f) + ((uint)i >> 3 & 0x1f1f1f) + ((uint)i >> 4 & 0xf0f0f));
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
                        if (l1 >= 0 && l1 < gameWidth)
                        {
                            for (int i2 = k - y; i2 <= k + y; i2 += 1)
                            {
                                if (i2 >= 0 && i2 < gameHeight)
                                {
                                    int j2 = pixels[l1 + gameWidth * i2];
                                    l += j2 >> 16 & 0xff;
                                    i1 += j2 >> 8 & 0xff;
                                    j1 += j2 & 0xff;
                                    k1 += 1;
                                }
                            }
                        }
                    }

                    pixels[i + gameWidth * k] = (l / k1 << 16) + (i1 / k1 << 8) + j1 / k1;
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
            if (value > 0xff)
            {
                return 0xff;
            }
            return value;
        }

        public static int RgbToInt(int i, int k, int l)
        {
            return (i << 16) + (k << 8) + l;
        }

        public void CleanUp()
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
            int i = DataOperations.GetShort(imageData, 0);
            int k = DataOperations.GetShort(metaData, i);
            i += 2;
            int l = DataOperations.GetShort(metaData, i);
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

            // UnpackedImages[_pixels] = new Texture2D(graphics, y, _w);
            // UnpackedImages[_pixels].SetData(ai);

            int k1 = 2;
            for (int l1 = startIndex; l1 < startIndex + count; l1 += 1)
            {
                if (l1 >= pictureOffsetX.Length)
                {
                    break;
                }

                pictureOffsetX[l1] = metaData[i++] & 0xff;
                pictureOffsetY[l1] = metaData[i++] & 0xff;
                pictureWidth[l1] = DataOperations.GetShort(metaData, i);
                i += 2;
                pictureHeight[l1] = DataOperations.GetShort(metaData, i);
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

                //using (var stream = System.IO.File.OpenWrite("c:/jpg/" + _pixels + ".jpg"))
                //{

                //    var size = pictureWidth[width] * pictureHeight[width];
                //    Color[] colorz = new Color[size];
                //    Array.Copy(clr.ToArray(), colorz, clr.Count);

                //    UnpackedImages[width] = new Texture2D(graphics, pictureWidth[width], pictureHeight[width], false, SurfaceFormat.Color);
                //    //UnpackedImages[_pixels].
                //    UnpackedImages[width].SetData(colorz);
                //    UnpackedImages[width].SaveAsJpeg(stream, UnpackedImages[_pixels].Width, UnpackedImages[width].Height);
                //}
            }

        }

        public void SetSleepSprite(int pictureIndex, sbyte[] spriteData)
        {
            int[] colors = pictureColors[pictureIndex] = new int[10200];
            pictureWidth[pictureIndex] = 255;
            pictureHeight[pictureIndex] = 40;
            pictureOffsetX[pictureIndex] = 0;
            pictureOffsetY[pictureIndex] = 0;
            pictureAssumedWidth[pictureIndex] = 255;
            pictureAssumedHeight[pictureIndex] = 40;
            hasTransparentBackground[pictureIndex] = false;
            int color = 0;
            int off = 1;
            int x;
            try
            {
                for (x = 0; x < 255; )
                {
                    int i1 = spriteData[off++] & 0xff;
                    for (int k1 = 0; k1 < i1; k1 += 1)
                    {
                        colors[x++] = color;
                    }

                    color = 0xffffff - color;
                }

                for (int y = 1; y < 40; y += 1)
                {
                    for (int l1 = 0; l1 < 255; )
                    {
                        int i2 = spriteData[off++] & 0xff;
                        for (int j2 = 0; j2 < i2; j2 += 1)
                        {
                            colors[x] = colors[x - 255];
                            x += 1;
                            l1 += 1;
                        }

                        if (l1 < 255)
                        {
                            colors[x] = 0xffffff - colors[x - 255];
                            x += 1;
                            l1 += 1;
                        }
                    }

                }
            }
            catch (Exception e)
            {
                //e.printStackTrace();
                logger.Error(GameOperation.RenderSprite, "An error has occurred while applying the image.", e);
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

            //using (var stream = System.IO.File.OpenWrite("c:/jpg/" + _pixels + ".jpg"))
            //{
            //    UnpackedImages[_pixels] = new Texture2D(graphics, pictureWidth[_pixels], pictureHeight[_pixels]);
            //    UnpackedImages[_pixels].SetData(colors);
            //    UnpackedImages[_pixels].SaveAsJpeg(stream, UnpackedImages[_pixels].Width, UnpackedImages[_pixels].Height);
            //}

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
            //Color[] clrs = new Color[pictureWidth[_pixels] * pictureHeight[_pixels]];
            //var p = 0;
            //List<Color> colors = new List<Color>();
            //for (int j = 0; j + 3 < ai1.Length; j += 3)
            //{
            //    var r = ai1[j + 2] & 0xff;
            //    var g = ai1[j + 1] & 0xff;
            //    var b = ai1[j + 0] & 0xff;
            //    colors.Add(new Color(r, g, b, 255));
            //    clrs[p++] = colors.Last();
            //}
            //using (var stream = System.IO.File.OpenWrite("c:/jpg/" + _pixels + ".jpg"))
            //{
            //    try
            //    {
            //        UnpackedImages[_pixels] = new Texture2D(graphics, pictureWidth[_pixels], pictureHeight[_pixels]);
            //        UnpackedImages[_pixels].SetData(clrs);
            //        UnpackedImages[_pixels].SaveAsJpeg(stream, UnpackedImages[_pixels].Width, UnpackedImages[_pixels].Height);
            //    }
            //    catch { }
            //}

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

            //***********************
            //** Lets see if we can output this image aswell!
            //***********************
            //var p = 0;
            //var pix = new int[pictureWidth[_pixels] * pictureHeight[_pixels]];
            //List<Color> colors = new List<Color>();
            //Color[] clrs = new Color[pictureWidth[_pixels] * pictureHeight[_pixels]];

            for (int x1 = x; x1 < x + width; x1 += 1)
            {
                for (int y1 = y; y1 < y + height; y1 += 1)
                {
                    //try
                    //{
                    //    pix[y] = pixels[_w + _h * gameWidth];
                    //    var bytes = BitConverter.GetBytes(pix[y]);
                    //    var r = bytes[2];
                    //    var g = bytes[1];
                    //    var b = bytes[0];
                    //    colors.Add(Color.FromNonPremultiplied(r, g, b, 255));
                    //    clrs[p++] = colors.Last();
                    //}
                    //catch { }
                    pictureColors[pictureIndex][k++] = pixels[x1 + y1 * gameWidth];
                }
            }

            //using (var stream = System.IO.File.OpenWrite("c:/jpg/" + _pixels + ".jpg"))
            //{
            //    try
            //    {
            //        UnpackedImages[_pixels] = new Texture2D(graphics, pictureWidth[_pixels], pictureHeight[_pixels]);
            //        UnpackedImages[_pixels].SetData(clrs);
            //        UnpackedImages[_pixels].SaveAsJpeg(stream, UnpackedImages[_pixels].Width, UnpackedImages[_pixels].Height);
            //    }
            //    catch { }
            //}
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

            //***********************
            //** Lets see if we can output this image aswell!
            //***********************
            //var p = 0;
            //var pix = new int[pictureWidth[_pixels] * pictureHeight[_pixels]];
            //List<Color> colors = new List<Color>();
            //Color[] clrs = new Color[pictureWidth[_pixels] * pictureHeight[_pixels]];

            for (int l = y; l < y + height; l += 1)
            {
                for (int i1 = x; i1 < x + width; i1 += 1)
                {
                    //try
                    //{
                    //    pix[y] = pixels[_w + _h * gameWidth];
                    //    var bytes = BitConverter.GetBytes(pix[y]);
                    //    var r = bytes[2];
                    //    var g = bytes[1];
                    //    var b = bytes[0];
                    //    colors.Add(Color.FromNonPremultiplied(r, g, b, 255));
                    //    clrs[p++] = colors.Last();
                    //}
                    //catch { }

                    pictureColors[pictureIndex][k++] = pixels[i1 + l * gameWidth];

                }

            }

            //using (var stream = System.IO.File.OpenWrite("c:/jpg/" + _pixels + ".jpg"))
            //{
            //    try
            //    {
            //        UnpackedImages[_pixels] = new Texture2D(graphics, pictureWidth[_pixels], pictureHeight[_pixels]);
            //        UnpackedImages[_pixels].SetData(clrs);
            //        UnpackedImages[_pixels].SaveAsJpeg(stream, UnpackedImages[_pixels].Width, UnpackedImages[_pixels].Height);
            //    }
            //    catch { }
            //}
        }

        public void DrawPicture(int x, int y, int pictureIndex)
        {
            if (hasTransparentBackground[pictureIndex])
            {
                x += pictureOffsetX[pictureIndex];
                y += pictureOffsetY[pictureIndex];
            }
            int i1 = x + y * gameWidth;
            int j1 = 0;
            int k1 = pictureHeight[pictureIndex];
            int l1 = pictureWidth[pictureIndex];
            int i2 = gameWidth - l1;
            int j2 = 0;
            if (y < imageY)
            {
                int k2 = imageY - y;
                k1 -= k2;
                y = imageY;
                j1 += k2 * l1;
                i1 += k2 * gameWidth;
            }
            if (y + k1 >= imageHeight)
            {
                k1 -= y + k1 - imageHeight + 1;
            }

            if (x < imageX)
            {
                int l2 = imageX - x;
                l1 -= l2;
                x = imageX;
                j1 += l2;
                i1 += l2;
                j2 += l2;
                i2 += l2;
            }
            if (x + l1 >= imageWidth)
            {
                int i3 = x + l1 - imageWidth + 1;
                l1 -= i3;
                j2 += i3;
                i2 += i3;
            }
            if (l1 <= 0 || k1 <= 0)
            {
                return;
            }

            byte byte0 = 1;
            if (interlace)
            {
                byte0 = 2;
                i2 += gameWidth;
                j2 += pictureWidth[pictureIndex];
                if ((y & 1) != 0)
                {
                    i1 += gameWidth;
                    k1 -= 1;
                }
            }
            if (pictureColors[pictureIndex] is null)
            {
                DrawSpriteTextured(ref pixels, pictureColorIndexes[pictureIndex], pictureColor[pictureIndex], j1, i1, l1, k1, i2, j2, byte0);
                return;
            }
            else
            {
                DrawSpriteOpaque(ref pixels, pictureColors[pictureIndex], 0, j1, i1, l1, k1, i2, j2, byte0);
                return;
            }
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
                int j3 = x + y * gameWidth;
                int l3 = gameWidth - width;
                if (y < imageY)
                {
                    int i4 = imageY - y;
                    height -= i4;
                    y = 0;
                    j3 += i4 * gameWidth;
                    j2 += l2 * i4;
                }
                if (y + height >= imageHeight)
                {
                    height -= y + height - imageHeight + 1;
                }

                if (x < imageX)
                {
                    int j4 = imageX - x;
                    width -= j4;
                    x = 0;
                    j3 += j4;
                    i2 += k2 * j4;
                    l3 += j4;
                }
                if (x + width >= imageWidth)
                {
                    int k4 = x + width - imageWidth + 1;
                    width -= k4;
                    l3 += k4;
                }
                byte byte0 = 1;
                if (interlace)
                {
                    byte0 = 2;
                    l3 += gameWidth;
                    l2 += l2;
                    if ((y & 1) != 0)
                    {
                        j3 += gameWidth;
                        height -= 1;
                    }
                }
                DrawSpriteTransparent(ref pixels, pictureColors[index], 0, i2, j2, j3, l3, width, height, k2, l2, k1, byte0);
                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderEntity, "Error in sprite clipping routine.");
            }
        }

        public void DrawPicture(int x, int y, int index, int i1)
        {
            if (hasTransparentBackground[index])
            {
                x += pictureOffsetX[index];
                y += pictureOffsetY[index];
            }
            int j1 = x + y * gameWidth;
            int k1 = 0;
            int l1 = pictureHeight[index];
            int i2 = pictureWidth[index];
            int j2 = gameWidth - i2;
            int k2 = 0;
            if (y < imageY)
            {
                int l2 = imageY - y;
                l1 -= l2;
                y = imageY;
                k1 += l2 * i2;
                j1 += l2 * gameWidth;
            }
            if (y + l1 >= imageHeight)
            {
                l1 -= y + l1 - imageHeight + 1;
            }

            if (x < imageX)
            {
                int i3 = imageX - x;
                i2 -= i3;
                x = imageX;
                k1 += i3;
                j1 += i3;
                k2 += i3;
                j2 += i3;
            }
            if (x + i2 >= imageWidth)
            {
                int j3 = x + i2 - imageWidth + 1;
                i2 -= j3;
                k2 += j3;
                j2 += j3;
            }
            if (i2 <= 0 || l1 <= 0)
            {
                return;
            }

            byte byte0 = 1;
            if (interlace)
            {
                byte0 = 2;
                j2 += gameWidth;
                k2 += pictureWidth[index];
                if ((y & 1) != 0)
                {
                    j1 += gameWidth;
                    l1 -= 1;
                }
            }
            if (pictureColors[index] is null)
            {
                DrawSpriteColorShiftedTextured(ref pixels, pictureColorIndexes[index], pictureColor[index], k1, j1, i2, l1, j2, k2, byte0, i1);
                return;
            }
            else
            {
                DrawSpriteColorShifted(ref pixels, pictureColors[index], 0, k1, j1, i2, l1, j2, k2, byte0, i1);
                return;
            }
        }

        public void DrawTransparentImage(int i, int k, int l, int i1, int j1, int k1)
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
                int k3 = i + k * gameWidth;
                int i4 = gameWidth - l;
                if (k < imageY)
                {
                    int j4 = imageY - k;
                    i1 -= j4;
                    k = 0;
                    k3 += j4 * gameWidth;
                    k2 += i3 * j4;
                }
                if (k + i1 >= imageHeight)
                {
                    i1 -= k + i1 - imageHeight + 1;
                }

                if (i < imageX)
                {
                    int k4 = imageX - i;
                    l -= k4;
                    i = 0;
                    k3 += k4;
                    j2 += l2 * k4;
                    i4 += k4;
                }
                if (i + l >= imageWidth)
                {
                    int l4 = i + l - imageWidth + 1;
                    l -= l4;
                    i4 += l4;
                }
                byte byte0 = 1;
                if (interlace)
                {
                    byte0 = 2;
                    i4 += gameWidth;
                    i3 += i3;
                    if ((k & 1) != 0)
                    {
                        k3 += gameWidth;
                        i1 -= 1;
                    }
                }
                DrawSpriteFlipped(ref pixels, pictureColors[j1], 0, j2, k2, k3, i4, l, i1, l2, i3, l1, byte0, k1);
                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderImage, "Error in sprite clipping routine.");
            }
        }

        public void DrawCharacterLegs(int i, int k, int l, int i1, int j1, int k1)
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
                int k3 = i + k * gameWidth;
                int i4 = gameWidth - l;
                if (k < imageY)
                {
                    int j4 = imageY - k;
                    i1 -= j4;
                    k = 0;
                    k3 += j4 * gameWidth;
                    k2 += i3 * j4;
                }
                if (k + i1 >= imageHeight)
                {
                    i1 -= k + i1 - imageHeight + 1;
                }

                if (i < imageX)
                {
                    int k4 = imageX - i;
                    l -= k4;
                    i = 0;
                    k3 += k4;
                    j2 += l2 * k4;
                    i4 += k4;
                }
                if (i + l >= imageWidth)
                {
                    int l4 = i + l - imageWidth + 1;
                    l -= l4;
                    i4 += l4;
                }
                byte byte0 = 1;
                if (interlace)
                {
                    byte0 = 2;
                    i4 += gameWidth;
                    i3 += i3;
                    if ((k & 1) != 0)
                    {
                        k3 += gameWidth;
                        i1 -= 1;
                    }
                }
                DrawSpriteFlippedColorShifted(ref pixels, pictureColors[j1], 0, j2, k2, k3, i4, l, i1, l2, i3, l1, byte0, k1);
                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderCharacter, "Error in sprite clipping routine.");
            }
        }

        private void DrawSpriteOpaque(ref int[] pixels, int[] colours, int currentColour, int srcOffset, int dstOffset, int width, int height,
                int dstStride, int srcStride, int rowStep)
        {
            int i = -(width >> 2);
            width = -(width & 3);
            for (int k = -height; k < 0; k += rowStep)
            {
                for (int l = i; l < 0; l += 1)
                {
                    currentColour = colours[srcOffset++];
                    if (currentColour != 0)
                    {
                        pixels[dstOffset++] = currentColour;
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    currentColour = colours[srcOffset++];
                    if (currentColour != 0)
                    {
                        pixels[dstOffset++] = currentColour;
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    currentColour = colours[srcOffset++];
                    if (currentColour != 0)
                    {
                        pixels[dstOffset++] = currentColour;
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    currentColour = colours[srcOffset++];
                    if (currentColour != 0)
                    {
                        pixels[dstOffset++] = currentColour;
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                for (int i1 = width; i1 < 0; i1 += 1)
                {
                    currentColour = colours[srcOffset++];
                    if (currentColour != 0)
                    {
                        pixels[dstOffset++] = currentColour;
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                dstOffset += dstStride;
                srcOffset += srcStride;
            }

        }

        private void DrawSpriteTextured(ref int[] pixels, sbyte[] colourIndexes, int[] colourLookup, int srcOffset, int dstOffset, int width, int height,
                int dstStride, int srcStride, int rowStep)
        {
            int i = -(width >> 2);
            width = -(width & 3);
            for (int k = -height; k < 0; k += rowStep)
            {
                for (int l = i; l < 0; l += 1)
                {
                    sbyte byte0 = colourIndexes[srcOffset++];
                    if (byte0 != 0)
                    {
                        pixels[dstOffset++] = colourLookup[byte0 & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    byte0 = colourIndexes[srcOffset++];
                    if (byte0 != 0)
                    {
                        pixels[dstOffset++] = colourLookup[byte0 & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    byte0 = colourIndexes[srcOffset++];
                    if (byte0 != 0)
                    {
                        pixels[dstOffset++] = colourLookup[byte0 & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }

                    byte0 = colourIndexes[srcOffset++];
                    if (byte0 != 0)
                    {
                        pixels[dstOffset++] = colourLookup[byte0 & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                for (int i1 = width; i1 < 0; i1 += 1)
                {
                    sbyte byte1 = colourIndexes[srcOffset++];
                    if (byte1 != 0)
                    {
                        pixels[dstOffset++] = colourLookup[byte1 & 0xff];
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                dstOffset += dstStride;
                srcOffset += srcStride;
            }

        }

        private void DrawSpriteTransparent(ref int[] pixels, int[] colours, int currentColour, int srcX, int srcY, int dstOffset, int dstStride,
                int width, int height, int xStep, int yStep, int srcWidth, int rowStep)
        {
            try
            {
                int i = srcX;
                for (int k = -height; k < 0; k += rowStep)
                {
                    int l = (srcY >> 16) * srcWidth;
                    for (int i1 = -width; i1 < 0; i1 += 1)
                    {
                        currentColour = colours[(srcX >> 16) + l];
                        if (currentColour != 0)
                        {
                            pixels[dstOffset++] = currentColour;
                        }
                        else
                        {
                            dstOffset += 1;
                        }

                        srcX += xStep;
                    }

                    srcY += yStep;
                    srcX = i;
                    dstOffset += dstStride;
                }

                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderSprite, "Error in the plot_scale routine.");
            }
        }

        private void DrawSpriteColorShifted(ref int[] pixels, int[] colours, int currentColour, int srcOffset, int dstOffset, int width, int height,
                int dstStride, int srcStride, int rowStep, int blendFactor)
        {
            int i = 256 - blendFactor;
            for (int k = -height; k < 0; k += rowStep)
            {
                for (int l = -width; l < 0; l += 1)
                {
                    currentColour = colours[srcOffset++];
                    if (currentColour != 0)
                    {
                        int i1 = pixels[dstOffset];
                        pixels[dstOffset++] = (int)(((currentColour & 0xff00ff) * blendFactor + (i1 & 0xff00ff) * i & 0xff00ff00) + ((currentColour & 0xff00) * blendFactor + (i1 & 0xff00) * i & 0xff0000) >> 8);
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                dstOffset += dstStride;
                srcOffset += srcStride;
            }

        }

        private void DrawSpriteColorShiftedTextured(ref int[] pixels, sbyte[] colourIndexes, int[] colourLookup, int srcOffset, int dstOffset, int width, int height,
                int dstStride, int srcStride, int rowStep, int blendFactor)
        {
            int i = 256 - blendFactor;
            for (int k = -height; k < 0; k += rowStep)
            {
                for (int l = -width; l < 0; l += 1)
                {
                    int i1 = colourIndexes[srcOffset++];
                    if (i1 != 0)
                    {
                        i1 = colourLookup[i1 & 0xff];
                        int j1 = pixels[dstOffset];
                        pixels[dstOffset++] = (int)(((i1 & 0xff00ff) * blendFactor + (j1 & 0xff00ff) * i & 0xff00ff00) + ((i1 & 0xff00) * blendFactor + (j1 & 0xff00) * i & 0xff0000) >> 8);
                    }
                    else
                    {
                        dstOffset += 1;
                    }
                }

                dstOffset += dstStride;
                srcOffset += srcStride;
            }

        }

        private void DrawSpriteFlipped(ref int[] pixels, int[] colours, int currentColour, int srcX, int srcY, int dstOffset, int dstStride,
                int width, int height, int xStep, int yStep, int srcWidth, int rowStep, int blendFactor)
        {
            int i = 256 - blendFactor;
            try
            {
                int k = srcX;
                for (int l = -height; l < 0; l += rowStep)
                {
                    int i1 = (srcY >> 16) * srcWidth;
                    for (int j1 = -width; j1 < 0; j1 += 1)
                    {
                        currentColour = colours[(srcX >> 16) + i1];
                        if (currentColour != 0)
                        {
                            int k1 = pixels[dstOffset];
                            pixels[dstOffset++] = (int)(((currentColour & 0xff00ff) * blendFactor + (k1 & 0xff00ff) * i & 0xff00ff00) + ((currentColour & 0xff00) * blendFactor + (k1 & 0xff00) * i & 0xff0000) >> 8);
                        }
                        else
                        {
                            dstOffset += 1;
                        }
                        srcX += xStep;
                    }

                    srcY += yStep;
                    srcX = k;
                    dstOffset += dstStride;
                }

                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderSprite, "Error in the tran_scale routine.");
            }
        }

        private void DrawSpriteFlippedColorShifted(ref int[] pixels, int[] colours, int currentColour, int srcX, int srcY, int dstOffset, int dstStride,
                int width, int height, int xStep, int yStep, int srcWidth, int rowStep, int color)
        {
            int red = color >> 16 & 0xff;
            int green = color >> 8 & 0xff;
            int blue = color & 0xff;
            try
            {
                int i1 = srcX;
                for (int j1 = -height; j1 < 0; j1 += rowStep)
                {
                    int k1 = (srcY >> 16) * srcWidth;
                    for (int l1 = -width; l1 < 0; l1 += 1)
                    {
                        currentColour = colours[(srcX >> 16) + k1];
                        if (currentColour != 0)
                        {
                            int i2 = currentColour >> 16 & 0xff;
                            int j2 = currentColour >> 8 & 0xff;
                            int k2 = currentColour & 0xff;
                            if (i2 == j2 && j2 == k2)
                            {
                                pixels[dstOffset++] = ((i2 * red >> 8) << 16) + ((j2 * green >> 8) << 8) + (k2 * blue >> 8);
                            }
                            else
                            {
                                pixels[dstOffset++] = currentColour;
                            }
                        }
                        else
                        {
                            dstOffset += 1;
                        }
                        srcX += xStep;
                    }

                    srcY += yStep;
                    srcX = i1;
                    dstOffset += dstStride;
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
            int i = gameWidth;
            int k = gameHeight;
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
            else
                if (scale == 128)
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
            else
                if (j4 > l5)
            {
                l5 = j4;
            }

            if (l4 < k5)
            {
                k5 = l4;
            }
            else
                if (l4 > l5)
            {
                l5 = l4;
            }

            if (j5 < k5)
            {
                k5 = j5;
            }
            else
                if (j5 > l5)
            {
                l5 = j5;
            }

            if (k5 < imageY)
            {
                k5 = imageY;
            }

            if (l5 > imageHeight)
            {
                l5 = imageHeight;
            }

            if (entityScanlineMinX is null || entityScanlineMinX.Length != k + 1)
            {
                entityScanlineMinX = new int[k + 1];
                entityScanlineMaxX = new int[k + 1];
                entityScanlineMinValue = new int[k + 1];
                entityScanlineMaxValue = new int[k + 1];
                entityScanlineMinExtra = new int[k + 1];
                entityScanlineMaxExtra = new int[k + 1];
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
            if (k6 > k - 1)
            {
                k6 = k - 1;
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
            if (k6 > k - 1)
            {
                k6 = k - 1;
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
            if (k6 > k - 1)
            {
                k6 = k - 1;
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
            if (k6 > k - 1)
            {
                k6 = k - 1;
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

            int l9 = k5 * i;
            int[] ai = pictureColors[pictureIndex];
            for (int i10 = k5; i10 < l5; i10 += 1)
            {
                int j10 = entityScanlineMinX[i10] >> 8;
                int k10 = entityScanlineMaxX[i10] >> 8;
                if (k10 - j10 <= 0)
                {
                    l9 += i;
                }
                else
                {
                    int l10 = entityScanlineMinValue[i10] << 9;
                    int i11 = ((entityScanlineMaxValue[i10] << 9) - l10) / (k10 - j10);
                    int j11 = entityScanlineMinExtra[i10] << 9;
                    int k11 = ((entityScanlineMaxExtra[i10] << 9) - j11) / (k10 - j10);
                    if (j10 < imageX)
                    {
                        l10 += (imageX - j10) * i11;
                        j11 += (imageX - j10) * k11;
                        j10 = imageX;
                    }
                    if (k10 > imageWidth)
                    {
                        k10 = imageWidth;
                    }

                    if (!interlace || (i10 & 1) == 0)
                    {
                        if (!hasTransparentBackground[pictureIndex])
                        {
                            DrawSpriteAlpha(ref pixels, ai, 0, l9 + j10, l10, j11, i11, k11, j10 - k10, j8);
                        }
                        else
                        {
                            DrawSpriteAlphaColorShifted(ref pixels, ai, 0, l9 + j10, l10, j11, i11, k11, j10 - k10, j8);
                        }
                    }

                    l9 += i;
                }
            }

        }

        private void DrawSpriteAlpha(ref int[] screenPixels, int[] colours, int currentColour, int dstOffset, int srcX, int srcY, int xStep,
                int yStep, int count, int srcWidth)
        {
            for (currentColour = count; currentColour < 0; currentColour += 1)
            {
                pixels[dstOffset++] = colours[(srcX >> 17) + (srcY >> 17) * srcWidth];
                srcX += xStep;
                srcY += yStep;
            }

        }

        private void DrawSpriteAlphaColorShifted(ref int[] screenPixels, int[] colours, int currentColour, int dstOffset, int srcX, int srcY, int xStep,
                int yStep, int count, int srcWidth)
        {
            for (int i = count; i < 0; i += 1)
            {
                currentColour = colours[(srcX >> 17) + (srcY >> 17) * srcWidth];
                if (currentColour != 0)
                {
                    pixels[dstOffset++] = currentColour;
                }
                else
                {
                    dstOffset += 1;
                }

                srcX += xStep;
                srcY += yStep;
            }

        }

        public virtual void DrawVisibleEntity(int i, int k, int l, int i1, int j1, int k1, int l1)
        {
            DrawEntity(i, k, l, i1, j1);
        }

        public virtual void DrawImage(int x, int y, int width, int height, int j1, int k1, int l1,
                int i2, bool flag)
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
                int k4 = y * gameWidth;
                j3 += x << 16;
                if (y < imageY)
                {
                    int i5 = imageY - y;
                    height -= i5;
                    y = imageY;
                    k4 += i5 * gameWidth;
                    i3 += l3 * i5;
                    j3 += i4 * i5;
                }
                if (y + height >= imageHeight)
                {
                    height -= y + height - imageHeight + 1;
                }

                int j5 = k4 / gameWidth & 1;
                if (!interlace)
                {
                    j5 = 2;
                }

                if (l1 == 0xffffff)
                {
                    if (pictureColors[j1] is not null)
                    {
                        if (!flag)
                        {
                            DrawSpriteFlatShaded(pixels, pictureColors[j1], 0, l2, i3, k4, width, height, k3, l3, j2, k1, j3, i4, j5);
                            return;
                        }
                        else
                        {
                            DrawSpriteFlatShaded(pixels, pictureColors[j1], 0, (pictureWidth[j1] << 16) - l2 - 1, i3, k4, width, height, -k3, l3, j2, k1, j3, i4, j5);
                            return;
                        }
                    }

                    if (!flag)
                    {
                        DrawSpriteFlatShadedTextured(pixels, pictureColorIndexes[j1], pictureColor[j1], 0, l2, i3, k4, width, height, k3, l3, j2, k1, j3, i4, j5);
                        return;
                    }
                    else
                    {
                        DrawSpriteFlatShadedTextured(pixels, pictureColorIndexes[j1], pictureColor[j1], 0, (pictureWidth[j1] << 16) - l2 - 1, i3, k4, width, height, -k3, l3, j2, k1, j3, i4, j5);
                        return;
                    }
                }
                if (pictureColors[j1] is not null)
                {
                    if (!flag)
                    {
                        DrawSpriteFlatShadedAlt(pixels, pictureColors[j1], 0, l2, i3, k4, width, height, k3, l3, j2, k1, l1, j3, i4, j5);
                        return;
                    }
                    else
                    {
                        DrawSpriteFlatShadedAlt(pixels, pictureColors[j1], 0, (pictureWidth[j1] << 16) - l2 - 1, i3, k4, width, height, -k3, l3, j2, k1, l1, j3, i4, j5);
                        return;
                    }
                }

                if (!flag)
                {
                    DrawSpriteFlatShadedTexturedAlt(pixels, pictureColorIndexes[j1], pictureColor[j1], 0, l2, i3, k4, width, height, k3, l3, j2, k1, l1, j3, i4, j5);
                    return;
                }
                else
                {
                    DrawSpriteFlatShadedTexturedAlt(pixels, pictureColorIndexes[j1], pictureColor[j1], 0, (pictureWidth[j1] << 16) - l2 - 1, i3, k4, width, height, -k3, l3, j2, k1, l1, j3, i4, j5);
                    return;
                }
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderImage, "Error in sprite clipping routine.");
            }
        }

        private void DrawSpriteFlatShaded(int[] pixels, int[] colours, int currentColour, int srcX, int srcY, int dstOffset, int width,
                int height, int xStep, int yStep, int srcWidth, int primaryColour, int xPosFixed, int xPosStep,
                int interlaceFlag)
        {
            int i1 = primaryColour >> 16 & 0xff;
            int j1 = primaryColour >> 8 & 0xff;
            int k1 = primaryColour & 0xff;
            try
            {
                int l1 = srcX;
                for (int i2 = -height; i2 < 0; i2 += 1)
                {
                    int j2 = (srcY >> 16) * srcWidth;
                    int k2 = xPosFixed >> 16;
                    int l2 = width;
                    if (k2 < imageX)
                    {
                        int i3 = imageX - k2;
                        l2 -= i3;
                        k2 = imageX;
                        srcX += xStep * i3;
                    }
                    if (k2 + l2 >= imageWidth)
                    {
                        int j3 = k2 + l2 - imageWidth;
                        l2 -= j3;
                    }
                    interlaceFlag = 1 - interlaceFlag;
                    if (interlaceFlag != 0)
                    {
                        for (int k3 = k2; k3 < k2 + l2; k3 += 1)
                        {
                            currentColour = colours[(srcX >> 16) + j2];
                            if (currentColour != 0)
                            {
                                int i = currentColour >> 16 & 0xff;
                                int k = currentColour >> 8 & 0xff;
                                int l = currentColour & 0xff;
                                if (i == k && k == l)
                                {
                                    pixels[k3 + dstOffset] = ((i * i1 >> 8) << 16) + ((k * j1 >> 8) << 8) + (l * k1 >> 8);
                                }
                                else
                                {
                                    pixels[k3 + dstOffset] = currentColour;
                                }
                            }
                            srcX += xStep;
                        }

                    }
                    srcY += yStep;
                    srcX = l1;
                    dstOffset += gameWidth;
                    xPosFixed += xPosStep;
                }

                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderSprite, "Error in transparent sprite plot routine.");
            }
        }

        private void DrawSpriteFlatShadedAlt(int[] pixels, int[] colours, int currentColour, int srcX, int srcY, int dstOffset, int width,
                int height, int xStep, int yStep, int srcWidth, int primaryColour, int secondaryColour, int xPosFixed,
                int xPosStep, int interlaceFlag)
        {
            int i1 = primaryColour >> 16 & 0xff;
            int j1 = primaryColour >> 8 & 0xff;
            int k1 = primaryColour & 0xff;
            int l1 = secondaryColour >> 16 & 0xff;
            int i2 = secondaryColour >> 8 & 0xff;
            int j2 = secondaryColour & 0xff;
            try
            {
                int k2 = srcX;
                for (int l2 = -height; l2 < 0; l2 += 1)
                {
                    int i3 = (srcY >> 16) * srcWidth;
                    int j3 = xPosFixed >> 16;
                    int k3 = width;
                    if (j3 < imageX)
                    {
                        int l3 = imageX - j3;
                        k3 -= l3;
                        j3 = imageX;
                        srcX += xStep * l3;
                    }
                    if (j3 + k3 >= imageWidth)
                    {
                        int i4 = j3 + k3 - imageWidth;
                        k3 -= i4;
                    }
                    interlaceFlag = 1 - interlaceFlag;
                    if (interlaceFlag != 0)
                    {
                        for (int j4 = j3; j4 < j3 + k3; j4 += 1)
                        {
                            currentColour = colours[(srcX >> 16) + i3];
                            if (currentColour != 0)
                            {
                                int i = currentColour >> 16 & 0xff;
                                int k = currentColour >> 8 & 0xff;
                                int l = currentColour & 0xff;
                                if (i == k && k == l)
                                {
                                    pixels[j4 + dstOffset] = ((i * i1 >> 8) << 16) + ((k * j1 >> 8) << 8) + (l * k1 >> 8);
                                }
                                else
                                    if (i == 255 && k == l)
                                {
                                    pixels[j4 + dstOffset] = ((i * l1 >> 8) << 16) + ((k * i2 >> 8) << 8) + (l * j2 >> 8);
                                }
                                else
                                {
                                    pixels[j4 + dstOffset] = currentColour;
                                }
                            }
                            srcX += xStep;
                        }

                    }
                    srcY += yStep;
                    srcX = k2;
                    dstOffset += gameWidth;
                    xPosFixed += xPosStep;
                }

                return;
            }
            catch (Exception)
            {
                logger.Error(GameOperation.RenderSprite, "Error in transparent sprite plot routine.");
            }
        }

        private void DrawSpriteFlatShadedTextured(int[] pixels, sbyte[] colourIndexes, int[] colourLookup, int currentColour, int srcX, int srcY, int dstOffset,
                int width, int height, int xStep, int yStep, int srcWidth, int primaryColour, int xPosFixed,
                int xPosStep, int interlaceFlag)
        {
            int i1 = primaryColour >> 16 & 0xff;
            int j1 = primaryColour >> 8 & 0xff;
            int k1 = primaryColour & 0xff;
            try
            {
                int l1 = srcX;
                for (int i2 = -height; i2 < 0; i2 += 1)
                {
                    int j2 = (srcY >> 16) * srcWidth;
                    int k2 = xPosFixed >> 16;
                    int l2 = width;
                    if (k2 < imageX)
                    {
                        int i3 = imageX - k2;
                        l2 -= i3;
                        k2 = imageX;
                        srcX += xStep * i3;
                    }
                    if (k2 + l2 >= imageWidth)
                    {
                        int j3 = k2 + l2 - imageWidth;
                        l2 -= j3;
                    }
                    interlaceFlag = 1 - interlaceFlag;
                    if (interlaceFlag != 0)
                    {
                        for (int k3 = k2; k3 < k2 + l2; k3 += 1)
                        {
                            currentColour = colourIndexes[(srcX >> 16) + j2] & 0xff;
                            if (currentColour != 0)
                            {
                                currentColour = colourLookup[currentColour];
                                int i = currentColour >> 16 & 0xff;
                                int k = currentColour >> 8 & 0xff;
                                int l = currentColour & 0xff;
                                if (i == k && k == l)
                                {
                                    pixels[k3 + dstOffset] = ((i * i1 >> 8) << 16) + ((k * j1 >> 8) << 8) + (l * k1 >> 8);
                                }
                                else
                                {
                                    pixels[k3 + dstOffset] = currentColour;
                                }
                            }
                            srcX += xStep;
                        }

                    }
                    srcY += yStep;
                    srcX = l1;
                    dstOffset += gameWidth;
                    xPosFixed += xPosStep;
                }

                return;
            }
            catch (Exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    "Error in transparent sprite plot routine.");
            }
        }

        private void DrawSpriteFlatShadedTexturedAlt(int[] pixels, sbyte[] colourIndexes, int[] colourLookup, int currentColour, int srcX, int srcY, int dstOffset,
                int width, int height, int xStep, int yStep, int srcWidth, int primaryColour, int secondaryColour,
                int xPosFixed, int xPosStep, int interlaceFlag)
        {
            int i1 = primaryColour >> 16 & 0xff;
            int j1 = primaryColour >> 8 & 0xff;
            int k1 = primaryColour & 0xff;
            int l1 = secondaryColour >> 16 & 0xff;
            int i2 = secondaryColour >> 8 & 0xff;
            int j2 = secondaryColour & 0xff;
            try
            {
                int k2 = srcX;
                for (int l2 = -height; l2 < 0; l2 += 1)
                {
                    int i3 = (srcY >> 16) * srcWidth;
                    int j3 = xPosFixed >> 16;
                    int k3 = width;
                    if (j3 < imageX)
                    {
                        int l3 = imageX - j3;
                        k3 -= l3;
                        j3 = imageX;
                        srcX += xStep * l3;
                    }
                    if (j3 + k3 >= imageWidth)
                    {
                        int i4 = j3 + k3 - imageWidth;
                        k3 -= i4;
                    }
                    interlaceFlag = 1 - interlaceFlag;
                    if (interlaceFlag != 0)
                    {
                        for (int j4 = j3; j4 < j3 + k3; j4 += 1)
                        {
                            currentColour = colourIndexes[(srcX >> 16) + i3] & 0xff;
                            if (currentColour != 0)
                            {
                                currentColour = colourLookup[currentColour];
                                int i = currentColour >> 16 & 0xff;
                                int k = currentColour >> 8 & 0xff;
                                int l = currentColour & 0xff;
                                if (i == k && k == l)
                                {
                                    pixels[j4 + dstOffset] = ((i * i1 >> 8) << 16) + ((k * j1 >> 8) << 8) + (l * k1 >> 8);
                                }
                                else
                                    if (i == 255 && k == l)
                                {
                                    pixels[j4 + dstOffset] = ((i * l1 >> 8) << 16) + ((k * i2 >> 8) << 8) + (l * j2 >> 8);
                                }
                                else
                                {
                                    pixels[j4 + dstOffset] = currentColour;
                                }
                            }
                            srcX += xStep;
                        }

                    }
                    srcY += yStep;
                    srcX = k2;
                    dstOffset += gameWidth;
                    xPosFixed += xPosStep;
                }

                return;
            }
            catch (Exception)
            {
                logger.Error(
                    GameOperation.RenderSprite,
                    "Error in transparent sprite plot routine.");
            }
        }

        //// GameApplet should be xna Game later.
        //public static void cdj(SpriteFont _pixels, /*FontMetrics y,*/ char destX, int destY, GameApplet startColor, int endColor, bool arg6)
        //{

        //    int x = (int)_pixels.MeasureString(destX.ToString()).X;// y.charWidth(destX);
        //    int y = x;
        //    if (arg6)
        //        try
        //        {
        //            if (destX == '/')
        //                arg6 = false;
        //            if (destX == 'f' || destX == 't' || destX == 'w' || destX == 'v' || destX == 'y' || destX == 'x' || destX == 'y' || destX == 'A' || destX == 'V' || destX == 'W')
        //                x += 1;
        //        }
        //        catch (Exception) { }

        //    // var ascent= _pixels.MeasureString(str)

        //    int _w = y.getMaxAscent();
        //    // int _w = ascent.X;
        //    int _h = y.getMaxAscent() + y.getMaxDescent();
        //    // il = ascent.X + ascent.Y
        //    int j1 = y.getHeight();

        //    // int j1 = ascent.Y;

        //    var image = startColor.createImage(x, _h);
        //    var g = image.GetGraphics();
        //    g.SetColor(Color.Black);
        //    g.FillRect(0, 0, x, _h);
        //    g.SetColor(Color.White);
        //    g.SetFont(_pixels);
        //    g.DrawString(destX.ToString(), 0, _w);
        //    if (arg6)
        //        g.DrawString(destX.ToString(), 1, _w);
        //    int[] ai = new int[x * _h];
        //    PixelGrabber pixelgrabber = new PixelGrabber(image, 0, 0, x, _h, ai, 0, x);
        //    try
        //    {
        //        pixelgrabber.grabPixels();
        //    }
        //    catch
        //    {
        //        return;
        //    }
        //    image.Flush();
        //    image = null;
        //    int k1 = 0;
        //    int width = 0;
        //    int i2 = x;
        //    int j2 = _h;
        //label0:
        //    for (int k2 = 0; k2 < _h; k2 += 1)
        //    {
        //        for (int l2 = 0; l2 < x; l2 += 1)
        //        {
        //            int j3 = ai[l2 + k2 * x];
        //            if ((j3 & 0xffffff) == 0)
        //                continue;
        //            width = k2;
        //            goto label1;
        //            // break label0;
        //        }

        //    }

        //label1:
        //    for (int i3 = 0; i3 < x; i3 += 1)
        //    {
        //        for (int k3 = 0; k3 < _h; k3 += 1)
        //        {
        //            int i4 = ai[i3 + k3 * x];
        //            if ((i4 & 0xffffff) == 0)
        //                continue;
        //            k1 = i3;
        //            goto label2;
        //            // break label1;
        //        }

        //    }

        //label2:
        //    for (int l3 = _h - 1; l3 >= 0; l3 -= 1)
        //    {
        //        for (int j4 = 0; j4 < x; j4 += 1)
        //        {
        //            int l4 = ai[j4 + l3 * x];
        //            if ((l4 & 0xffffff) == 0)
        //                continue;
        //            j2 = l3 + 1;
        //            goto label3;
        //            // break label2;
        //        }

        //    }

        //label3:
        //    for (int k4 = x - 1; k4 >= 0; k4 -= 1)
        //    {
        //        for (int i5 = 0; i5 < _h; i5 += 1)
        //        {
        //            int k5 = ai[k4 + i5 * x];
        //            if ((k5 & 0xffffff) == 0)
        //                continue;
        //            i2 = k4 + 1;
        //            goto label4;
        //            // break label3;
        //        }

        //    }
        //label4:
        //    cae[destY * 9] = (byte)(cad / 16384);
        //    cae[destY * 9 + 1] = (byte)(cad / 128 & 0x7f);
        //    cae[destY * 9 + 2] = (byte)(cad & 0x7f);
        //    cae[destY * 9 + 3] = (byte)(i2 - k1);
        //    cae[destY * 9 + 4] = (byte)(j2 - width);
        //    cae[destY * 9 + 5] = (byte)k1;
        //    cae[destY * 9 + 6] = (byte)(_w - width);
        //    cae[destY * 9 + 7] = (byte)y;
        //    cae[destY * 9 + 8] = (byte)j1;
        //    for (int j5 = width; j5 < j2; j5 += 1)
        //    {
        //        for (int l5 = k1; l5 < i2; l5 += 1)
        //        {
        //            int i6 = ai[l5 + j5 * x] & 0xff;
        //            if (i6 > 30 && i6 < 230)
        //                fontShadowEnabled[endColor] = true;
        //            cae[cad++] = (byte)i6;
        //        }

        //    }

        //}

        public void DrawLabel(string s, int i, int k, int l, int i1)
        {
            DrawString(s, i - TextWidth(s, l), k, l, i1);
        }

        public void DrawText(string s, int i, int k, int l, int i1)
        {
            DrawString(s, i - TextWidth(s, l) / 2, k, l, i1);
        }

        //public int TextWidth(string s, int _w)
        //{
        //    return (int)GameClient.gameFont12.MeasureString(s).X;
        //}

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
                        y += TextHeightNumber(fontIndex);
                    }
                }

                if (i > 0)
                {
                    DrawText(text.Substring(k), x, y, fontIndex, colour);
                    return;
                }
            }
            catch (Exception exception)
            {
                logger.Error(GameOperation.RenderText, "Error in the centrepara routine.", exception);
                //exception.printStackTrace();
            }
        }

        public static List<StringDraw> stringsToDraw = [];

        public void DrawString(string text, int x, int y, int fontIndex, int colour)
        {
            try
            {
#warning fix real draw string

                //return;
                //GameClient.spriteBatch.BeginSafe();
                //GameClient.gameFont12
                //if (!GameClient.spriteBatch.BeginIsActive()) return;
                //GameClient.spriteBatch.DrawString(GameClient.gameFont12, _pixels, new Vector2(y, destX), Color.Red);

                //GameClient.spriteBatch.EndSafe();

                //return;

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
                        else if (text[i] == '~' && i + 4 < text.Length && text[i + 4] == '~')
                        {
                            char c = text[i + 1];
                            char c1 = text[i + 2];
                            char c2 = text[i + 3];
                            if (c >= '0' && c <= '9' && c1 >= '0' && c1 <= '9' && c2 >= '0' && c2 <= '9')
                            {
                                x = int.Parse(text.Substring(i + 1, i + 4));
                            }

                            i += 3;
                        } else if (text[i] != '@' && text[i] != '~')
                        {
                            int k = characterFontOffsetTable[text[i]];
                            if (loggedIn && !fontShadowEnabled[fontIndex] && colour != 0)
                            {
                                UnpackSpriteRow(k, x + 1, y, 0, abyte0, fontShadowEnabled[fontIndex]);
                            }

                            if (loggedIn && !fontShadowEnabled[fontIndex] && colour != 0)
                            {
                                UnpackSpriteRow(k, x, y + 1, 0, abyte0, fontShadowEnabled[fontIndex]);
                            }

                            UnpackSpriteRow(k, x, y, colour, abyte0, fontShadowEnabled[fontIndex]);
                            x += abyte0[k + 7];
                        }
                    }
                }
                catch { }

                //stringsToDraw.Add(new StringDraw
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
                // exception.printStackTrace();
                return;
            }
        }

        private void UnpackSpriteRow(int charOffset, int x, int y, int colour, sbyte[] fontData, bool useShadow)
        {
            int j1 = x + fontData[charOffset + 5];
            int k1 = y - fontData[charOffset + 6];
            int l1 = fontData[charOffset + 3];
            int i2 = fontData[charOffset + 4];
            int j2 = fontData[charOffset] * 16384 + fontData[charOffset + 1] * 128 + fontData[charOffset + 2];
            int k2 = j1 + k1 * gameWidth;
            int l2 = gameWidth - l1;
            int i3 = 0;
            if (k1 < imageY)
            {
                int j3 = imageY - k1;
                i2 -= j3;
                k1 = imageY;
                j2 += j3 * l1;
                k2 += j3 * gameWidth;
            }
            if (k1 + i2 >= imageHeight)
            {
                i2 -= k1 + i2 - imageHeight + 1;
            }

            if (j1 < imageX)
            {
                int k3 = imageX - j1;
                l1 -= k3;
                j1 = imageX;
                j2 += k3;
                k2 += k3;
                i3 += k3;
                l2 += k3;
            }
            if (j1 + l1 >= imageWidth)
            {
                int l3 = j1 + l1 - imageWidth + 1;
                l1 -= l3;
                i3 += l3;
                l2 += l3;
            }
            if (l1 > 0 && i2 > 0)
            {
                if (useShadow)
                {
                    DrawCharacterRow(ref pixels, fontData, colour, j2, k2, l1, i2, l2, i3);
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
                //exception.printStackTrace();
                return;
            }
        }

        private void DrawCharacterRow(ref int[] pixels, sbyte[] fontData, int colour, int glyphOffset, int pixelOffset, int glyphWidth, int glyphHeight,
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

        public int TextHeightNumber(int i)
        {
            //return (int)GameClient.gameFont12.MeasureString("A").Y;

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
            else
            {
                return GetCharacterWidth(i);
            }
        }

        public int GetCharacterWidth(int i)
        {
            if (i == 0)
            {
                return gameFonts[i][8] - 2;
            }
            else
            {
                return gameFonts[i][8] - 1;
            }
        }

        public int TextWidth(string text, int fontIndex)
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

        public void DrawPixels(int[][] pixels, int drawx, int drawy, int width, int height)
        {

            for (int x = drawx; x < drawx + width; x += 1)
            {
                for (int y = drawy; y < drawy + height; y += 1)
                {
                    this.pixels[x + y * gameWidth] = pixels[x - drawx][y - drawy];
                }
            }
        }

        public static int AddFont(sbyte[] bytes)
        {
            gameFonts[currentFont] = bytes;
            currentFont += 1;
            return currentFont - 1;
        }

        public int gameWidth;
        public int gameHeight;
        public int area;
        public int width;
        public int height;
        //ColorModel colorModel;
        public int[] pixels;
        //ImageConsumer imageConsumer;
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
        private int imageY;
        private int imageHeight;
        private int imageX;
        private int imageWidth;
        public bool interlace;
        private static readonly sbyte[][] gameFonts = new sbyte[50][];
        private static readonly int[] characterFontOffsetTable;
        public bool loggedIn;

        private readonly ILogger logger = NuciLoggerFactory.CreateLogger<GameImage>();

        public bool IsLoggedIn
        {
            get => loggedIn;
            set => loggedIn = value;
        }

        public Size2D GameSize => new(gameWidth, gameHeight);
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

        static GameImage()
        {
            string s = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!\"!$%^&*()-_=+[{]};:'@#~,<.>/?\\| ";
            characterFontOffsetTable = new int[256];
            for (int i = 0; i < 256; i += 1)
            {
                int k = s.IndexOf((char)i);
                if (k == -1)
                {
                    k = 74;
                }

                characterFontOffsetTable[i] = k * 9;
            }

        }

        public GraphicsDevice graphics { get; set; }
    }
}
