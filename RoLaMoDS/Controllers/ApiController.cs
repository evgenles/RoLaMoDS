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
<<<<<<< HEAD
=======
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        public ApiController( UserManager<UserModel> userManager, SignInManager<UserModel> signInManager){
            _userManager = userManager;
            _signInManager = signInManager;
        }
>>>>>>> 9d198b4b1633309de920499864efac7e3f9b23a2
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
<<<<<<< HEAD
=======

        public Guid GetUserId()
        {
            Guid userid = Guid.Empty;
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid.TryParse(id, out userid);
            return userid;
        }   


>>>>>>> 9d198b4b1633309de920499864efac7e3f9b23a2
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