using UnityEngine;

namespace PROS.Water
{
    public class Kernel
    {
        private ComputeShader computeShader = null;

        public int index = 0;

        private Vector3Int size = new Vector3Int();


        public Kernel(ComputeShader computeShader, string name)
        {
            this.computeShader = computeShader;
            index = computeShader.FindKernel(name);
            computeShader.GetKernelThreadGroupSizes(index, out uint x, out uint y, out uint z);
            size = new Vector3Int((int)x, (int)y, (int)z);
        }


        public void Invoke(int textureSize)
        {
            computeShader.Dispatch(index, textureSize / size.x, textureSize / size.y, 1);
        }
    }
}