using System.Drawing;
using System.IO;

namespace RoLaMoDS.Services.Interfaces
{
    public interface IImageValidator
    {
        /// <summary>
        /// Validate image size
        /// </summary>
        /// <param name="img">Image to validate</param>
        /// <returns>True if valid image</returns>
        bool IsImageValidSize(Image img);

    }
}