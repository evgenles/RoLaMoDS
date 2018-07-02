using RoLaMoDS.Services;
using RoLaMoDS.Services.Interfaces;
using Xunit;
using System.IO;
using Moq;
using System.Linq;
using RoLaMoDS.Models.Enums;

namespace RoLaMoDS.Tests.Services
{
    public class FileServiceTests : System.IDisposable
    {
        private readonly IFileService _fileService;
        public FileServiceTests()
        {
            var MockEnvironment = new Mock<Microsoft.AspNetCore.Hosting.IHostingEnvironment>();
            MockEnvironment.Setup(env => env.WebRootPath)
               .Returns(Directory.GetCurrentDirectory() + @"\wwwroot\");
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\wwwroot")) Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\wwwroot");
            _fileService = new FileService(MockEnvironment.Object);

        }
        [Fact]
        public void CreatePatchesTest1()
        {
            var path = (Directory.GetCurrentDirectory() + @"\wwwroot\images");
            if (Directory.Exists(path)) Directory.Delete(path, true);
            _fileService.CreateImagePathes();
            Assert.True(Directory.Exists(path));
            var items = Directory.GetDirectories(path).Select(i => i.Replace(path + "\\", ""));
            Assert.All(items.ToArray(), i => Assert.Contains(i, new string[] { "uploads", "results", "recognize" }));

        }
        
        [Fact]
        public void CreatePatchesTest2()
        {
            var path = (Directory.GetCurrentDirectory() + @"\wwwroot\images");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var upl = Path.Combine(path,"uploads");
            var rez = Path.Combine(path,"results");
            var rec = Path.Combine(path,"recognize");

            if (!Directory.Exists(upl)) Directory.CreateDirectory(upl);
            if (!Directory.Exists(rez)) Directory.CreateDirectory(rez);
            if (!Directory.Exists(rec)) Directory.CreateDirectory(rec);

            _fileService.CreateImagePathes();
            Assert.True(Directory.Exists(path));
            var items = Directory.GetDirectories(path).Select(i => i.Replace(path + "\\", ""));
            Assert.All(items.ToArray(), i => Assert.Contains(i, new string[] { "uploads", "results", "recognize" }));

        }
        
        [Fact]
        public void GetNextFileTest1()
        {
            var path = (Directory.GetCurrentDirectory() + @"\wwwroot\images");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = Path.Combine(path, "uploads");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            
            var resultPatches = _fileService.GetNextFilesPath(5, DirectoryType.Upload);
            Assert.Equal(5,resultPatches.Length);
            
        }

        [Fact]
        public void GetNextFileTest2()
        {
            var path = (Directory.GetCurrentDirectory() + @"\wwwroot\images");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = Path.Combine(path, "results");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            
            var resultPatches = _fileService.GetNextFilesPath(10, DirectoryType.Result);
            Assert.Equal(10,resultPatches.Length);
            foreach(var rp in resultPatches){
                File.Create(rp).Close();
            }
            Assert.Equal(10, Directory.GetFiles(Path.Combine(path,"1")).Length);
            resultPatches = _fileService.GetNextFilesPath(65530, DirectoryType.Result);
            Assert.Equal(65530, resultPatches.Length);
            
            Assert.Equal(10,Directory.GetFiles(Path.Combine(path,"1")).Length);
            Assert.All(resultPatches, rp=>rp.Contains(@"\results\2\"));
     
        }

        [Fact]
        public void GetNextFileTest3()
        {
            var path = (Directory.GetCurrentDirectory() + @"\wwwroot\images");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            path = Path.Combine(path, "recognize");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var path1 = Path.Combine(path, "1");
             if (!Directory.Exists(path1)) Directory.CreateDirectory(path1);

            var resultPatches = _fileService.GetNextFilesPath(0, DirectoryType.Recognize);
            Assert.Equal(0,resultPatches.Length);
        }
        public void Dispose()
        {
            var path = (Directory.GetCurrentDirectory() + @"\wwwroot");
            if (Directory.Exists(path)) Directory.Delete(path, true);
        }
    }
}