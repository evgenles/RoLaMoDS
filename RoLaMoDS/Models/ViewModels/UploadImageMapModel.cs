using System.ComponentModel.DataAnnotations;
using RoLaMoDS.Models.Enums;

namespace RoLaMoDS.Models.ViewModels
{
    public class UploadImageMapModel:UploadImageModel
    {
        [Required]
        public MapTypes MapType { get; set; }

        /// <summary>
        /// Zoom level
        /// From 1 to 19 (UA) to 22 (USA) Google, From 1 to 19 (UA) to 23 USA Bing 
        /// </summary>
        /// <returns></returns>
        [Required]
        [Range(1,23)]
        public int Zoom {get;set;}

        public int? MapSizeX {get;set;}
        public int? MapSizeY{get;set;}
    }
}