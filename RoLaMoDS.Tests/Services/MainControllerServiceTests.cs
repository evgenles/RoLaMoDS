using Xunit;
using RoLaMoDS.Services.Interfaces;
using RoLaMoDS.Services;
using RoLaMoDS.Models;
using Moq;
using System.IO;
using System.Linq;
using System;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace RoLaMoDS.Tests.Services
{
    public class MainControllerServiceTests : IDisposable
    {
        private readonly IMainControllerService MainControllerService;
        public MainControllerServiceTests()
        {
            var FileService = new Mock<IFileService>();
            var cd = Directory.GetCurrentDirectory();
            if (!Directory.Exists(Path.Combine(cd, "images")))
                Directory.CreateDirectory(Path.Combine(cd, "images"));
            FileService.Setup(fs => fs.GetNextFilesPath(1, DirectoryType.Upload))
                .Returns(new string[] { Path.Combine(cd, "images", "101.bmp") }
                .ToArray());
            FileService.Setup(fs => fs.GetNextFilesPath(5, DirectoryType.Recognize))
                .Returns(new string[] { "1", "2", "3", "4", "5" }.Select(s => Path.Combine(cd, "images", s + ".bmp"))
                .ToArray());
            var ImageService = new Mock<IImageWorkerService>();
            MainControllerService = new MainControllerSevice(ImageService.Object, FileService.Object);
        }

        public void Dispose()
        {
            Directory.Delete(
                Path.Combine(Directory.GetCurrentDirectory(), "images"), true);
        }

        [Fact]
        public void FileUploadTest()
        {
            var FileMoq = new Mock<IFormFile>();
            var cd = Directory.GetCurrentDirectory();
            FileMoq.Setup(f => f.OpenReadStream())
                .Returns(new FileStream($"{cd}\\..\\..\\..\\..\\RoLaMoDS\\Capture.png", FileMode.Open));

            var model = new UploadImageFileModel
            {
                File = FileMoq.Object,
                Latitude = 0,
                Longitude = 0,
                Scale = 17
            };
            var ret = MainControllerService.UploadImageFromFile(model);
            Assert.True(File.Exists(cd + "\\images\\101.bmp"));
            var expected = (new { resultImagePath = "\\images\\101.bmp" }, 200, "");
            Assert.Equal(expected.Item2, ret.Item2);
            Assert.Equal(expected.Item3, ret.Item3);
            Assert.Equal(expected.Item1.GetHashCode(), ret.Item1.GetHashCode());
            File.Delete(cd + "\\images\\101.bmp");
        }

        [Fact]
        ///WARNING! Need enternet
        public async Task URLUploadTest()
        {
            var cd = Directory.GetCurrentDirectory();
            var model = new UploadImageURLModel
            {
                URL = @"https://www.google.com.ua/images/branding/googlelogo/2x/googlelogo_color_272x92dp.png",
                Latitude = 0,
                Longitude = 0,
                Scale = 17
            };
            var ret = await MainControllerService.UploadImageFromURL(model);
            Assert.True(File.Exists(cd + "\\images\\101.bmp"));
            var expected = (new { resultImagePath = "\\images\\101.bmp" }, 200, "");
            Assert.Equal(expected.Item2, ret.Item2);
            Assert.Equal(expected.Item3, ret.Item3);
            Assert.Equal(expected.Item1.GetHashCode(), ret.Item1.GetHashCode());
            File.Delete(cd + "\\images\\101.bmp");
        }

        [Fact]
        ///WARNING! Need enternet
        public async Task URLErrorUploadTest()
        {
            var cd = Directory.GetCurrentDirectory();
            var model = new UploadImageURLModel
            {
                URL = @"https://notfoundurl.nf/",
                Latitude = 0,
                Longitude = 0,
                Scale = 17
            };
            var ret = await MainControllerService.UploadImageFromURL(model);
            Assert.False(File.Exists(cd + "\\images\\101.bmp"));
            var expected = ("", 404, "Host_Not_found");
            Assert.Equal(expected.Item2, ret.Item2);
            Assert.Equal(expected.Item3, ret.Item3);
            Assert.Equal(expected.Item1.GetHashCode(), ret.Item1.GetHashCode());
            File.Delete(cd + "\\images\\101.bmp");
        }

        [Fact]
        ///WARNING! Need enternet
        public async Task MapBingUploadTest()
        {
            var cd = Directory.GetCurrentDirectory();
            var model = new UploadImageMapModel
            {
                Latitude = 55,
                Longitude = 48,
                Scale = 17,
                Zoom = 17,
                MapType = MapTypes.Bing
            };
            var ret = await MainControllerService.UploadImageFromMaps(model);
            Assert.True(File.Exists(cd + "\\images\\101.bmp"));
            var expected = (new { resultImagePath = "\\images\\101.bmp" }, 200, "");
            Assert.Equal(expected.Item2, ret.Item2);
            Assert.Equal(expected.Item3, ret.Item3);
            Assert.Equal(expected.Item1.GetHashCode(), ret.Item1.GetHashCode());
            File.Delete(cd + "\\images\\101.bmp");
        }


        
        [Fact]
        ///WARNING! Need enternet
        public async Task MapBingUploadWhiteImageTest()
        {
            var cd = Directory.GetCurrentDirectory();
            var model = new UploadImageMapModel
            {
                Latitude = 55,
                Longitude = 48,
                Scale = 17,
                Zoom = 20,
                MapType = MapTypes.Bing
            };
            var ret = await MainControllerService.UploadImageFromMaps(model);
            Assert.False(File.Exists(cd + "\\images\\101.bmp"));
            var expected =  ("", 400, "Image_So_Zommed");
            Assert.Equal(expected.Item2, ret.Item2);
            Assert.Equal(expected.Item3, ret.Item3);
            Assert.Equal(expected.Item1.GetHashCode(), ret.Item1.GetHashCode());
            File.Delete(cd + "\\images\\101.bmp");
        }
        

        [Fact]
        ///WARNING! Need enternet
        public async Task MapGoogleUploadTest()
        {
            var cd = Directory.GetCurrentDirectory();
            var model = new UploadImageMapModel
            {
                Latitude = 55,
                Longitude = 48,
                Scale = 17,
                Zoom = 17,
                MapType = MapTypes.Google
            };
            var ret = await MainControllerService.UploadImageFromMaps(model);
            Assert.True(File.Exists(cd + "\\images\\101.bmp"));
            var expected = (new { resultImagePath = "\\images\\101.bmp" }, 200, "");
            Assert.Equal(expected.Item2, ret.Item2);
            Assert.Equal(expected.Item3, ret.Item3);
            Assert.Equal(expected.Item1.GetHashCode(), ret.Item1.GetHashCode());
            File.Delete(cd + "\\images\\101.bmp");
        }

         [Fact]
        ///WARNING! Need enternet
        public async Task MapGoogleUploadWhiteImageTest()
        {
            var cd = Directory.GetCurrentDirectory();
            var model = new UploadImageMapModel
            {
                Latitude = 55,
                Longitude = 48,
                Scale = 17,
                Zoom = 21,
                MapType = MapTypes.Google
            };
            var ret = await MainControllerService.UploadImageFromMaps(model);
            Assert.False(File.Exists(cd + "\\images\\101.bmp"));
            var expected =  ("", 400, "Image_So_Zommed");
            Assert.Equal(expected.Item2, ret.Item2);
            Assert.Equal(expected.Item3, ret.Item3);
            Assert.Equal(expected.Item1.GetHashCode(), ret.Item1.GetHashCode());
            File.Delete(cd + "\\images\\101.bmp");
        }
    }
}