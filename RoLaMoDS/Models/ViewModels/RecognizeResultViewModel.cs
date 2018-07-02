namespace RoLaMoDS.Models.ViewModels
{
    /// <summary>
    /// View model for rezult of recognize action
    /// </summary>
    public class RecognizeResultViewModel
    {
        public string Class { get; set; }
        public int Height { get; set; }
        public string CellURL { get; set; }
    }
}