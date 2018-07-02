using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
namespace RoLaMoDS.Models
{
    /// <summary>
    /// Model for one image cell
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Image on cell
        /// </summary>
        [NotMapped]
        public Image CellImage { get; set; }

        /// <summary>
        /// Url of saved image
        /// </summary>
        public string URL {get;set;}

        /// <summary>
        /// Position X in default image
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Position Y in default image
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Height of recognized object
        /// </summary>
        public int? HeightObject {get;set;}

        /// <summary>
        /// Ground type on cell
        /// </summary>
        public string GroundType{ get; set; }

        /// <summary>
        /// Recognized object on cell
        /// </summary>
        /// <returns></returns>
        public string RecognizedObject {get;set;}
    }
}