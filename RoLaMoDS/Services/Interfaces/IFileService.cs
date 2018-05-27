namespace RoLaMoDS.Services.Interfaces
{
    public interface IFileService
    {
         
         /// <summary>
         /// Get name next file
         /// </summary>
         /// <param name="CountFile">Counter of uploading files (for reservation)</param>
         /// <param name="DType">Type of directory</param>
         /// <returns>Names of files</returns>
         string[] GetNextFilesPath(int CountFile, DirectoryType DType);

        /// <summary>
        /// Create directories /wwwroot/images, /wwwroot/images/uploads,
        /// /wwwroot/images/results, /wwwroot/images/recognize
        /// </summary>
         void CreateImagePathes();
    }
}