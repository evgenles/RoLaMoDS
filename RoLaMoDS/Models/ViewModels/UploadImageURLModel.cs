using System.ComponentModel.DataAnnotations;
using RoLaMoDS.Attributes;

namespace RoLaMoDS.Models.ViewModels
{
    public class UploadImageURLModel:UploadImageModel
    {
        [Required]
        public string URL { get; set; }
    }
}