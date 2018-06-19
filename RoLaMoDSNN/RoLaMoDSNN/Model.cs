using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;
using CNTK;
using System.IO;
namespace RoLaMoDSNN
{
    class Model
    {
        public enum Activation
        {
            None,
            ReLU,
            Sigmoid,
            Tanh
        }

        public Function Classifier;
        private int[] ImageDim;
        public int size { get; set; }
        public int hStride { get; set; }
        public int vStride { get; set; }
        public int poolingWindowWidth { get; set; }
        public int poolingWindowHeight { get; set; }
        public int KernelWidth { get; set; }
        public int KernelHeight { get; set; }
        public double ConvolutionScale { get; set; }
        private int NumClasses { get; set; }
        private DeviceDescriptor device;
        private Variable input;
        private Function ScaledInput;
        private MinibatchSource minibatchSource;
        private StreamInformation imageStreamInfo;
        private StreamInformation labelStreamInfo;

        public List<int> Recognize(string Path)
        {
            var outputDataMap = new Dictionary<Variable, Value>() {
                    { Classifier.Output, null } };
            Variable inputVar = Classifier.Arguments.Single();
            NDShape inputShape = inputVar.Shape;
            int imageWidth = inputShape[0];
            int imageHeight = inputShape[1];
            string sampleImage = Path;
            Bitmap bmp = new Bitmap(Bitmap.FromFile(sampleImage));
            var resized = bmp.Resize(imageWidth, imageHeight, true);
            List<float> resizedCHW = resized.ParallelExtractCHW();
            var inputDataMap = new Dictionary<Variable, Value>();
            var inputVal = Value.CreateBatch(inputShape, resizedCHW, device);
            inputDataMap.Add(inputVar, inputVal);



            Classifier.Evaluate(inputDataMap, outputDataMap, device);
            var outputData = outputDataMap[Classifier.Output].GetDenseData<float>(Classifier.Output);
            return outputData.Select(l => l.IndexOf(l.Max())).ToList();

        }

        public Model(string FilePath, string OnDevice)
        {
            if (OnDevice == "CPU")
                device = CNTK.DeviceDescriptor.CPUDevice;
            else
                device = CNTK.DeviceDescriptor.GPUDevice(0);
            Classifier = Function.Load(FilePath, device);
            var a = Classifier.Arguments[0].Shape.Dimensions;
            ImageDim = new int[3];
            ImageDim[0] = a[0];
            ImageDim[1] = a[1];
            ImageDim[2] = a[2];
            // Classifier.Arguments
        }
        public Model(int[] imageDim, string OnDevice, int numClasses, double convolutionScale)
        {
            NumClasses = numClasses;
            ImageDim = imageDim;
            size = Math.Min(imageDim[0], imageDim[1]);
            hStride = 2;
            vStride = 2;
            poolingWindowHeight = 3;
            poolingWindowWidth = 3;
            KernelWidth = 3;
            KernelHeight = 3;
            ConvolutionScale = convolutionScale;
            
            if (OnDevice == "CPU")
                device = CNTK.DeviceDescriptor.CPUDevice;
            else
                device = CNTK.DeviceDescriptor.GPUDevice(0);

            CreateConvolutionalNeuralNetwork( "classifier");

        }


        public void SaveModel(string FilePath = "")
        {
            // if (FilePath == "")
            FilePath = Path.Combine("D:\\", "hello.txt");
            Classifier.Save(FilePath);
        }

        /// <param name="input"></param>
        /// <param name="outputDim"></param>
        /// <param name="device"></param>
        /// <param name="activation"></param>
        /// <param name="outputName"></param>
        /// <returns></returns>
        public static Function Dense(Variable input, int outputDim, DeviceDescriptor device,
        Activation activation = Activation.None, string outputName = "")
        {
            if (input.Shape.Rank != 1)
            {
                // 
                int newDim = input.Shape.Dimensions.Aggregate((d1, d2) => d1 * d2);
                input = CNTKLib.Reshape(input, new int[] { newDim });
            }



            Function fullyConnected = FullyConnectedLinearLayer(input, outputDim, device, outputName);
            switch (activation)
            {
                default:
                case Activation.None:
                    return fullyConnected;
                case Activation.ReLU:
                    return CNTKLib.ReLU(fullyConnected);
                case Activation.Sigmoid:
                    return CNTKLib.Sigmoid(fullyConnected);
                case Activation.Tanh:
                    return CNTKLib.Tanh(fullyConnected);
            }
        }

        public void Training(string filePath)
        {
            ImageLoader(filePath);
            var imageInput = CNTKLib.InputVariable(ImageDim, imageStreamInfo.m_elementType, "features");
            var labelsVar = CNTKLib.InputVariable(new int[] { NumClasses }, labelStreamInfo.m_elementType, "labels");
            var trainingLoss = CNTKLib.CrossEntropyWithSoftmax(new Variable(Classifier), labelsVar, "lossFunction");
            var prediction = CNTKLib.ClassificationError(new Variable(Classifier), labelsVar, 5, "predictionError");

            var learningRatePerSample = new TrainingParameterScheduleDouble(0.0078125, 1);
            var trainer = Trainer.CreateTrainer(Classifier, trainingLoss, prediction,
                new List<Learner> { Learner.SGDLearner(Classifier.Parameters(), learningRatePerSample) });

            uint minibatchSize = 5;
            // int outputFrequencyInMinibatches = 20, miniBatchCount = 0;
            while (true)
            {

                var minibatchData = minibatchSource.GetNextMinibatch(minibatchSize, device);
                if (minibatchData.empty())
                {
                    break;
                }
                var arguments = new Dictionary<Variable, MinibatchData>()
                {
                    { input, minibatchData[imageStreamInfo] },
                    { labelsVar, minibatchData[labelStreamInfo] }
                };
                // Stop training once max epochs is reached.
                trainer.TrainMinibatch(arguments, device);
                //  TestHelper.PrintTrainingProgress(trainer, miniBatchCount++, outputFrequencyInMinibatches);
            }


        }

        private static Function FullyConnectedLinearLayer(Variable input, int outputDim, DeviceDescriptor device,
            string outputName = "output")
        {
            System.Diagnostics.Debug.Assert(input.Shape.Rank == 1);

            int inputDim = input.Shape[0];

            int[] s = { outputDim, inputDim };
            var timesParam = new Parameter((NDShape)s, DataType.Float,
                CNTKLib.GlorotUniformInitializer(
                    CNTKLib.DefaultParamInitScale,
                    CNTKLib.SentinelValueForInferParamInitRank,
                    CNTKLib.SentinelValueForInferParamInitRank, 1),
                device, "timesParam");
            var timesFunction = CNTKLib.Times(timesParam, input, "times");

            int[] s2 = { outputDim };
            var plusParam = new Parameter(s2, 0.0f, device, "plusParam");
            return CNTKLib.Plus(plusParam, timesFunction, outputName);
        }


        public double[] ValidateModel(string Path, int maxCount = 1000)
        {
            int[,] numarray = new int[2, NumClasses];
            ImageLoader(Path);
            var labelOutput = Classifier.Output;
            int batchSize = 10;
            int totalCount = 0;
            while (true)
            {
                var minibatchData = minibatchSource.GetNextMinibatch((uint)batchSize, device);
                if (minibatchData == null || minibatchData.Count == 0)
                    break;
                totalCount += (int)minibatchData[imageStreamInfo].numberOfSamples;

                // expected labels are in the minibatch data.
                var labelData = minibatchData[labelStreamInfo].data.GetDenseData<float>(labelOutput);
                var expectedLabels = labelData.Select(l => l.IndexOf(l.Max())).ToList();

                var inputDataMap = new Dictionary<Variable, Value>() {
                    { labelOutput, minibatchData[imageStreamInfo].data }
                };

                var outputDataMap = new Dictionary<Variable, Value>() {
                    { Classifier.Output, null }
                };

                Classifier.Evaluate(inputDataMap, outputDataMap, device);
                var outputData = outputDataMap[labelOutput].GetDenseData<float>(labelOutput);
                var actualLabels = outputData.Select(l => l.IndexOf(l.Max())).ToList();

                //int misMatches = actualLabels.Zip(expectedLabels, (a, b) => a.Equals(b) ? 0 : 1).Sum();
                for (int i = 0; i < expectedLabels.Count; i++)
                {
                    if (expectedLabels[i] == actualLabels[i])
                        numarray[1, expectedLabels[i]]++;
                    numarray[0, expectedLabels[i]]++;

                }

                if (totalCount > maxCount)
                    break;
            }
            double[] error = new double[NumClasses];
            for (int i = 0; i < NumClasses; i++)
                error[i] = numarray[1, i] / numarray[0, i];

            return error;
        }

        void CreateConvolutionalNeuralNetwork(string classifierName)
        {
            input = CNTKLib.InputVariable(ImageDim, DataType.Float, "features");
            var scaledInput = CNTKLib.ElementTimes(Constant.Scalar<float>(0.00390625f, device), input);
            int size = Math.Min(ImageDim[0], ImageDim[1]);
            int kernelWidth = KernelWidth + size * 3 / 100;
            int kernelHeight = KernelHeight + size * 3 / 100;
            int numInputChannels = ImageDim[2], outFeatureMapCount = 9;
            Function pooling1 = ConvolutionWithMaxPooling(scaledInput, device, kernelWidth, kernelHeight,
                    numInputChannels, outFeatureMapCount);
            size = size / 3;
            while (size>3)
            {
                size = size / 3;    
                kernelWidth = 3 + size * 3 / 100;
                kernelHeight = 3 + size * 3 / 100;
                numInputChannels = outFeatureMapCount;
                outFeatureMapCount *= 3;
                pooling1 = ConvolutionWithMaxPooling(pooling1, device, kernelWidth, kernelHeight,
                    numInputChannels, outFeatureMapCount);
            }

            Classifier = Dense(pooling1, NumClasses, device, Activation.ReLU, classifierName);

        }
        //Function CreateConvolutionalNeuralNetwork(Variable features, int outDims, DeviceDescriptor device, string classifierName)
        //{
        //    // 28x28x1 -> 14x14x4
        //    int kernelWidth1 = 3, kernelHeight1 = 3, numInputChannels1 = 3, outFeatureMapCount1 = 4;
        //    int hStride1 = 2, vStride1 = 2;
        //    int poolingWindowWidth1 = 3, poolingWindowHeight1 = 3;
        //    Function pooling1 = ConvolutionWithMaxPooling(features, device, kernelWidth1, kernelHeight1,
        //        numInputChannels1, outFeatureMapCount1, hStride1, vStride1, poolingWindowWidth1, poolingWindowHeight1);
        //    // 14x14x4 -> 7x7x8
        //    int kernelWidth2 = 3, kernelHeight2 = 3, numInputChannels2 = outFeatureMapCount1, outFeatureMapCount2 = 8;
        //    int hStride2 = 2, vStride2 = 2;
        //    int poolingWindowWidth2 = 3, poolingWindowHeight2 = 3;
        //    Function pooling2 = ConvolutionWithMaxPooling(pooling1, device, kernelWidth2, kernelHeight2,
        //        numInputChannels2, outFeatureMapCount2, hStride2, vStride2, poolingWindowWidth2, poolingWindowHeight2);
        //    Function denseLayer = Dense(pooling2, outDims, device, Activation.None, classifierName);
        //    return denseLayer;
        //}
        private void ImageLoader(string MapFilePath)
        {
            List<CNTKDictionary> transforms = new List<CNTKDictionary>{
                CNTKLib.ReaderScale(ImageDim[0], ImageDim[1], ImageDim[2])
      //          CNTKLib.ReaderMean(meanFilePath)
            };
            var deserializerConfiguration = CNTKLib.ImageDeserializer(MapFilePath,
                "labels", (uint)NumClasses,
                "features",
                transforms);

            MinibatchSourceConfig config = new MinibatchSourceConfig(new List<CNTKDictionary> { deserializerConfiguration })
            {
                MaxSweeps = 50
            };

            minibatchSource = CNTKLib.CreateCompositeMinibatchSource(config);
            imageStreamInfo = minibatchSource.StreamInfo("features");
            labelStreamInfo = minibatchSource.StreamInfo("labels");


        }


        private Function ConvolutionWithMaxPooling(Variable features, DeviceDescriptor device,
            int kernelWidth, int kernelHeight, int numInputChannels, int outFeatureMapCount)
        {
            var convParams = new Parameter(new int[] { kernelWidth, kernelHeight, numInputChannels, outFeatureMapCount }, DataType.Float,
                CNTKLib.GlorotUniformInitializer(ConvolutionScale, -1, 2), device);
            Function convFunction = CNTKLib.ReLU(CNTKLib.Convolution(convParams, features, new int[] { 1, 1, numInputChannels } /* strides */));

            Function pooling = CNTKLib.Pooling(operand: convFunction.Output, poolingType: PoolingType.Max,
                poolingWindowShape: new int[] { poolingWindowWidth, poolingWindowHeight }, strides: new int[] { hStride, vStride }, autoPadding: new bool[] { true });
            return pooling;
        }
    }
}
