using Microsoft.AspNetCore.Hosting;
using RoLaMoDS.Models.Enums;
using RoLaMoDS.Services.Interfaces;
using System.IO;

namespace RoLaMoDS.Services
{
    public class FileService : IFileService
    {
        private readonly IHostingEnvironment _env;
        public FileService(IHostingEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// Create directories /wwwroot/images, /wwwroot/images/uploads,
        /// /wwwroot/images/results, /wwwroot/images/recognize
        /// </summary>
        public void CreateImagePathes()
        {
            var imgDir = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(imgDir))
            {
                Directory.CreateDirectory(imgDir);
            }
            var uploadDir = Path.Combine(imgDir, "uploads");
            var resultDir = Path.Combine(imgDir, "results");
            var recognizeDir = Path.Combine(imgDir, "recognize");

            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }
            if (!Directory.Exists(resultDir))
            {
                Directory.CreateDirectory(resultDir);
            }
            if (!Directory.Exists(recognizeDir))
            {
                Directory.CreateDirectory(recognizeDir);
            }
        }

        /// <summary>
        /// Get name next file (if directory with new file size > 65535, creates new directory)
        /// </summary>
        /// <param name="CountFile">Counter of uploading files (for reservation)</param>
        /// <param name="DType">Type of directory</param>
        /// <returns>File patches</returns>
        public string[] GetNextFilesPath(int CountFile, DirectoryType DType)
        {
            var imgDir = Path.Combine(_env.WebRootPath, "images");
            switch (DType)
            {
                case DirectoryType.Upload:
                    imgDir = Path.Combine(imgDir, "uploads");
                    break;
                case DirectoryType.Result:
                    imgDir = Path.Combine(imgDir, "results");
                    break;
                case DirectoryType.Recognize:
                    imgDir = Path.Combine(imgDir, "recognize");
                    break;
            }
            var directories = Directory.GetDirectories(imgDir);
            string fileDirectory = null;
            if (directories.Length == 0)
            {
                fileDirectory = Directory.CreateDirectory(Path.Combine(imgDir, "1")).FullName;
            }
            else
            {
                foreach (string directory in directories)
                {
                    var fullName = Path.Combine(imgDir, directory);
                    if (Directory.GetFiles(fullName).Length < ushort.MaxValue - CountFile)
                    {
                        fileDirectory = fullName;
                        break;
                    }
                }
                if (fileDirectory == null)
                {
                    fileDirectory = Directory.CreateDirectory(Path.Combine(imgDir,
                        (System.Convert.ToUInt16(directories[directories.Length - 1]
                            .Remove(0, directories[directories.Length - 1].LastIndexOf("\\uploads\\")+9)) + 1)
                            .ToString())).FullName;
                }
            }
            string[] rezultNames = new string[CountFile];
            for (int i = 0; i < CountFile; i++)
                rezultNames[i] = Path.Combine(fileDirectory, Path.GetRandomFileName() + ".bmp");
            return rezultNames;
        }


    }
}