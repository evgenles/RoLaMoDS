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

        /// <summary>
        /// Action for main view
        /// </summary>
        /// <returns>View</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Upload image to server
        /// </summary>
        /// <param name="model">Model to upload</param>
        /// <typeparam name="T">Type of image</typeparam>
        /// <returns>Rezult uploading</returns>
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

        /// <summary>
        /// Upload image from file
        /// </summary>
        /// <param name="model">Model of uploading file</param>
        /// <returns>Rezult uploading</returns>
        [HttpPost]
        public async Task<IActionResult> UploadImageFromFile(UploadImageFileModel model)
        {
            return await Upload<UploadImageFileModel>(model);
        }

        /// <summary>
        /// Upload image from URL
        /// </summary>
        /// <param name="model">Model of uploading image url</param>
        /// <returns>Rezult uploading</returns>
        [HttpPost]
        public async Task<IActionResult> UploadImageFromURL([FromBody] UploadImageURLModel model)
        {
            return await Upload<UploadImageURLModel>(model);
        }

        /// <summary>
        /// Upload image from maps
        /// </summary>
        /// <param name="model">Model of uploading maps</param>
        /// <returns>Rezult uploading</returns>
        [HttpPost]
        public async Task<IActionResult> UploadImageFromMap([FromBody] UploadImageMapModel model)
        {
            return await Upload<UploadImageMapModel>(model);
        }
    }
}