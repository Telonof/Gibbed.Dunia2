using System.Runtime.InteropServices;

namespace Gibbed.Dunia2.FileFormats
{
    internal class Oodle
    {
        private static class Windows64
        {
            [DllImport("oo2core_5_win64.dll")]
            internal static extern int OodleLZ_Decompress(byte[] Buffer, long BufferSize, byte[] OutputBuffer, long OutputBufferSize, uint a, uint b, uint c, uint d, uint e, uint f, uint g, uint h, uint i, int ThreadModule);

            [DllImport("oo2core_5_win64.dll")]
            internal static extern int OodleLZ_Compress(int version, byte[] inputBuffer, long bufferSize, byte[] outputBuffer, ulong level, uint a, uint b, uint c);
        }

        public static void Decompress(byte[] inputBytes,
                                      int inputCount,
                                      ref byte[] outputBytes,
                                      int outputSize)
        {
            _ = Windows64.OodleLZ_Decompress(inputBytes, inputCount, outputBytes, outputSize, 1, 0, 0, 0, 0, 0, 0, 0, 0, 3);
        }

        public static int Compress(byte[] inputBytes, int inputCount, ref byte[] outputBytes)
        {
            return Windows64.OodleLZ_Compress(8, inputBytes, inputCount, outputBytes, 4, 0, 0, 0);
        }
    }
}
