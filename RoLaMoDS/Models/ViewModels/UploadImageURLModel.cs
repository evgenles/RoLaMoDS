using System.ComponentModel.DataAnnotations;
using RoLaMoDS.Attributes;

namespace RoLaMoDS.Models.ViewModels
{
    /// <summary>
    /// Model for downloading by URL
    /// </summary>
    public class UploadImageURLModel:UploadImageModel
    {
        [Required]
        public string URL { get; set; }
    }
}