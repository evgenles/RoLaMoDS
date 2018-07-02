using CNTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoLaMoDSNN
{
    class NNWorker : IDisposable
    {
        Dictionary<string, Model> models = new Dictionary<string, Model>();

        public void CreateNewModel(string ModelPath, int[] imageDim, int numClasses, string OnDevice, double ConvolutionScale)
        {
            var model = models.SingleOrDefault(a => a.Key == ModelPath).Value;
            if (model != null)
                models.Remove(ModelPath);

            model = new Model(imageDim, OnDevice, numClasses, ConvolutionScale);
            models.Add(ModelPath, model);
            //// create chois of gpu
        }
        public (List<int>,string) Recognize(string ModelPath, string SetelPath, string TopPath)
        {
            var model = models?.SingleOrDefault(a => a.Key == ModelPath).Value;
            if (model == null)
                return (null, "Error: Model not exist");
            
            return (model.Recognize(SetelPath), "");
        }
        public string LoadModel(string path, string OnDevice)
        {
            var model = models?.SingleOrDefault(a => a.Key == path).Value;
            //if (model != null)
            //    return "Error: model exist";
            model = new Model(path, OnDevice);
            
            models.Add(path,model);
            
            return "Model load";
        }
        public (double[],string) ValidateModel(string ModelPath,string MapFilePath)
        {
            var model = models.SingleOrDefault(a => a.Key == ModelPath).Value;
            if (model == null)
                return (null, "Error: Model not exist");
            return (model.ValidateModel(ModelPath),"");
        }

        public string Train(string ModelPath,string MapPath)
        {
            var model = models.SingleOrDefault(a => a.Key == ModelPath).Value;
            if (model == null)
                return "Error: Model not exist";
            model.Training(MapPath);
            return "";
        }

        public void Dispose()
        {
            foreach (var i in models)
            {
                i.Value.SaveModel(i.Key);
            }
        }

        public NNWorker()
        {
            
        }


    }
}
