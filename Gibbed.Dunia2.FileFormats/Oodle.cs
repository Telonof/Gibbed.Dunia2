using System.Runtime.InteropServices;

namespace Gibbed.Dunia2.FileFormats
{
    internal class Oodle
    {
        private static class Windows64
        {
            [DllImport("oo2core_5_win64.dll")]
            internal static extern int OodleLZ_Decompress(byte[] Buffer, long BufferSize, byte[] OutputBuffer, long OutputBufferSize, uint a, uint b, uint c, uint d, uint e, uint f, uint g, uint h, uint i, int ThreadModule);
        }

        public static void Decompress(byte[] inputBytes,
                                      int inputCount,
                                      ref byte[] outputBytes,
                                      int outputSize)
        {
            Windows64.OodleLZ_Decompress(inputBytes, inputCount, outputBytes, outputSize, 1, 0, 0, 0, 0, 0, 0, 0, 0, 3);
        }
    }
}
