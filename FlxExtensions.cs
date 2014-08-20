using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.Linq;

namespace org.flixel
{
    public static class FlxExtensions
    {
        public static uint FromARGB(byte alpha, byte red, byte green, byte blue)
        {
            return (uint)((alpha << 24) | (red << 16) |
                          (green << 8) | (blue << 0));
        }
        
        public static Color ToColor(this uint color)
        {
            byte a = (byte)(color >> 24);
            byte r = (byte)(color >> 16);
            byte g = (byte)(color >> 8);
            byte b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }

        public static uint ToUInt(this Color color)
        {
            return (uint)((color.A << 24) | (color.R << 16) |
                          (color.G << 8) | (color.B << 0));
        }

        public static byte GetAlpha(this int color)
        {
            return (byte)(color >> 24);
        }

        public static byte GetAlpha(this uint color)
        {
            return (byte)(color >> 24);
        }

        public static byte GetRed(this int color)
        {
            return (byte)(color >> 16);
        }

        public static byte GetRed(this uint color)
        {
            return (byte)(color >> 16);
        }

        public static byte GetGreen(this int color)
        {
            return (byte)(color >> 8);
        }

        public static byte GetGreen(this uint color)
        {
            return (byte)(color >> 8);
        }

        public static byte GetBlue(this int color)
        {
            return (byte)(color >> 0);
        }

        public static byte GetBlue(this uint color)
        {
            return (byte)(color >> 0);
        }
        
        public static void CopyPixels(this WriteableBitmap bitmap, WriteableBitmap source)
        {
            CopyPixels(bitmap, source, source.GetRect(), new IntPoint(), 1);
        }

        public static void CopyPixels(this WriteableBitmap bitmap, WriteableBitmap source, IntRect sourceRect, IntPoint destPoint)
        {
            CopyPixels(bitmap, source, sourceRect, destPoint, 1);
        }
        
        public static void CopyPixels(this WriteableBitmap bitmap, WriteableBitmap source, IntRect sourceRect, IntPoint destPoint, int zoom)
        {
            if (bitmap != null && source != null)
            {
                int bitmapWidth = bitmap.PixelWidth;
                int bitmapHeight = bitmap.PixelHeight;
                int sourceWidth = source.PixelWidth;
                int sourceHeight = source.PixelHeight;
                int sourceRectLeft = sourceRect.Left;
                int sourceRectRight = sourceRect.Right;
                int sourceRectTop = sourceRect.Top;
                int sourceRectBottom = sourceRect.Bottom;
                int destPointX = destPoint.X;
                int destPointY = destPoint.Y;
                int bitmapPixelsLength = bitmap.Pixels.Length;
                int sourcePixelsLength = source.Pixels.Length;

                for (int xCol = sourceRectLeft; xCol < sourceRectRight; ++xCol)
                {
                    for (int yRow = sourceRectBottom; yRow < sourceRectTop; ++yRow)
                    {
                        if (xCol >= 0 && xCol < sourceWidth &&
                            yRow >= 0 && yRow < sourceHeight)
                        {                        
                            int sourceIndex = yRow * sourceWidth + xCol;

                            for (int zoomXOffset = 0; zoomXOffset < zoom; ++zoomXOffset)
                            {
                                for (int zoomYOffset = 0; zoomYOffset < zoom; ++zoomYOffset)
                                {
                                    int destY = destPointY + (yRow * zoom) + zoomYOffset;
                                    int destX = destPointX + (xCol * zoom) - sourceRectLeft + zoomXOffset;

                                    if (destX >= 0 && destX < bitmapWidth &&
                                        destY >= 0 && destY < bitmapHeight)
                                    {
                                        int destIndex = destY * bitmapWidth + destX;
                                        //int destColor = bitmap.Pixels[destIndex];
                                        int sourceColor = source.Pixels[sourceIndex];
                                        //double sourceAlpha = (double)sourceColor.GetAlpha() / 255.0;
                                        byte sourceAlphaByte = sourceColor.GetAlpha();
                                        //double destAlpha = 1 - sourceAlpha;

                                        if (sourceAlphaByte != 0)
                                            bitmap.Pixels[destIndex] = sourceColor;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CopyPixels(this WriteableBitmap bitmap, WriteableBitmap source, IntRect sourceRect, IntPoint destPoint, int zoom, double sourceAlpha)
        {
            if (sourceAlpha != 0 && bitmap != null && source != null)
            {
                int bitmapWidth = bitmap.PixelWidth;
                int bitmapHeight = bitmap.PixelHeight;
                int sourceWidth = source.PixelWidth;
                int sourceHeight = source.PixelHeight;
                int sourceRectLeft = sourceRect.Left;
                int sourceRectRight = sourceRect.Right;
                int sourceRectTop = sourceRect.Top;
                int sourceRectBottom = sourceRect.Bottom;
                int destPointX = destPoint.X;
                int destPointY = destPoint.Y;
                int bitmapPixelsLength = bitmap.Pixels.Length;
                int sourcePixelsLength = source.Pixels.Length;

                for (int xCol = sourceRectLeft; xCol < sourceRectRight; ++xCol)
                {
                    for (int yRow = sourceRectBottom; yRow < sourceRectTop; ++yRow)
                    {
                        if (xCol >= 0 && xCol < sourceWidth &&
                            yRow >= 0 && yRow < sourceHeight)
                        {
                            int sourceIndex = yRow * sourceWidth + xCol;

                            for (int zoomXOffset = 0; zoomXOffset < zoom; ++zoomXOffset)
                            {
                                for (int zoomYOffset = 0; zoomYOffset < zoom; ++zoomYOffset)
                                {
                                    int destY = destPointY + (yRow * zoom) + zoomYOffset;
                                    int destX = destPointX + (xCol * zoom) - sourceRectLeft + zoomXOffset;

                                    if (destX >= 0 && destX < bitmapWidth &&
                                        destY >= 0 && destY < bitmapHeight)
                                    {
                                        int destIndex = destY * bitmapWidth + destX;
                                        int destColor = bitmap.Pixels[destIndex];
                                        int sourceColor = source.Pixels[sourceIndex];
                                        double destAlpha = 1 - sourceAlpha;

                                        bitmap.Pixels[destIndex] = unchecked((int)(FlxExtensions.FromARGB(
                                                0xFF,
                                                (byte)(sourceColor.GetRed() * sourceAlpha + destColor.GetRed() * destAlpha),
                                                (byte)(sourceColor.GetGreen() * sourceAlpha + destColor.GetGreen() * destAlpha),
                                                (byte)(sourceColor.GetBlue() * sourceAlpha + destColor.GetBlue() * destAlpha)
                                            )));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CopyPixelsXReverse(this WriteableBitmap bitmap, WriteableBitmap source)
        {
            CopyPixelsXReverse(bitmap, source, source.GetRect(), new IntPoint());
        }
        
        public static void CopyPixelsXReverse(this WriteableBitmap bitmap, WriteableBitmap source, IntRect sourceRect, IntPoint destPoint)
        {
            if (bitmap != null && source != null)
            {
                int bitmapWidth = bitmap.PixelWidth;
                int bitmapHeight = bitmap.PixelHeight;
                int sourceWidth = source.PixelWidth;
                int sourceHeight = source.PixelHeight;
                int sourceRectLeft = sourceRect.Left;
                int sourceRectRight = sourceRect.Right;
                int sourceRectTop = sourceRect.Top;
                int sourceRectBottom = sourceRect.Bottom;
                int destPointX = destPoint.X;
                int destPointY = destPoint.Y;
                int bitmapPixelsLength = bitmap.Pixels.Length;
                int sourcePixelsLength = source.Pixels.Length;

                for (int xCol = sourceRectLeft; xCol < sourceRectRight; ++xCol)
                {
                    for (int yRow = sourceRectBottom; yRow < sourceRectTop; ++yRow)
                    {
                        if (xCol >= 0 && xCol < sourceWidth &&
                            yRow >= 0 && yRow < sourceHeight)
                        {
                            int sourceIndex = yRow * sourceWidth + xCol; 
                            int destY = destPointY + yRow;
                            int destX = sourceWidth - (destPointX + xCol - sourceRectLeft);

                            if (destX >= 0 && destX < bitmapWidth &&
                                        destY >= 0 && destY < bitmapHeight)
                            {
                                int destIndex = destY * bitmapWidth + destX;
                                //int destColor = bitmap.Pixels[destIndex];
                                int sourceColor = source.Pixels[sourceIndex];
                                //double sourceAlpha = (double)sourceColor.GetAlpha() / 255.0;
                                byte sourceAlphaByte = sourceColor.GetAlpha();
                                //double destAlpha = 1 - sourceAlpha;

                                if (sourceAlphaByte != 0)
                                    bitmap.Pixels[destIndex] = sourceColor;
                            }
                        }                                                    
                    }
                }
            }
        }

        public static void CopyPixels(this WriteableBitmap bitmap, WriteableBitmap source, IntRect sourceRect, TransformGroup tg)
        {
            if (bitmap != null && source != null)
            {
                int bitmapWidth = bitmap.PixelWidth;
                int bitmapHeight = bitmap.PixelHeight;
                int sourceWidth = source.PixelWidth;
                int sourceHeight = source.PixelHeight;
                int sourceRectLeft = sourceRect.Left;
                int sourceRectRight = sourceRect.Right;
                int sourceRectTop = sourceRect.Top;
                int sourceRectBottom = sourceRect.Bottom;
                int bitmapPixelsLength = bitmap.Pixels.Length;
                int sourcePixelsLength = source.Pixels.Length;

                for (int xCol = sourceRectLeft; xCol < sourceRectRight; ++xCol)
                {
                    for (int yRow = sourceRectBottom; yRow < sourceRectTop; ++yRow)
                    {
                        if (xCol >= 0 && xCol < sourceWidth &&
                            yRow >= 0 && yRow < sourceHeight)
                        {
                            int sourceIndex = yRow * sourceWidth + xCol;
                            IntPoint destPoint = new IntPoint(tg.Transform(new Point(xCol, yRow)));

                            if (destPoint.X >= 0 && destPoint.X < bitmapWidth &&
                                destPoint.Y >= 0 && destPoint.Y < bitmapHeight)
                            {
                                int destIndex = destPoint.Y * bitmapWidth + destPoint.X;

                                if (source.Pixels[sourceIndex].GetAlpha() != 0)
                                    bitmap.Pixels[destIndex] = source.Pixels[sourceIndex];
                            }
                        }
                    }
                }
            }           
        }

        public static IntRect GetRect(this WriteableBitmap bitmap)
        {
            return new IntRect(0, bitmap.PixelWidth, bitmap.PixelHeight, 0);
        }

        public static void SetColor(this WriteableBitmap bitmap, int color)
        {
            for (int i = 0; i < bitmap.Pixels.Length; ++i) 
                bitmap.Pixels[i] = color;
        }

        public static void SetColor(this WriteableBitmap bitmap, uint color)
        {
            int colorInt = unchecked((int)color);
            for (int i = 0; i < bitmap.Pixels.Length; ++i)
                bitmap.Pixels[i] = colorInt;
        }

        public static int[] GetEnumValues<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            return (
              from field in type.GetFields(BindingFlags.Public | BindingFlags.Static)
              where field.IsLiteral
              select (int)field.GetValue(type)).ToArray();
        }

        public static string[] GetEnumStrings<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException("Type '" + type.Name + "' is not an enum");

            return (
              from field in type.GetFields(BindingFlags.Public | BindingFlags.Static)
              where field.IsLiteral
              select field.Name).ToArray();
        }
    }
}
