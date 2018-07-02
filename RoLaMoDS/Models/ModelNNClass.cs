using System;

namespace RoLaMoDS.Models
{
    /// <summary>
    /// neural network model`s class for DB
    /// </summary>
    public class ModelNNClass
    {
        public Guid? Id { get; set; }
        public int NumberClass { get; set; }
        public string Name { get; set; }
        public Guid? ModelId {get;set;}
        public ModelsNNDB Model { get; set; }
    }
}