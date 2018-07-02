using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using RoLaMoDS.Models;
using RoLaMoDS.Services.Interfaces;
using RoLaMoDS.Extention;
namespace RoLaMoDS.Services
{
    public class ImageWorkerService : IImageWorkerService
    {

        private readonly IImageValidator _imageValidator;

        public ImageWorkerService(IImageValidator imageValidator)
        {
            _imageValidator = imageValidator;

        }
        private IEnumerable<Cell> imageCells;
        private int cCell = -1;
        /// <summary>
        /// Divide an image into cells with scale. 
        /// </summary>
        /// <param name="image">Image to divide</param>
        /// <param name="scale">Scale of image. Minimum scale 5:1, maximum scale 1:50</param>
        /// <returns>List of cells</returns>
        private IEnumerable<Cell> DivideImageIntoCells(Image image, int scale)
        {
            List<Cell> arr = new List<Cell>(100);
            cCell = (int)System.Math.Round((double)scale / 2.5);
            int cellWidth = image.Width / cCell;
            int cellHeight = image.Height / cCell;
            for (int i = 0; i < cCell; i++)
            {
                for (int j = 0; j < cCell; j++)
                {
                    Bitmap cellBmp = new Bitmap(cellWidth, cellHeight);
                    using (Graphics g = Graphics.FromImage(cellBmp))
                    {
                        g.DrawImage(image, 0, 0, new Rectangle(cellWidth * i, cellHeight * j, cellWidth, cellHeight), GraphicsUnit.Pixel);
                    }
                    arr.Add(new Cell { CellImage = cellBmp, X = i, Y = j });

                }
            }
            return arr;
        }


        /// <summary>
        /// Create an image from cells (uploaded by UseImage)
        /// </summary>
        /// <returns>Result image</returns>
        public Image FormResultImage()
        {
            Image firstImage = ((List<Cell>)imageCells)[0].CellImage;
            Image bmp = new Bitmap(firstImage.Width * cCell, firstImage.Height * cCell);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                foreach (var cell in imageCells)
                {
                    gr.DrawImage(cell.CellImage, cell.X * firstImage.Width, cell.Y * firstImage.Height);
                }
            }
            return bmp;
        }

        /// <summary>
        /// Create an image from cells 
        /// </summary>
        /// <param name="cells">Cells of image</param>
        /// <returns>Result image</returns>
        public Image FormResultImage(IEnumerable<Cell> cells)
        {
            var numerator = (cells).GetEnumerator();
            if (numerator.MoveNext())
            {
                Image firstImage = numerator.Current.CellImage;
                int countCell = (int)System.Math.Sqrt(cells.Count());
                Image bmp = new Bitmap(firstImage.Width * countCell, firstImage.Height * countCell);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    foreach (var cell in cells)
                    {
                        gr.DrawImage(cell.CellImage, cell.X * firstImage.Width, cell.Y * firstImage.Height);
                    }
                }
                return bmp;
            }
            return null;
        }

        public IEnumerator<Cell> GetEnumerator() =>
            imageCells.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            imageCells.GetEnumerator();

        // <summary>
        /// Create border on cell
        /// </summary>
        /// <param name="input">Cell without border</param>
        /// <returns>Cell with border</returns>
        public Cell MakeBorderOnCell(Cell input)
        {
            using (Graphics gr = Graphics.FromImage(input.CellImage))
            {
                gr.DrawLines(new Pen(Color.Black), new Point[] {
                    new Point (0, 0),
                        new Point (input.CellImage.Width - 1, 0),
                        new Point (input.CellImage.Width - 1, input.CellImage.Height - 1),
                        new Point (0, input.CellImage.Height - 1),
                        new Point (0, 0)
                });
            }
            return input;
        }



        /// <summary>
        /// Load image into ImageWorkerService. Divide an image into cells with scale. 
        /// </summary>
        /// <param name="image">Image to load and divide</param>
        /// <param name="scale">Scale of image. Minimum scale 1:5, maximum scale 1:50</param>
        /// <returns>Count of cells in line</returns>
        public int UseImage(Image img, int scale)
        {
            imageCells = DivideImageIntoCells(img, scale);
            return cCell;
        }

        /// <summary>
        /// Load image into ImageWorkerService. Divide an image into cells with scale. 
        /// </summary>
        /// <param name="imgStream">File stream</param>
        /// <param name="scale">Scale of image. Minimum scale 1:5, maximum scale 1:50</param>
        /// <returns>Count of cells in line</returns>
        public int UseImage(Stream imgStream, int scale)
        {
            if (imgStream.TryConvertToImage(out Image img))
            {
                return UseImage(img, scale);
            }
            return -1;
        }
    }
}