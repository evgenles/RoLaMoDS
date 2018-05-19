using System;
using System.Drawing;
using System.IO;
using RoLaMoDS.Services;
using RoLaMoDS.Services.Interfaces;
using Xunit;

namespace RoLaMoDS.Tests.Services {
    public class ImageWorkerTests {
        private readonly IImageWorkerService _IImageWorker;
        public ImageWorkerTests () {
            _IImageWorker = new ImageWorkerService (new ImageValidator ());
        }

        [Fact]
        public void UseImageScaleInRange () {
            Bitmap bmp = new Bitmap (500, 500);
            Assert.True (_IImageWorker.UseImage (bmp, 5) == 1);
            Assert.True (_IImageWorker.UseImage (bmp, 10) == 2);
            Assert.True (_IImageWorker.UseImage (bmp, 47) == 9);
            Assert.True (_IImageWorker.UseImage (bmp, 48) == 10);
            Assert.True (_IImageWorker.UseImage (bmp, 50) == 10);
        }

        [Fact]
        public void UseImageScaleOutOfRange () {
            Bitmap bmp = new Bitmap (100, 100);
            Assert.True (_IImageWorker.UseImage (bmp, 55) == -1);
            Assert.True (_IImageWorker.UseImage (bmp, 2) == -1);
        }

        [Fact]
        public void MakeBorderTest () {
            Bitmap bmp = new Bitmap (100, 100);
            Models.Cell resultCell = _IImageWorker.MakeBorderOnCell (new Models.Cell {
                CellImage = bmp, X = 0, Y = 1
            });
            Assert.True (resultCell.X == 0 && resultCell.Y == 1);
            Bitmap result = ((Bitmap) resultCell.CellImage);
            Assert.True (result.GetPixel (0, 0).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (99, 0).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (99, 99).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (0, 99).ToArgb ().Equals (Color.Black.ToArgb ()));

            Assert.True (result.GetPixel (10, 0).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (0, 10).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (99, 10).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (10, 99).ToArgb ().Equals (Color.Black.ToArgb ()));
        }

        [Fact]
        public void FormResultImageTest () {
            Bitmap bmp = new Bitmap (500, 500);
            using (Graphics gr = Graphics.FromImage (bmp)) {
                gr.DrawRectangle (new Pen (Color.Blue), 10, 10, 100, 100);
            }
            _IImageWorker.UseImage (bmp, 10);
            Bitmap resultImg = (Bitmap) _IImageWorker.FormResultImage ();
            MemoryStream ms = new MemoryStream ();
            bmp.Save (ms, System.Drawing.Imaging.ImageFormat.Png);
            String firstBitmap = Convert.ToBase64String (ms.ToArray ());
            ms.Position = 0;
            resultImg.Save (ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String (ms.ToArray ());
            Assert.Equal(firstBitmap, secondBitmap);
        }
    }
}