using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
namespace RoLaMoDS.Models
{
    public class Cell
    {
        [NotMapped]
        public Image CellImage { get; set; }
        public string URL {get;set;}
        public int X { get; set; }
        public int Y { get; set; }

        public int? HeightObject {get;set;}
        public string RecognizedObject {get;set;}
    }
}