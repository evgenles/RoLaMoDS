using System;
using System.Drawing;
using System.IO;

namespace RoLaMoDS.Extention
{
    public static class StreamExtention
    {
        /// <summary>
        /// Convert stream to image
        /// </summary>
        /// <param name="stream">Stream to convert</param>
        /// <param name="img">Result image</param>
        /// <returns>True if success</returns>
        public static bool TryConvertToImage(this Stream stream, out Image img){
            try {
                 img = Image.FromStream (stream, false, false);
                 return true;
            }
            catch (ArgumentException){
                img = null;
                return false;
            }
        }
    }
}