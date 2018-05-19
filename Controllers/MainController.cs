using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoLaMoDS.Services.Interfaces;

namespace RoLaMoDS.Controllers {
    public class MainController : Controller {
        private readonly IImageWorkerService _imageWorkerService;
        private readonly IHostingEnvironment _env;
        public MainController (IImageWorkerService imageWorkerService, IHostingEnvironment env) {
            _imageWorkerService = imageWorkerService;
            _env = env;
        }
        public IActionResult Index () {
            return View ();
        }

        [HttpPost]
        public async Task<IActionResult> UploadMap (IFormFile file, string scale) {

            var filePath = Path.GetTempFileName ();
            if (Int32.TryParse (scale, out int parsedScale)&&parsedScale <=50 &&parsedScale >=5) {
                string[, ] imageSplitPatches = null;
                using (var stream = new FileStream (filePath, FileMode.Create)) {
                    await file.CopyToAsync (stream);
                    int count = _imageWorkerService.UseImage (Image.FromStream (file.OpenReadStream ()), parsedScale);
                    imageSplitPatches = new string[count, count];
                    var nowPath = "";
                    foreach (var cell in _imageWorkerService) {
                        nowPath = Path.Combine ("images", $"{cell.X}_{cell.Y}.bmp");
                        imageSplitPatches[cell.Y, cell.X] = "\\" + nowPath;
                        using (var streamSave = new FileStream (Path.Combine (_env.WebRootPath, nowPath), FileMode.Create)) {
                            _imageWorkerService.MakeBorderOnCell (cell).CellImage.Save (streamSave, System.Drawing.Imaging.ImageFormat.Bmp);
                        }
                    }
                }
                string resultPath = Path.Combine ("images", "result.bmp");
                using (var streamSave = new FileStream (Path.Combine (_env.WebRootPath, resultPath), FileMode.Create)) {
                    _imageWorkerService.FormResultImage ().Save (streamSave, System.Drawing.Imaging.ImageFormat.Bmp);
                }

                // process uploaded files
                // Don't rely on or trust the FileName property without validation.

                return View ("Index", (imageSplitPatches, "\\" + resultPath));
            }
            else{
                //TODO: Scale error
                return View("Index");
            }
        }
    }
}