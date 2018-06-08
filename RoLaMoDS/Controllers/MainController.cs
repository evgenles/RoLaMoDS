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
using RoLaMoDS.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace RoLaMoDS.Controllers
{
    public class MainController : ApiController
    {
        private readonly IMainControllerService _mainControllerService;
        public MainController(IMainControllerService mainControllerService,
                           UserManager<UserModel> userManager,
                           SignInManager<UserModel> signInManager):
                           base(userManager, signInManager)
        {
            _mainControllerService = mainControllerService;
        }


        public IActionResult Index()
        {
            return View();
        }
        private async Task<IActionResult> Upload<T>(T model) where T : UploadImageModel
        {
            if (ModelState.IsValid)
            {
                Guid userid = GetUserId();                
                (object, int, string) rez = ("", 200, "");
                if (model is UploadImageFileModel)
                {
                    rez = await _mainControllerService.UploadImageFromFile(model as UploadImageFileModel, userid);
                }
                else if (model is UploadImageURLModel)
                {
                    rez = await _mainControllerService.UploadImageFromURL(model as UploadImageURLModel, userid);
                }
                else if (model is UploadImageMapModel)
                {
                    rez = await _mainControllerService.UploadImageFromMaps(model as UploadImageMapModel, userid);
                }
                return JSON(rez.Item1, rez.Item2, rez.Item3);
            }
            else
            {
                return JSON("", 400, GetErrorsKeys());
            }
        }


        [HttpPost]
        public async Task<IActionResult> UploadImageFromFile(UploadImageFileModel model)
        {
            return await Upload<UploadImageFileModel>(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageFromURL([FromBody] UploadImageURLModel model)
        {
            return await Upload<UploadImageURLModel>(model);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageFromMap([FromBody] UploadImageMapModel model)
        {
            return await Upload<UploadImageMapModel>(model);
        }
    }
}