using System;
using System.Drawing;
using System.IO;
using RoLaMoDS.Services.Interfaces;

namespace RoLaMoDS.Services
{
    public class ImageValidator : IImageValidator
    {
        /// <summary>
        /// Validate image size
        /// </summary>
        /// <param name="img">Image to validate</param>
        /// <returns>True if valid image</returns>
        public bool IsImageValidSize(Image img)
        {
            return (img.Height <= 5000 && img.Width <= 5000 && img.Width >= 100 && img.Height >= 100);
        }
    }
}