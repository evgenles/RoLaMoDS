using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RoLaMoDS.Models.ViewModels
{
    /// <summary>
    /// View model for train action [MUST RECEIVE FORM DATA]
    /// </summary>
    public class TrainViewModel
    {
        public List<IFormFile> Files { get; set; }
        public string Class { get; set; }
        public string ModelURL { get; set; }
    }
}