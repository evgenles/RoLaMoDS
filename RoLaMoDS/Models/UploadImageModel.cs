using System.ComponentModel.DataAnnotations;
namespace RoLaMoDS.Models
{
    public class UploadImageModel
    {
        [Required]
        [Range(5,50)]
        public int Scale { get; set; } 
        
        [Required]
        [Range(-90.0,90.0)]
        public double Latitude { get; set; }

        [Required]
        [Range(-180.0,180.0)]
        public double Longitude { get; set; }
    }
}