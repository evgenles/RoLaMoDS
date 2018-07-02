using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using RoLaMoDS.Data;
using RoLaMoDS.Models.Enums;
using RoLaMoDS.Models.ViewModels;
using RoLaMoDS.Services.Interfaces;

namespace RoLaMoDS.Services
{
    public class RecognizeService : IRecognizeService
    {
        private readonly ApplicationDBContext _applicationDbContext;
        private readonly IFileService _fileService;
        private readonly IImageWorkerService _imageWorkerService;
        private readonly IHostingEnvironment _env;

        public RecognizeService(ApplicationDBContext applicationDBContext,
                                IFileService fileService,
                                IImageWorkerService imageWorkerService,
                                IHostingEnvironment env)
        {
            _imageWorkerService = imageWorkerService;
            _applicationDbContext = applicationDBContext;
            _fileService = fileService;
            _env = env;
        }

        /// <summary>
        /// Save uploaded file on server
        /// </summary>
        /// <param name="file">File to save</param>
        /// <returns></returns>
        private async Task<string> SaveUploadFile(IFormFile file)
        {
            var filepath = _fileService.GetNextFilesPath(1, DirectoryType.Upload)[0];
            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filepath;
        }

        /// <summary>
        /// [NOT CONNECTED TO RECOGNIZE MODULE] Save pictures (or archive of pictures) on server, send pictures to train of recognize module
        /// </summary>
        /// <param name="model">Model to train</param>
        /// <returns>F1 Score of recognize valid data</returns>
        public async Task<double?> TrainModel(TrainViewModel model)
        {
            var modelNN = await _applicationDbContext.ModelsNN.SingleOrDefaultAsync(m => m.URL == model.ModelURL);
            if (modelNN != null)
            {
                List<string> pathes = new List<string>(model.Files.Count);
                byte[] buffer = new byte[4096];
                //Save files
                foreach (var file in model.Files)
                {
                    if (file.ContentType.IndexOf("zip") > -1)
                    {
                        using (ZipInputStream s = new ZipInputStream(file.OpenReadStream()))
                        {

                            ZipEntry theEntry;
                            while ((theEntry = s.GetNextEntry()) != null)
                            {
                                var fpath = Path.GetFileName(theEntry.Name).ToLower();
                                if (fpath.IndexOf(".img") > -1 ||
                                    fpath.IndexOf(".png") > -1 ||
                                    fpath.IndexOf(".gif") > -1)
                                {
                                    var filepath = _fileService.GetNextFilesPath(1, DirectoryType.Upload)[0];
                                    using (FileStream streamWriter = File.Create(filepath))
                                    {
                                        StreamUtils.Copy(s, streamWriter, buffer);
                                        pathes.Add(filepath);
                                    }
                                }
                            }
                            s.Close();
                        }
                    }
                    if (file.ContentType.IndexOf("image") > -1)
                    {
                        pathes.Add(await SaveUploadFile(file));
                    }
                }
                //Create file map: path_to_file \t number_class
                if (pathes.Count != 0)
                {
                    var mapFilePath = _fileService.GetNextFilesPath(1, DirectoryType.Model)[0] + ".map";
                    using (var mapStream = new StreamWriter(mapFilePath))
                    {
                        foreach (var img in pathes)
                        {
                            await mapStream.WriteLineAsync($"{img}\t{model.Class}");
                        }
                        mapStream.Close();
                    }
                }
            }
            //TODO: Send .map file path to Recognize module
            return 0;
        }

        /// <summary>
        /// [NOT CONNECTED TO RECOGNIZE MODULE] Split image on cells and send it to recognize
        /// </summary>
        /// <param name="model">Images to recognize</param>
        /// <returns>Object`s in cells</returns>
        public async Task<RecognizeResultViewModel[,]> StartRecognize(RecognizeViewModel model)
        {
            var imgUrl = model.SatelliteURL
                                .Remove(0, model.SatelliteURL.LastIndexOf("/images/"))
                                .Replace("/", "\\");
            var img = await _applicationDbContext.Images
                                .SingleOrDefaultAsync(a => a.URL == imgUrl);
            if (img != null)
            {
                var count = _imageWorkerService
                        .UseImage(Image.FromFile(_env.WebRootPath + imgUrl), img.Scale);
                var cellsPath = _fileService
                        .GetNextFilesPath(_imageWorkerService.Count(), DirectoryType.Recognize);
                int i = 0;
                var retMatrix = new RecognizeResultViewModel[count, count];
                img.Cells = new List<Models.CellDB>();
                foreach (var cell in _imageWorkerService)
                {

                    cell.CellImage.Save(cellsPath[i]);

                    //Send to recognize and mark image
                    img.Cells.Add(new Models.CellDB
                    {
                        URL = cellsPath[i] // Add aditional recognized data
                    });
                    //Replace to recognized
                    retMatrix[cell.X, cell.Y] = new RecognizeResultViewModel
                    {
                        Class = "Not Recognized",
                        Height = 0,
                        CellURL = cellsPath[i].Remove(0, cellsPath[i].LastIndexOf("\\images\\"))
                    };
                    i++;
                }
                await _applicationDbContext.SaveChangesAsync();
                return retMatrix;

            }
            return null;
        }

        /// <summary>
        /// Get all public models and models for selected user
        /// </summary>
        /// <param name="userId">Userid of current user</param>
        /// <returns>List of all models</returns>
        public Task<List<GetModelViewModel>> GetUserModels(Guid userId)
        {
            return _applicationDbContext.ModelsNN
                            .Where(model => model.IsPublished || model.UserId == userId)
                            .Select(model => new GetModelViewModel { URL = model.URL, Name = model.Name })
                            .ToListAsync();
        }

        /// <summary>
        /// Get all available class in model
        /// </summary>
        /// <param name="modelURL">URL model</param>
        /// <returns>Class list</returns>
        public async Task<List<GetModelClassesViewModel>> GetClassOfModel(string modelURL)
        {
            var model = await _applicationDbContext.ModelsNN.SingleAsync(m => m.URL == modelURL);
            return model == null ? null :
                await _applicationDbContext
                    .ModelsNNClasses
                    .Where(cl => cl.Model == model)
                    .Select(cl => new GetModelClassesViewModel { Name = cl.Name, Id = cl.NumberClass })
                    .ToListAsync();
        }
    }
}