using System;
using System.Drawing;
using System.IO;

namespace RoLaMoDS.Extention
{
    public static class StreamExtention
    {
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