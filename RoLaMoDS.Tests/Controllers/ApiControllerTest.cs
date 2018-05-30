using RoLaMoDS.Controllers;
using Xunit;
using Newtonsoft.Json;
namespace RoLaMoDS.Tests.Controllers
{
    public class ApiControllerTest
    {
        ApiController ApiController = new ApiController();

        [Fact]
        public void JSONTest(){
            var result = ApiController.JSON(new {a = "Hello", b="World"},200,"Ok");
            Assert.Equal("{\"data\":{\"a\":\"Hello\",\"b\":\"World\"},\"code\":200,\"message\":\"Ok\"}",result.Value.ToString());
        }
    }
}