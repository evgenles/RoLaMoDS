using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoLaMoDS.Models
{
    /// <summary>
    /// Image for database
    /// </summary>
    public class ImageDBModel:ViewModels.UploadImageURLModel
    {
        public Guid? Id{get;set;}
        
        [DataType(DataType.DateTime)]
        public DateTime Expires {get;set;}
        public double Humidity {get;set;}
        public double WindSpeed { get; set; }  
        public double Cloudy { get; set; }
        public double Teperature { get; set; }

        
        public Guid? UserId {get;set;}
        public UserModel User {get;set;}
        public Guid? ModelNNId {get;set;}
        public ModelsNNDB ModelNN { get; set; }
        public List<CellDB> Cells {get;set;}
    }
}