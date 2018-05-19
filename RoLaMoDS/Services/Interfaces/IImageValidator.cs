using System.Drawing;
using System.IO;

namespace RoLaMoDS.Services.Interfaces {
    public interface IImageValidator {
              bool IsImageValidSize (Image img);
    }
}