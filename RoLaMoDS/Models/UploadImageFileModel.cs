using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RoLaMoDS.Models
{
    public class UploadImageFileModel:UploadImageModel
    {
        [Required]
        public IFormFile File { get; set; }
    }
}