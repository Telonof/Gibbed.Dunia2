/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.IO;
using Gibbed.IO;

namespace Gibbed.Dunia2.FileFormats.Big
{
    internal class EntrySerializerV5 : IEntrySerializer
    {
        // uuuuuuuu 00000000 hhhhhhhh hhhhhhhh
        // cccccccc oooooooo

        // [u] uncompressed size = 4 bytes
        // [0] zeros = 4 bytes
        // [h] hash = 8 bytes
        // [c] compressed size = 4 bytes
        // [o] offset = 4 bytes

        public void Serialize(Stream output, Entry entry, Endian endian)
        {
            uint a = 0;
            a |= ((entry.UncompressedSize << 2) & 0xFFFFFFFCu);
            a |= (uint)(((byte)entry.CompressionScheme << 0) & 0x00000003u);

            var b = entry.author;

            var c = entry.NameHash;

            var d = entry.CompressedSize;

            var e = (uint)((entry.Offset & 0X00000003FFFFFFFCL) >> 2);

            output.WriteValueU32(a, endian);
            output.WriteValueU32(b, Endian.Big);
            output.WriteValueU64(c, endian);
            output.WriteValueU32(d, endian);
            output.WriteValueU32(e, endian);
        }

        public void Deserialize(Stream input, Endian endian, out Entry entry)
        {
            var a = input.ReadValueU32(Endian.Little);
            //Zeros
            input.ReadValueU32(Endian.Little);
            var b = input.ReadValueU64(Endian.Little);
            var c = input.ReadValueU32(Endian.Little);
            var d = input.ReadValueU32(Endian.Little);

            entry.UncompressedSize = (a & 0xFFFFFFFCu) >> 2;

            entry.author = 0;

            entry.CompressionScheme = (CompressionScheme)((a & 0x00000003u) >> 0);

            entry.NameHash = b;

            entry.CompressedSize = (uint)((c & 0x3FFFFFFFul) >> 0);

            entry.Offset = (long)d << 2;
            entry.Offset |= ((c & 0xC0000000u) >> 30);
        }
    }
}
