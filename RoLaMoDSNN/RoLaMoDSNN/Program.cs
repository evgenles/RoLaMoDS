using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNTK;

namespace RoLaMoDSNN
{
    class Program
    {
        static void Main(string[] args)
        {
            NNWorker w = new NNWorker();
            //w.CreateNewModel("../model", new int[] { 100, 100, 3 }, 40, "CPU", 0.1);
            //w.Train("D:\\hello","D:\\hello.txt");
            w.LoadModel("D:\\hello.txt", "CPU");
            var a = w.Recognize("D:\\hello.txt", "D:\\Vasilisk1.jpg", "");

            w.Dispose();    
            
            Console.Write("Done");
        }
    }
}
