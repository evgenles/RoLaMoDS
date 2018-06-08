using System.ComponentModel.DataAnnotations;
namespace RoLaMoDS.Models.ViewModels
{
    public class UploadImageModel
    {
        [Range(5, 50)]
        public int Scale { get; set; }

        [Range(-90.0, 90.0)]
        public double Latitude { get; set; }

        [Range(-180.0, 180.0)]
        public double Longitude { get; set; }

        public bool IsPreview { get; set; } = false;
    }
}