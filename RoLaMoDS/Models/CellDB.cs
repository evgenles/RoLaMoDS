using System;

namespace RoLaMoDS.Models
{
    /// <summary>
    /// Cell for database
    /// </summary>
    public class CellDB : Cell
    {
        public Guid? Id { get; set; }
        public Guid? ImageId { get; set; }
        public ImageDBModel Image { get; set; }
    }
}