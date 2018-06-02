using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoLaMoDS.Data;
using RoLaMoDS.Models.UserViewModels;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using RoLaMoDS.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using RoLaMoDS.Models.ViewModels;
using System;

namespace RoLaMoDS.Controllers
{
    public class UserController : ApiController
    {
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly ILogger _logger;

        public UserController(ApplicationDBContext applicationDBContext,
                            UserManager<UserModel> userManager,
                            SignInManager<UserModel> signInManager,
                            ILogger<UserController> logger):base(userManager, signInManager)
        {
            _applicationDBContext = applicationDBContext;
            _userManager = userManager;

            _signInManager = signInManager;
            _logger = logger;
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
            return JSON(null, 400, "");
        }

        public IActionResult Lol()
        {
            return Ok();
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Main");
        }

        public IActionResult OAuth(string provider)
        {
            var redirectUrl = Url.Action(nameof(OAuthCallBack), "User");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        public async Task<IActionResult> OAuthCallBack(string remoteError = null)
        {
            if (remoteError != null)
            {
                return JSON("", 400, $"Error from external provider: {remoteError}");
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Index", "Main", "SignIn");
            }
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                User.Claims.Append(new Claim("Provider","ExternalAuth"));
                _logger.LogInformation($"User logged in with {info.LoginProvider} provider.");
                return RedirectToAction("Index", "Main");
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            { //If user exist add to him this auth
                var resultAddLogin = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation($"User add login to account using {info.LoginProvider} provider.");
                    return RedirectToAction("Index", "Main");
                }
                else
                {
                    //Error
                }
            }
            return RedirectToAction("Index", "Main", "OAuthRegister");
        }

        public async Task<IActionResult> OAuthRegister([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var user = new UserModel { UserName = model.Login, Email = email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation($"User created an account using {info.LoginProvider} provider.");
                        return RedirectToAction("Index", "Main");

                    }
                }
                //ToDO: Errors
            }
            return JSON("",400,"Has errors");
        }
    }
}