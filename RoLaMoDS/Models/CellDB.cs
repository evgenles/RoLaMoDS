using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoLaMoDS.Models
{
    public class CellDB : Cell
    {
        public Guid? Id { get; set; }
        public Guid? ImageId { get; set; }
        public ImageDBModel Image { get; set; }
    }
}