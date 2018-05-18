using System.Collections.Generic;

namespace RoLaMoDS.Services.Interfaces
{
    public interface IImageWorkerService
    {
        bool SeparateImageOnCell();
        bool GetNextCell(out byte[] result);
        byte[] MakeBorderOnCell(byte[] input);
        byte[] FormResultImage(IEnumerable<byte[]> cells); 
    }
}