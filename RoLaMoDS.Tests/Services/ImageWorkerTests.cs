using System;
using System.Drawing;
using System.IO;
using RoLaMoDS.Services;
using RoLaMoDS.Services.Interfaces;
using System.Linq;
using Xunit;
using System.Collections.Generic;
using RoLaMoDS.Models;

namespace RoLaMoDS.Tests.Services {
    public class ImageWorkerTests {
        private readonly IImageWorkerService _IImageWorker;
        public ImageWorkerTests () {
            _IImageWorker = new ImageWorkerService (new ImageValidator ());
        }

        [Fact]
        public void UseImageTest1 () {
            Bitmap bmp = new Bitmap (200, 200);
            var cCell = _IImageWorker.UseImage(bmp,10);
            Assert.Equal(4, cCell);
            Assert.Equal(cCell*cCell, _IImageWorker.Count());
        }
        [Fact]
        public void UseImageTest2 () {
            Bitmap bmp = new Bitmap (200, 200);
            bmp.SetPixel(120,10, Color.Red);
            bmp.SetPixel(130,120, Color.Red);
            bmp.SetPixel(10,60, Color.Red);
            bmp.SetPixel(115,115, Color.Red);
            Bitmap bmpTR = new Bitmap(100,100);
            using (Graphics gr = Graphics.FromImage(bmpTR)){
                gr.DrawImage(bmp,0,0, new Rectangle(100,0,100,100),GraphicsUnit.Pixel);
            }
            var cCell = _IImageWorker.UseImage(bmp,5);
            Assert.Equal(2, cCell);
            var rezTR = _IImageWorker.Single(cell=>cell.X==1&&cell.Y==0).CellImage;
            MemoryStream ms = new MemoryStream ();
            bmpTR.Save (ms, System.Drawing.Imaging.ImageFormat.Bmp);
            String firstBitmap = Convert.ToBase64String (ms.ToArray ());
            ms.Position = 0;
            rezTR.Save (ms, System.Drawing.Imaging.ImageFormat.Bmp);
            String secondBitmap = Convert.ToBase64String (ms.ToArray ());
            Assert.Equal(firstBitmap, secondBitmap);
        }
        [Fact]
        public void MakeBorderTest1 () {
            Bitmap bmp = new Bitmap (100, 100);
            Models.Cell resultCell = _IImageWorker.MakeBorderOnCell (new Models.Cell {
                CellImage = bmp, X = 0, Y = 1
            });
            Assert.True (resultCell.X == 0 && resultCell.Y == 1);
            Bitmap result = ((Bitmap) resultCell.CellImage);
            Assert.True (result.Width == 100 && result.Height == 100);

            Assert.True (result.GetPixel (10, 0).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (0, 10).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (99, 10).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (10, 99).ToArgb ().Equals (Color.Black.ToArgb ()));
        }
        [Fact]
        public void MakeBorderTest2 () {
            Bitmap bmp = new Bitmap (100, 100);
            bmp.SetPixel(10,10, Color.Red);
            Models.Cell resultCell = _IImageWorker.MakeBorderOnCell (new Models.Cell {
                CellImage = bmp, X = 0, Y = 1
            });
            Assert.True (resultCell.X == 0 && resultCell.Y == 1);
            Bitmap result = ((Bitmap) resultCell.CellImage);
            Assert.True (result.Width == 100 && result.Height == 100);
            Assert.True (result.GetPixel (10, 0).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (0, 10).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (99, 10).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (10, 99).ToArgb ().Equals (Color.Black.ToArgb ()));
            Assert.True (result.GetPixel (10, 10).ToArgb ().Equals (Color.Red.ToArgb ()));
        }

        [Fact]
        public void FormResultImageTest1 () {
            var rez = (Bitmap) _IImageWorker.FormResultImage (new List<Cell>());
            Assert.Null(rez);
        }
         
        [Fact]
        public void FormResultImageTest2 () {
            var rez = (Bitmap) _IImageWorker.FormResultImage (
                new List<Cell>{ 
                    new Cell{X=0,Y=0,CellImage = new Bitmap(100,100)},
                    new Cell{X=1,Y=0,CellImage = new Bitmap(100,100)},
                    new Cell{X=0,Y=1,CellImage = new Bitmap(100,100)},
                    new Cell{X=1,Y=1,CellImage = new Bitmap(100,100)},
                });
            Assert.Equal(200,rez.Height);
            Assert.Equal(200,rez.Width);
        }
        [Fact]
        public void FormResultImageTest3 () {
            Bitmap bmp = new Bitmap (200, 200);
            using (Graphics gr = Graphics.FromImage (bmp)) {
                gr.DrawRectangle (new Pen (Color.Blue), 10, 10, 100, 100);
            }
            _IImageWorker.UseImage (bmp, 5);
            Bitmap resultImg = (Bitmap) _IImageWorker.FormResultImage (_IImageWorker);
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