using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace RoLaMoDS.Controllers
{
    public class ApiController : Controller
    {
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