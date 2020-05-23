namespace Common.Extensions
{
    using System;

    public class HexExtensions
    {
        public static byte[] HexStringToBytes(string hexValue)
        {
            if (hexValue.Length % 2 == 1)
                throw new Exception("Hex string length must be a multiple of 2.");

            var bytes = new byte[hexValue.Length >> 1];

            for (var i = 0; i < hexValue.Length >> 1; ++i)
            {
                bytes[i] = (byte)((HexCharToInt(hexValue[i << 1]) << 4) + (HexCharToInt(hexValue[(i << 1) + 1])));
            }

            return bytes;
        }

        public static int HexCharToInt(char hex)
        {
            var val = (int)hex;

            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
