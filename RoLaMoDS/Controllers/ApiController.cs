using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RoLaMoDS.Models;
using System.Linq;
namespace RoLaMoDS.Controllers
{
    public class ApiController : Controller
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        public ApiController( UserManager<UserModel> userManager, SignInManager<UserModel> signInManager){
            _userManager = userManager;
            _signInManager = signInManager;
        }
        /// <summary>
        /// Get all error keys in modelstate
        /// </summary>
        /// <returns>Errors in string. Separeted by ";"</returns>
        protected string GetErrorsKeys()
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

        /// <summary>
        /// Get active user guid
        /// </summary>
        /// <returns>User Guid; Guid.Empty if unauthorize</returns>
        public Guid GetUserId()
        {
            Guid userid = Guid.Empty;
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(id, out userid);
            return userid;
        }   

        /// <summary>
        /// Get active user model
        /// </summary>
        /// <returns>User model</returns>
        public async Task<UserModel> GetActiveUser() =>
                await _userManager.GetUserAsync(User);

        /// <summary>
        /// Convert Data to jsonResult
        /// </summary>
        /// <param name="data">Object to return</param>
        /// <param name="code">State code</param>
        /// <param name="message">Message about error</param>
        /// <returns></returns>
        public static JsonResult JSON(object data, int code = 200, string message = "")
        {
            return new JsonResult(JsonConvert.SerializeObject(new
            {
                data = data,
                code = code,
                message = message
            }));
        }
    }
}