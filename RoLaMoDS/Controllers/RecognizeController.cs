using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RoLaMoDS.Models;
using System.Linq;
using RoLaMoDS.Data;
using RoLaMoDS.Models.ViewModels;
using System;
using Microsoft.EntityFrameworkCore;
using RoLaMoDS.Services.Interfaces;
using RoLaMoDS.Models.Enums;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace RoLaMoDS.Controllers
{
    public class RecognizeController : ApiController
    {
        private readonly IRecognizeService _recognizeService;
        public RecognizeController(UserManager<UserModel> userManager,
        SignInManager<UserModel> signInManager,
        IRecognizeService recognizeService
        ) : base(userManager, signInManager)
        {
           _recognizeService = recognizeService;
        }

        /// <summary>
        /// Get all public models neural network and models of active user
        /// </summary>
        /// <returns>JSON With models neural network or unauthorize</returns>
        public async Task<IActionResult> GetModels()
        {
            Guid userId = GetUserId();
            if (userId != Guid.Empty)
            {
                return JSON(await _recognizeService.GetUserModels(userId));
            }
            return JSON("", 401, "Unauthorized");
        }

        /// <summary>
        /// Get all object classes of model
        /// </summary>
        /// <param name="URL">URL to model</param>
        /// <returns>Object class</returns>
        public async Task<IActionResult> GetClassModels(string URL)
        {
           var rez =  await _recognizeService.GetClassOfModel(URL);
           return rez!=null?JSON(rez):JSON("",400,"Model unavailable");
        }


        /// <summary>
        /// Save pictures (or archive of pictures) on server, send pictures to train of recognize module. Data from form.
        /// </summary>
        /// <param name="model">Model to train</param>
        /// <returns>F1 Score of recognize valid data</returns>
        [HttpPost]
        public async Task<IActionResult> TrainModel(TrainViewModel model)
        {
           var rez = await _recognizeService.TrainModel(model);
            return rez.HasValue?JSON(rez):JSON("",401,"Unauthorize");
        }
        
        /// <summary>
        /// Recognize picture
        /// </summary>
        /// <param name="model">Model to recognize</param>
        /// <returns>Matrix of pictures and objects on it</returns>
        [HttpPost]
        public async Task<IActionResult> StartRecognize([FromBody]RecognizeViewModel model)
        {
            var rez = await _recognizeService.StartRecognize(model);
            return rez!=null?JSON(rez):JSON("",400,"Image unavailable");
        }
    }
}
