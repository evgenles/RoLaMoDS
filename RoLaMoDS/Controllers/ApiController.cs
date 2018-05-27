using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace RoLaMoDS.Controllers
{
    public class ApiController:Controller
    {
        public static JsonResult JSON(object data, int code = 200, string message =""){
            return new JsonResult(JsonConvert.SerializeObject(new {
                data = data,
                code = code,
                message = message
            }));
        }
    }
}