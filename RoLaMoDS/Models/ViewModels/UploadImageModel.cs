using System.ComponentModel.DataAnnotations;
using RoLaMoDS.Attributes;
namespace RoLaMoDS.Models.ViewModels
{
    public class UploadImageModel
    {
        [RangeWithSpecial(5, 50,0)]
        public int Scale { get; set; }

        [Range(-90.0, 90.0)]
        public double Latitude { get; set; }

        [Range(-180.0, 180.0)]
        public double Longitude { get; set; }

        public bool IsPreview { get; set; } = false;
    }
}