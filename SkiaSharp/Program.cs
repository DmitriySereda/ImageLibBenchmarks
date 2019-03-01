using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SkiaSharp
{
    static class Program
    {
        private const string Filename = "w2.jpg";

        private static void Main()
        {
            var path = "../../../" + Filename;
            Stream fileStream = File.OpenRead(path); // open a stream to an image file


            for (int i = 0; i < 25; i++)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                fileStream.Position = 0;

                // decode the bitmap from the stream
                using (var stream = new SKManagedStream(fileStream))
                using (var bitmap = SKBitmap.Decode(stream))

                using (var paint = new SKPaint())
                {
                    //Black & White color matrix
                    paint.ColorFilter = paint.ColorFilter =
                        SKColorFilter.CreateColorMatrix(new float[]
                        {
                                1.5f, 1.5f, 1.5f, 0, -1.5f,
                                1.5f, 1.5f, 1.5f, 0, -1.5f,
                                1.5f, 1.5f, 1.5f, 0, -1.5f,
                                0,    0,    0,    1,  0
                        }); ;

                    Console.WriteLine($"Apply color filter {watch.ElapsedMilliseconds}");

                    using (var toBitmap = new SKBitmap(bitmap.Width, bitmap.Height, bitmap.ColorType, bitmap.AlphaType))
                    using (var canvas = new SKCanvas(toBitmap))
                    {
                        Console.WriteLine($"New canvas {watch.ElapsedMilliseconds}");

                        // draw the bitmap through the filter
                        canvas.DrawBitmap(bitmap, SKRect.Create(bitmap.Width, bitmap.Height), paint);
                        Console.WriteLine($"DrawBitmap {watch.ElapsedMilliseconds}");

                        canvas.Flush();

                        Console.WriteLine($"Flush {watch.ElapsedMilliseconds}");

						Console.WriteLine($"Before getPixels {watch.ElapsedMilliseconds}");

						//byte[] pixelLocation = toBitmap.Bytes;

						//File.WriteAllBytes(@"../../../image.bytes", pixelLocation);

                        var bmp = ToWriteableBitmap(toBitmap);

                        Console.WriteLine($"after write {watch.ElapsedMilliseconds}");
					}
                }

                watch.Stop();
                Console.WriteLine($"Total: {watch.ElapsedMilliseconds}");
                Console.WriteLine();
            }

            Console.ReadKey();
        }
        public static WriteableBitmap ToWriteableBitmap(SKBitmap skiaBitmap)
        {
            using (var image = SKImage.FromPixels(skiaBitmap.PeekPixels()))
            {
                return ToWriteableBitmap(image);
            }
        }

        public static WriteableBitmap ToWriteableBitmap(SKImage skiaImage)
        {
            var info = new SKImageInfo(skiaImage.Width, skiaImage.Height);
            var bitmap = new WriteableBitmap(info.Width, info.Height, 96, 96, PixelFormats.Pbgra32, null);
            bitmap.Lock();

            // copy
            using (var pixmap = new SKPixmap(info, bitmap.BackBuffer, bitmap.BackBufferStride))
            {
                skiaImage.ReadPixels(pixmap, 0, 0);
            }

            bitmap.AddDirtyRect(new Int32Rect(0, 0, info.Width, info.Height));
            bitmap.Unlock();
            return bitmap;
        }
    }

}
