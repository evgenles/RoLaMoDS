using RoLaMoDS.Controllers;
using Xunit;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Moq;
namespace RoLaMoDS.Tests.Controllers
{
    public class ApiControllerTest
    {
        ApiController ApiController;

        public ApiControllerTest(){
            var MockUserManager = new Mock<UserManager<Models.UserModel>>();
            var MockSignInManager = new Mock<SignInManager<Models.UserModel>>();

            ApiController = new ApiController(MockUserManager.Object, MockSignInManager.Object);
        }

        [Fact]
        public void JSONTest(){
            var result = ApiController.JSON(new {a = "Hello", b="World"},200,"Ok");
            Assert.Equal("{\"data\":{\"a\":\"Hello\",\"b\":\"World\"},\"code\":200,\"message\":\"Ok\"}",result.Value.ToString());
        }
    }
}