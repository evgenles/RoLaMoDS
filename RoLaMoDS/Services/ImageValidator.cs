using System;
using System.Drawing;
using System.IO;
using RoLaMoDS.Services.Interfaces;

namespace RoLaMoDS.Services {
    public class ImageValidator : IImageValidator {
        public bool IsImageValidSize (Image img) {
            if (img.Height <= 5000 && img.Width <= 5000 && img.Width >= 100 && img.Height >= 100) {
                return true;
            }
            return false;
        }
    }
}