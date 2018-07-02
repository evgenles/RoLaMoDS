using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using RoLaMoDS.Data;
using RoLaMoDS.Extention;
using RoLaMoDS.Models;
using RoLaMoDS.Models.Enums;
using RoLaMoDS.Models.ViewModels;
using RoLaMoDS.Services.Interfaces;
namespace RoLaMoDS.Services
{
    public class MainControllerSevice : IMainControllerService
    {
        private readonly IImageWorkerService _imageWorkerService;
        private readonly IFileService _fileService;

        private readonly ApplicationDBContext _applicationDBContext;

        public MainControllerSevice(IImageWorkerService imageWorkerService, IFileService fileService, ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
            _fileService = fileService;
            _imageWorkerService = imageWorkerService;
        }

        public void StartRecognize()
        {
           // throw new System.NotImplementedException();
        }

        /// <summary>
        /// Upload image from file with longitude, lantitude and scale
        /// </summary>
        /// <param name="model">Model to upload</param>
        /// <returns>Task of (result, state, message)</returns>
        public async Task<(object, int, string)> UploadImageFromFile(UploadImageFileModel model, Guid UserId)
        {
            var filePath = _fileService.GetNextFilesPath(1, DirectoryType.Upload)[0];

            if (model.File.OpenReadStream().TryConvertToImage(out Image img))
            {
                img.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);
                string retUrl = filePath.Remove(0, filePath.LastIndexOf("\\images\\"));
                ImageDBModel image = new ImageDBModel
                {
                    Cells = null,
                    Expires = DateTime.Now + TimeSpan.FromDays(1),
                    IsPreview = model.IsPreview,
                    Longitude = -1,
                    Latitude = -1,
                    Scale = -1,
                    URL = retUrl
                };
                if (UserId == Guid.Empty)
                    _applicationDBContext.Images.Add(image);
                else
                    _applicationDBContext.Users.Find(UserId).DownloadedImages.Add(image);

                await _applicationDBContext.SaveChangesAsync();
                return (new
                {
                    resultImagePath = retUrl
                }, 200, "");
            }
            return ("", 400, "File_Not_Image");
        }

        /// <summary>
        /// Upload image from URL with longitude, lantitude and scale
        /// </summary>
        /// <param name="model">Model to upload</param>
        /// <returns>(result, state, message)</returns>
        public async Task<(object, int, string)> UploadImageFromURL(UploadImageURLModel model, Guid UserId)
        {
            if (!model.IsPreview)
            {
                ImageDBModel image = _applicationDBContext.Images.SingleOrDefault(img => img.URL == model.URL);
                if (image != null)
                {
                    image.Expires = DateTime.Now+TimeSpan.FromDays(30);
                    image.Latitude = model.Latitude;
                    image.IsPreview = false;
                    image.Longitude = model.Longitude;
                    image.Scale = model.Scale;
                    await _applicationDBContext.SaveChangesAsync();

                    return (new
                    {
                        resultImagePath = model.URL
                    }, 200, "");
                }
                else
                {
                    //TODO: errors
                }
            }
            else
            {
                var filePath = _fileService.GetNextFilesPath(1, DirectoryType.Upload)[0];
                using (var client = new HttpClient())
                {
                    try
                    {
                        using (var result = await client.GetAsync(model.URL))
                        {
                            if (result.IsSuccessStatusCode)
                            {
                                if ((await result.Content.ReadAsStreamAsync()).TryConvertToImage(out Image img))
                                {
                                    img.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);

                                    string retUrl = filePath.Remove(0, filePath.LastIndexOf("\\images\\"));
                                    ImageDBModel image = new ImageDBModel
                                    {
                                        Cells = null,
                                        Expires = DateTime.Now + TimeSpan.FromDays(1),
                                        IsPreview = model.IsPreview,
                                        Longitude = -1,
                                        Latitude = -1,
                                        Scale = -1,
                                        URL = retUrl
                                    };
                                    if (UserId == Guid.Empty)
                                        _applicationDBContext.Images.Add(image);
                                    else
                                        _applicationDBContext.Users.Find(UserId).DownloadedImages.Add(image);

                                    await _applicationDBContext.SaveChangesAsync();
                                    return (new
                                    {
                                        resultImagePath = retUrl
                                    }, 200, "");
                                }
                                return ("", 400, "File_Not_Image");
                            }
                        }
                    }
                    catch (HttpRequestException)
                    {
                        return ("", 404, "Host_Not_found");
                    }
                }
            }
            return ("", 404, "Unavailable_URL");
        }

        /// <summary>
        /// Upload image from Google maps or Bing Maps with longitude, lantitude and scale
        /// </summary>
        /// <param name="model">Model to upload</param>
        /// <returns>(result, state, message)</returns>
        public async Task<(object, int, string)> UploadImageFromMaps(UploadImageMapModel model, Guid UserId)
        {
            var filePath = _fileService.GetNextFilesPath(1, DirectoryType.Upload)[0];
            var key = "";
            var url = "";
            switch (model.MapType)
            {
                case MapTypes.Bing:
                    key = "AsxCWNx09JBu4SthLwqimpbExMR30Ho7iVzGaxCp6TsMzlDn9G7f3O6tZS40io7K";
                    url = $@"https://dev.virtualearth.net/REST/v1/Imagery/Map/Aerial/{model.Latitude}," +
                        $@"{model.Longitude}/{model.Zoom}?mapSize={model.MapSizeX ?? 2000},{model.MapSizeY ?? 1500}&key={key}";
                    break;
                case MapTypes.Google:
                    key = "AIzaSyD1Gub16QiBvQ4olb_0HvFidiqQoPKsrkk";
                    url = $@"https://maps.googleapis.com/maps/api/staticmap?center={model.Latitude}," +
                        $@"{model.Longitude}&zoom={model.Zoom}&size={model.MapSizeX ?? 640}x{model.MapSizeY ?? 640}&key={key}" +
                        "&maptype=satellite&scale=1&format=bmp";
                    break;
            }
            using (var client = new HttpClient())
            {
                try
                {
                    using (var result = await client.GetAsync(url))
                    {
                        if (result.IsSuccessStatusCode)
                        {
                            //TODO: Handle error

                            if ((await result.Content.ReadAsStreamAsync()).TryConvertToImage(out Image img))
                            {
                                Bitmap bmp = (Bitmap)img;

                                if (model.MapType == MapTypes.Bing && (bmp.GetPixel(0, bmp.Height - 1).Name == "fff5f2ed" ||
                                     bmp.GetPixel(bmp.Width - 1, bmp.Height - 1).Name == "fff5f2ed") ||

                                     (model.MapType == MapTypes.Google && (bmp.GetPixel(0, bmp.Height - 1).Name == "ffe4e2de" ||
                                     bmp.GetPixel(bmp.Width - 1, bmp.Height - 1).Name == "ffe4e2de"))
                                )
                                {
                                    return ("", 400, "Image_So_Zommed");
                                }
                                img.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);
                                string retUrl = filePath.Remove(0, filePath.LastIndexOf("\\images\\"));
                                ImageDBModel image = new ImageDBModel
                                {
                                    Cells = null,
                                    Expires = model.IsPreview ? DateTime.Now + TimeSpan.FromDays(1) :
                                        DateTime.Now + TimeSpan.FromDays(30),
                                    IsPreview = model.IsPreview,
                                    Longitude = model.Longitude,
                                    Latitude = model.Latitude,
                                    Scale = -1,
                                    URL = retUrl
                                };
                                if (UserId == Guid.Empty)
                                    _applicationDBContext.Images.Add(image);
                                else
                                    _applicationDBContext.Users.Find(UserId).DownloadedImages.Add(image);

                                await _applicationDBContext.SaveChangesAsync();
                                return (new
                                {
                                    resultImagePath = retUrl
                                }, 200, "");
                            }
                            return ("", 400, "File_Not_Image");
                        }
                    }
                }
                catch (HttpRequestException)
                {
                    return ("", 404, "Host_Not_found");
                }
            }
            return ("", 400, "Unavailable_URL");
        }


        /// <summary>
        /// Split image into cells
        /// </summary>
        /// <param name="path">Path of image</param>
        /// <param name="scale">Scale of image</param>
        /// <returns>Matrix of path to cell images</returns>
        public string[,] SplitImage(string path, int scale)
        {

            int count = _imageWorkerService.UseImage(Image.FromFile(path), scale);
            var imageSplitPatches = new string[count, count];
            var filePathes = _fileService.GetNextFilesPath(count * count, DirectoryType.Recognize);
            int i = 0;
            foreach (var cell in _imageWorkerService)
            {
                //nowPath = Path.Combine("images", $"{cell.X}_{cell.Y}.bmp");
                //TODO: DB
                imageSplitPatches[cell.Y, cell.X] = filePathes[i].Remove(0, filePathes[i].LastIndexOf("\\images\\"));
                using (var streamSave = new FileStream(filePathes[i], FileMode.Create))
                {
                    cell.CellImage.Save(streamSave, System.Drawing.Imaging.ImageFormat.Bmp);
                }
                i++;
            }
            return imageSplitPatches;

        }

        /// <summary>
        /// Form result image from cells
        /// </summary>
        /// <param name="pathes">Matrix of path to cell images</param>
        /// <returns>Path of result image</returns>
        public string FormResult(string[,] pathes)
        {
            string resultPath = _fileService.GetNextFilesPath(1, DirectoryType.Result)[0];
            List<Cell> cells = new List<Cell>(pathes.Length);
            for (int i = 0; i < pathes.GetLength(0); i++)
            {
                for (int j = 0; j < pathes.GetLength(1); j++)
                {
                    cells.Add(new Cell
                    {
                        CellImage = Image.FromFile(pathes[i, j]),
                        X = i,
                        Y = j
                    });
                }
            }
            using (var streamSave = new FileStream(resultPath, FileMode.Create))
            {
                _imageWorkerService.FormResultImage(cells).Save(streamSave, System.Drawing.Imaging.ImageFormat.Bmp);
            }

            return resultPath.Remove(0, resultPath.LastIndexOf("\\images\\"));
        }
    }
}