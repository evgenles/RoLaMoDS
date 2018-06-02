using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RoLaMoDS.Models
{
    public class ImageDBModel:ViewModels.UploadImageURLModel
    {
        public Guid? Id{get;set;}
        
        [DataType(DataType.DateTime)]
        public DateTime Expires {get;set;}
        public Guid? UserId {get;set;}
        public UserModel User {get;set;}
        public List<CellDB> Cells {get;set;}

        
    }
}