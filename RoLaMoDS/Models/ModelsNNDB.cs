using System;
using System.Collections.Generic;

namespace RoLaMoDS.Models
{
    /// <summary>
    /// Model of neural network for DB
    /// </summary>
    public class ModelsNNDB
    {
        public Guid? Id {get;set;}
        public string URL {get;set;}
        public string Name {get;set;}

        /// <summary>
        /// If published all user can access to this model
        /// </summary>
        public bool IsPublished { get; set; } = false;
        public Guid? UserId {get;set;}
        public UserModel User {get;set;} = null; 

        public List<ImageDBModel> Images { get; set; }
        public List <ModelNNClass> ModelNNClass { get; set; }
    }
}