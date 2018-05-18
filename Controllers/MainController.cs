using Microsoft.AspNetCore.Mvc;

namespace RoLaMoDS.Controllers
{
    public class MainController:Controller
    {
        public IActionResult Index(){
            return View();
        }
    }
}