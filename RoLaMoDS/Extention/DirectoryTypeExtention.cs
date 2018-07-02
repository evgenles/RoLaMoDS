using RoLaMoDS.Models.Enums;
namespace RoLaMoDS.Extention
{
    public static class DirectoryTypeExtention
    {
        /// <summary>
        /// Convert DirectoryType to path string
        /// </summary>
        /// <param name="type">Type of dirrectory</param>
        /// <returns>Result path string </returns>
        public static string GetPathString(this DirectoryType type){
        string retStr ="";
            switch (type)
                {
                    case DirectoryType.Upload:
                       retStr = "uploads";
                        break;
                    case DirectoryType.Result:
                        retStr = "results";
                        break;
                    case DirectoryType.Recognize:
                        retStr = "recognize";
                        break;
                    case DirectoryType.Model:
                        retStr = "modelsNN";
                        break;
                }
                return retStr;
            }
    }
}