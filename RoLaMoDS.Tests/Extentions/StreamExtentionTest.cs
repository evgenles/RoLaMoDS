using RoLaMoDS.Extention;
using Xunit;
using System.IO;
using System.Drawing;
using System;

namespace RoLaMoDS.Tests.Extentions
{
    public class StreamExtentionTest
    {
        [Fact]
        public void TestImageStream()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "nif.txt");
            File.WriteAllText(path, "not image file");
            Stream stream = new FileStream(path, FileMode.Open);
            Assert.False(stream.TryConvertToImage(out Image img));
            stream.Close();
            var path1 = Path.Combine(Directory.GetCurrentDirectory(), "if.txt");
            new Bitmap(1, 1).Save(path1);
            Stream stream1 = new FileStream(path1, FileMode.Open);
            Assert.True(stream1.TryConvertToImage(out Image img1));
            stream1.Close();
            File.Delete(path);
            File.Delete(path1);
        }
    }
}