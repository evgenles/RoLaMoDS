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
using RoLaMoDS.Models;
namespace RoLaMoDS.Controllers
{
    public class MainController : ApiController
    {
        private readonly IMainControllerService _mainControllerService;

        public MainController(IMainControllerService mainControllerService)
        {
            _mainControllerService = mainControllerService;
        }

        private string GetErrorsKeys()
        {
            string errors = "";
            foreach (var ms in ModelState)
            {
                if (ms.Value.Errors.Count != 0)
                {
                    errors += ms.Key + ";";
                }
            }
            return errors;
        }
        public IActionResult Index()
        {
            return View();

        }


        [HttpPost]
        public IActionResult UploadImageFromFile(UploadImageFileModel model)
        {
            if (ModelState.IsValid)
            {
                var rez = _mainControllerService.UploadImageFromFile(model);
                return JSON(rez.Item1, rez.Item2, rez.Item3);
            }
            else
            {
                return JSON("", 400, GetErrorsKeys());
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageFromURL([FromBody] UploadImageURLModel model)
        {
            if (ModelState.IsValid)
            {
                var rez = await _mainControllerService.UploadImageFromURL(model);
                return JSON(rez.Item1, rez.Item2, rez.Item3);
            }
            else
            {
                return JSON("", 400, GetErrorsKeys());
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageFromMap([FromBody] UploadImageMapModel model)
        {
            if (ModelState.IsValid)
            {
                var rez = await _mainControllerService.UploadImageFromMaps (model);
                 return JSON(rez.Item1, rez.Item2, rez.Item3);
            }
            else
            {
                return JSON("", 400, GetErrorsKeys());
            }
        }
    }
}