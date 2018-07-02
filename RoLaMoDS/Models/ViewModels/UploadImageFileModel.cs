using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RoLaMoDS.Models.ViewModels
{
    /// <summary>
    /// Model for uploading files
    /// </summary>
    public class UploadImageFileModel:UploadImageModel
    {
        [Required]
        public IFormFile File { get; set; }
    }
}