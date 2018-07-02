using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoLaMoDS.Models.ViewModels;

namespace RoLaMoDS.Services.Interfaces
{
    public interface IRecognizeService
    {
        /// <summary>
        /// Get all public models and models for selected user
        /// </summary>
        /// <param name="userId">Userid of current user</param>
        /// <returns>List of all models</returns>
        Task <List<GetModelViewModel>> GetUserModels(System.Guid userId);

        /// <summary>
        /// Get all available class in model
        /// </summary>
        /// <param name="modelURL">URL model</param>
        /// <returns>Class list</returns>
        Task <List<GetModelClassesViewModel>> GetClassOfModel(string modelURL);

        /// <summary>
        /// [NOT CONNECTED TO RECOGNIZE MODULE] Save pictures (or archive of pictures) on server, send pictures to train of recognize module
        /// </summary>
        /// <param name="model">Model to train</param>
        /// <returns>F1 Score of recognize valid data</returns>
        Task<double?> TrainModel(TrainViewModel model);
        
        /// <summary>
        /// [NOT CONNECTED TO RECOGNIZE MODULE] Split image on cells and send it to recognize
        /// </summary>
        /// <param name="model">Images to recognize</param>
        /// <returns>Object`s in cells</returns>
        Task<RecognizeResultViewModel[,]> StartRecognize(RecognizeViewModel model);
    }
}