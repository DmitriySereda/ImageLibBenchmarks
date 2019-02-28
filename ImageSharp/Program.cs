using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace ImageSharp
{
    static class Program
    {
        private const string Filename = "w2.jpg";

        private static void Main()
        {
            var path = "../../../" + Filename;
            Stream fileStream = File.OpenRead(path);
            for (var i = 0; i < 50; i++)
            {
                fileStream.Position = 0;
                var watch = Stopwatch.StartNew();
                using (var image = Image.Load(fileStream))
                {
                    using (var outStream = new MemoryStream())
                    {
                        image.Mutate(x => x.BlackWhite());
                        image.Save(outStream, new PngEncoder());
                    }
                }

                watch.Stop();
                Console.WriteLine($"Operation {i + 1}: {watch.ElapsedMilliseconds}");
            }
            Console.ReadKey();
        }
    }
}
