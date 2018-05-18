using System.Collections.Generic;
using RoLaMoDS.Services.Interfaces;
namespace RoLaMoDS.Services
{
    public class ImageWorkerService : IImageWorkerService
    {
        public byte[] FormResultImage(IEnumerable<byte[]> cells)
        {
            throw new System.NotImplementedException();
        }

        public bool GetNextCell(out byte[] result)
        {
            throw new System.NotImplementedException();
        }

        public byte[] MakeBorderOnCell(byte[] input)
        {
            throw new System.NotImplementedException();
        }

        public bool SeparateImageOnCell()
        {
            throw new System.NotImplementedException();
        }
    }
}