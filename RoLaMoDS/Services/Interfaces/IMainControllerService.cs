using System;
using System.Threading.Tasks;
using RoLaMoDS.Models;
using RoLaMoDS.Models.ViewModels;

namespace RoLaMoDS.Services.Interfaces
{
    public interface IMainControllerService
    {
        /// <summary>
        /// Upload image from file with longitude, lantitude and scale
        /// </summary>
        /// <param name="model">Model to upload</param>
        /// <returns>(result, state, message)</returns>
        Task<(object, int, string)> UploadImageFromFile(UploadImageFileModel model, Guid userId);

        /// <summary>
        /// Upload image from URL with longitude, lantitude and scale
        /// </summary>
        /// <param name="model">Model to upload</param>
        /// <returns>(result, state, message)</returns>
        Task<(object, int, string)> UploadImageFromURL(UploadImageURLModel model, Guid userId);

        /// <summary>
        /// Upload image from Google maps or Bing Maps with longitude, lantitude and scale
        /// </summary>
        /// <param name="model">Model to upload</param>
        /// <returns>(result, state, message)</returns>
        Task<(object, int, string)> UploadImageFromMaps(UploadImageMapModel model, Guid userId);

        /// <summary>
        /// Split image into cells
        /// </summary>
        /// <param name="path">Path of image</param>
        /// <param name="scale">Scale of image</param>
        /// <returns>Matrix of path to cell images</returns>
        string[,] SplitImage(string path, int scale);

        /// <summary>
        /// Form result image from cells
        /// </summary>
        /// <param name="pathes">Matrix of path to cell images</param>
        /// <returns>Path of result image</returns>
        string FormResult(string[,] pathes);
    }
}