using System.ComponentModel.DataAnnotations;

namespace RoLaMoDS.Models
{
    public class UploadImageURLModel:UploadImageModel
    {
        [Required]
        [Url]
        public string URL { get; set; }
    }
}