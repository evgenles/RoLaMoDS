using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoLaMoDS.Data;
using RoLaMoDS.Models.UserViewModels;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using RoLaMoDS.Models;

namespace RoLaMoDS.Controllers
{
    public class UserController : ApiController
    {
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public UserController(ApplicationDBContext applicationDBContext, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _applicationDBContext = applicationDBContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = new UserModel { UserName = model.Login, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return JSON(model.Login);
                }
                else
                {
                    //TODO: Register error
                }
            }
            return JSON(null, 400, "");
        }

        public async Task<IActionResult> SignIn([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Login,
                   model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return JSON(model.Login);
                }
                //TODO: SignInError
            }
            return JSON(null,400,"");
        }
    }
}