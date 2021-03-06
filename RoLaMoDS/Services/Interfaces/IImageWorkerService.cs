using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using RoLaMoDS.Models;

namespace RoLaMoDS.Services.Interfaces
{
    public interface IImageWorkerService : IEnumerable<Cell>
    {
        /// <summary>
        /// Load image into ImageWorkerService. Divide an image into cells with scale. 
        /// </summary>
        /// <param name="image">Image to load and divide</param>
        /// <param name="scale">Scale of image. Minimum scale 1:5, maximum scale 1:50</param>
        /// <returns>Count of cells in line</returns>
        int UseImage(Image img, int scale);

        /// <summary>
        /// Load image into ImageWorkerService. Divide an image into cells with scale. 
        /// </summary>
        /// <param name="imgStream">File stream</param>
        /// <param name="scale">Scale of image. Minimum scale 1:5, maximum scale 1:50</param>
        /// <returns>Count of cells in line</returns>
        int UseImage(Stream imgStream, int scale);

        /// <summary>
        /// Create border on cell
        /// </summary>
        /// <param name="input">Cell without border</param>
        /// <returns>Cell with border</returns>
        Cell MakeBorderOnCell(Cell input);

        /// <summary>
        /// Create an image from cells (uploaded by UseImage)
        /// </summary>
        /// <returns>Result image</returns>
        Image FormResultImage();

        /// <summary>
        /// Create an image from cells 
        /// </summary>
        /// <param name="cells">Cells of image</param>
        /// <returns>Result image</returns>
        Image FormResultImage(IEnumerable<Cell> cells);
    }
}