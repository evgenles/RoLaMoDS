using System.ComponentModel.DataAnnotations;

namespace RoLaMoDS.Models.ViewModels
{
    public class UploadImageURLModel:UploadImageModel
    {
        [Required]
        [Url]
        public string URL { get; set; }
    }
}