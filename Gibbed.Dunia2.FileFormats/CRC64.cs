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

namespace Gibbed.Dunia2.FileFormats
{
    public static class CRC64
    {
        public static ulong Hash(string value, bool jones = false)
        {
            ulong hash = 0ul;
            for (int i = 0; i < value.Length; i++)
            {
                if (jones)
                    hash = JonesVariantTable[(byte)hash ^ (byte)value[i]] ^ (hash >> 8);
                else
                    hash = _Table[(byte)hash ^ (byte)value[i]] ^ (hash >> 8);
            }
            return hash;
        }

        public static ulong Hash(byte[] buffer, int offset, int length)
        {
            return Hash(buffer, offset, length, 0ul);
        }

        public static ulong Hash(byte[] buffer, int offset, int length, ulong hash)
        {
            for (int i = offset; i < offset + length; i++)
            {
                hash = _Table[(byte)hash ^ buffer[i]] ^ (hash >> 8);
            }
            return hash;
        }

        private static readonly ulong[] _Table =
            {
                0x0000000000000000ul, 0x01B0000000000000ul, 0x0360000000000000ul, 0x02D0000000000000ul,
                0x06C0000000000000ul, 0x0770000000000000ul, 0x05A0000000000000ul, 0x0410000000000000ul,
                0x0D80000000000000ul, 0x0C30000000000000ul, 0x0EE0000000000000ul, 0x0F50000000000000ul,
                0x0B40000000000000ul, 0x0AF0000000000000ul, 0x0820000000000000ul, 0x0990000000000000ul,
                0x1B00000000000000ul, 0x1AB0000000000000ul, 0x1860000000000000ul, 0x19D0000000000000ul,
                0x1DC0000000000000ul, 0x1C70000000000000ul, 0x1EA0000000000000ul, 0x1F10000000000000ul,
                0x1680000000000000ul, 0x1730000000000000ul, 0x15E0000000000000ul, 0x1450000000000000ul,
                0x1040000000000000ul, 0x11F0000000000000ul, 0x1320000000000000ul, 0x1290000000000000ul,
                0x3600000000000000ul, 0x37B0000000000000ul, 0x3560000000000000ul, 0x34D0000000000000ul,
                0x30C0000000000000ul, 0x3170000000000000ul, 0x33A0000000000000ul, 0x3210000000000000ul,
                0x3B80000000000000ul, 0x3A30000000000000ul, 0x38E0000000000000ul, 0x3950000000000000ul,
                0x3D40000000000000ul, 0x3CF0000000000000ul, 0x3E20000000000000ul, 0x3F90000000000000ul,
                0x2D00000000000000ul, 0x2CB0000000000000ul, 0x2E60000000000000ul, 0x2FD0000000000000ul,
                0x2BC0000000000000ul, 0x2A70000000000000ul, 0x28A0000000000000ul, 0x2910000000000000ul,
                0x2080000000000000ul, 0x2130000000000000ul, 0x23E0000000000000ul, 0x2250000000000000ul,
                0x2640000000000000ul, 0x27F0000000000000ul, 0x2520000000000000ul, 0x2490000000000000ul,
                0x6C00000000000000ul, 0x6DB0000000000000ul, 0x6F60000000000000ul, 0x6ED0000000000000ul,
                0x6AC0000000000000ul, 0x6B70000000000000ul, 0x69A0000000000000ul, 0x6810000000000000ul,
                0x6180000000000000ul, 0x6030000000000000ul, 0x62E0000000000000ul, 0x6350000000000000ul,
                0x6740000000000000ul, 0x66F0000000000000ul, 0x6420000000000000ul, 0x6590000000000000ul,
                0x7700000000000000ul, 0x76B0000000000000ul, 0x7460000000000000ul, 0x75D0000000000000ul,
                0x71C0000000000000ul, 0x7070000000000000ul, 0x72A0000000000000ul, 0x7310000000000000ul,
                0x7A80000000000000ul, 0x7B30000000000000ul, 0x79E0000000000000ul, 0x7850000000000000ul,
                0x7C40000000000000ul, 0x7DF0000000000000ul, 0x7F20000000000000ul, 0x7E90000000000000ul,
                0x5A00000000000000ul, 0x5BB0000000000000ul, 0x5960000000000000ul, 0x58D0000000000000ul,
                0x5CC0000000000000ul, 0x5D70000000000000ul, 0x5FA0000000000000ul, 0x5E10000000000000ul,
                0x5780000000000000ul, 0x5630000000000000ul, 0x54E0000000000000ul, 0x5550000000000000ul,
                0x5140000000000000ul, 0x50F0000000000000ul, 0x5220000000000000ul, 0x5390000000000000ul,
                0x4100000000000000ul, 0x40B0000000000000ul, 0x4260000000000000ul, 0x43D0000000000000ul,
                0x47C0000000000000ul, 0x4670000000000000ul, 0x44A0000000000000ul, 0x4510000000000000ul,
                0x4C80000000000000ul, 0x4D30000000000000ul, 0x4FE0000000000000ul, 0x4E50000000000000ul,
                0x4A40000000000000ul, 0x4BF0000000000000ul, 0x4920000000000000ul, 0x4890000000000000ul,
                0xD800000000000000ul, 0xD9B0000000000000ul, 0xDB60000000000000ul, 0xDAD0000000000000ul,
                0xDEC0000000000000ul, 0xDF70000000000000ul, 0xDDA0000000000000ul, 0xDC10000000000000ul,
                0xD580000000000000ul, 0xD430000000000000ul, 0xD6E0000000000000ul, 0xD750000000000000ul,
                0xD340000000000000ul, 0xD2F0000000000000ul, 0xD020000000000000ul, 0xD190000000000000ul,
                0xC300000000000000ul, 0xC2B0000000000000ul, 0xC060000000000000ul, 0xC1D0000000000000ul,
                0xC5C0000000000000ul, 0xC470000000000000ul, 0xC6A0000000000000ul, 0xC710000000000000ul,
                0xCE80000000000000ul, 0xCF30000000000000ul, 0xCDE0000000000000ul, 0xCC50000000000000ul,
                0xC840000000000000ul, 0xC9F0000000000000ul, 0xCB20000000000000ul, 0xCA90000000000000ul,
                0xEE00000000000000ul, 0xEFB0000000000000ul, 0xED60000000000000ul, 0xECD0000000000000ul,
                0xE8C0000000000000ul, 0xE970000000000000ul, 0xEBA0000000000000ul, 0xEA10000000000000ul,
                0xE380000000000000ul, 0xE230000000000000ul, 0xE0E0000000000000ul, 0xE150000000000000ul,
                0xE540000000000000ul, 0xE4F0000000000000ul, 0xE620000000000000ul, 0xE790000000000000ul,
                0xF500000000000000ul, 0xF4B0000000000000ul, 0xF660000000000000ul, 0xF7D0000000000000ul,
                0xF3C0000000000000ul, 0xF270000000000000ul, 0xF0A0000000000000ul, 0xF110000000000000ul,
                0xF880000000000000ul, 0xF930000000000000ul, 0xFBE0000000000000ul, 0xFA50000000000000ul,
                0xFE40000000000000ul, 0xFFF0000000000000ul, 0xFD20000000000000ul, 0xFC90000000000000ul,
                0xB400000000000000ul, 0xB5B0000000000000ul, 0xB760000000000000ul, 0xB6D0000000000000ul,
                0xB2C0000000000000ul, 0xB370000000000000ul, 0xB1A0000000000000ul, 0xB010000000000000ul,
                0xB980000000000000ul, 0xB830000000000000ul, 0xBAE0000000000000ul, 0xBB50000000000000ul,
                0xBF40000000000000ul, 0xBEF0000000000000ul, 0xBC20000000000000ul, 0xBD90000000000000ul,
                0xAF00000000000000ul, 0xAEB0000000000000ul, 0xAC60000000000000ul, 0xADD0000000000000ul,
                0xA9C0000000000000ul, 0xA870000000000000ul, 0xAAA0000000000000ul, 0xAB10000000000000ul,
                0xA280000000000000ul, 0xA330000000000000ul, 0xA1E0000000000000ul, 0xA050000000000000ul,
                0xA440000000000000ul, 0xA5F0000000000000ul, 0xA720000000000000ul, 0xA690000000000000ul,
                0x8200000000000000ul, 0x83B0000000000000ul, 0x8160000000000000ul, 0x80D0000000000000ul,
                0x84C0000000000000ul, 0x8570000000000000ul, 0x87A0000000000000ul, 0x8610000000000000ul,
                0x8F80000000000000ul, 0x8E30000000000000ul, 0x8CE0000000000000ul, 0x8D50000000000000ul,
                0x8940000000000000ul, 0x88F0000000000000ul, 0x8A20000000000000ul, 0x8B90000000000000ul,
                0x9900000000000000ul, 0x98B0000000000000ul, 0x9A60000000000000ul, 0x9BD0000000000000ul,
                0x9FC0000000000000ul, 0x9E70000000000000ul, 0x9CA0000000000000ul, 0x9D10000000000000ul,
                0x9480000000000000ul, 0x9530000000000000ul, 0x97E0000000000000ul, 0x9650000000000000ul,
                0x9240000000000000ul, 0x93F0000000000000ul, 0x9120000000000000ul, 0x9090000000000000ul,
            };

        private static readonly ulong[] JonesVariantTable =
            {
            0x0000000000000000, 0x7ad870c830358979,
            0xf5b0e190606b12f2, 0x8f689158505e9b8b,
            0xc038e5739841b68f, 0xbae095bba8743ff6,
            0x358804e3f82aa47d, 0x4f50742bc81f2d04,
            0xab28ecb46814fe75, 0xd1f09c7c5821770c,
            0x5e980d24087fec87, 0x24407dec384a65fe,
            0x6b1009c7f05548fa, 0x11c8790fc060c183,
            0x9ea0e857903e5a08, 0xe478989fa00bd371,
            0x7d08ff3b88be6f81, 0x07d08ff3b88be6f8,
            0x88b81eabe8d57d73, 0xf2606e63d8e0f40a,
            0xbd301a4810ffd90e, 0xc7e86a8020ca5077,
            0x4880fbd87094cbfc, 0x32588b1040a14285,
            0xd620138fe0aa91f4, 0xacf86347d09f188d,
            0x2390f21f80c18306, 0x594882d7b0f40a7f,
            0x1618f6fc78eb277b, 0x6cc0863448deae02,
            0xe3a8176c18803589, 0x997067a428b5bcf0,
            0xfa11fe77117cdf02, 0x80c98ebf2149567b,
            0x0fa11fe77117cdf0, 0x75796f2f41224489,
            0x3a291b04893d698d, 0x40f16bccb908e0f4,
            0xcf99fa94e9567b7f, 0xb5418a5cd963f206,
            0x513912c379682177, 0x2be1620b495da80e,
            0xa489f35319033385, 0xde51839b2936bafc,
            0x9101f7b0e12997f8, 0xebd98778d11c1e81,
            0x64b116208142850a, 0x1e6966e8b1770c73,
            0x8719014c99c2b083, 0xfdc17184a9f739fa,
            0x72a9e0dcf9a9a271, 0x08719014c99c2b08,
            0x4721e43f0183060c, 0x3df994f731b68f75,
            0xb29105af61e814fe, 0xc849756751dd9d87,
            0x2c31edf8f1d64ef6, 0x56e99d30c1e3c78f,
            0xd9810c6891bd5c04, 0xa3597ca0a188d57d,
            0xec09088b6997f879, 0x96d1784359a27100,
            0x19b9e91b09fcea8b, 0x636199d339c963f2,
            0xdf7adabd7a6e2d6f, 0xa5a2aa754a5ba416,
            0x2aca3b2d1a053f9d, 0x50124be52a30b6e4,
            0x1f423fcee22f9be0, 0x659a4f06d21a1299,
            0xeaf2de5e82448912, 0x902aae96b271006b,
            0x74523609127ad31a, 0x0e8a46c1224f5a63,
            0x81e2d7997211c1e8, 0xfb3aa75142244891,
            0xb46ad37a8a3b6595, 0xceb2a3b2ba0eecec,
            0x41da32eaea507767, 0x3b024222da65fe1e,
            0xa2722586f2d042ee, 0xd8aa554ec2e5cb97,
            0x57c2c41692bb501c, 0x2d1ab4dea28ed965,
            0x624ac0f56a91f461, 0x1892b03d5aa47d18,
            0x97fa21650afae693, 0xed2251ad3acf6fea,
            0x095ac9329ac4bc9b, 0x7382b9faaaf135e2,
            0xfcea28a2faafae69, 0x8632586aca9a2710,
            0xc9622c4102850a14, 0xb3ba5c8932b0836d,
            0x3cd2cdd162ee18e6, 0x460abd1952db919f,
            0x256b24ca6b12f26d, 0x5fb354025b277b14,
            0xd0dbc55a0b79e09f, 0xaa03b5923b4c69e6,
            0xe553c1b9f35344e2, 0x9f8bb171c366cd9b,
            0x10e3202993385610, 0x6a3b50e1a30ddf69,
            0x8e43c87e03060c18, 0xf49bb8b633338561,
            0x7bf329ee636d1eea, 0x012b592653589793,
            0x4e7b2d0d9b47ba97, 0x34a35dc5ab7233ee,
            0xbbcbcc9dfb2ca865, 0xc113bc55cb19211c,
            0x5863dbf1e3ac9dec, 0x22bbab39d3991495,
            0xadd33a6183c78f1e, 0xd70b4aa9b3f20667,
            0x985b3e827bed2b63, 0xe2834e4a4bd8a21a,
            0x6debdf121b863991, 0x1733afda2bb3b0e8,
            0xf34b37458bb86399, 0x8993478dbb8deae0,
            0x06fbd6d5ebd3716b, 0x7c23a61ddbe6f812,
            0x3373d23613f9d516, 0x49aba2fe23cc5c6f,
            0xc6c333a67392c7e4, 0xbc1b436e43a74e9d,
            0x95ac9329ac4bc9b5, 0xef74e3e19c7e40cc,
            0x601c72b9cc20db47, 0x1ac40271fc15523e,
            0x5594765a340a7f3a, 0x2f4c0692043ff643,
            0xa02497ca54616dc8, 0xdafce7026454e4b1,
            0x3e847f9dc45f37c0, 0x445c0f55f46abeb9,
            0xcb349e0da4342532, 0xb1eceec59401ac4b,
            0xfebc9aee5c1e814f, 0x8464ea266c2b0836,
            0x0b0c7b7e3c7593bd, 0x71d40bb60c401ac4,
            0xe8a46c1224f5a634, 0x927c1cda14c02f4d,
            0x1d148d82449eb4c6, 0x67ccfd4a74ab3dbf,
            0x289c8961bcb410bb, 0x5244f9a98c8199c2,
            0xdd2c68f1dcdf0249, 0xa7f41839ecea8b30,
            0x438c80a64ce15841, 0x3954f06e7cd4d138,
            0xb63c61362c8a4ab3, 0xcce411fe1cbfc3ca,
            0x83b465d5d4a0eece, 0xf96c151de49567b7,
            0x76048445b4cbfc3c, 0x0cdcf48d84fe7545,
            0x6fbd6d5ebd3716b7, 0x15651d968d029fce,
            0x9a0d8ccedd5c0445, 0xe0d5fc06ed698d3c,
            0xaf85882d2576a038, 0xd55df8e515432941,
            0x5a3569bd451db2ca, 0x20ed197575283bb3,
            0xc49581ead523e8c2, 0xbe4df122e51661bb,
            0x3125607ab548fa30, 0x4bfd10b2857d7349,
            0x04ad64994d625e4d, 0x7e7514517d57d734,
            0xf11d85092d094cbf, 0x8bc5f5c11d3cc5c6,
            0x12b5926535897936, 0x686de2ad05bcf04f,
            0xe70573f555e26bc4, 0x9ddd033d65d7e2bd,
            0xd28d7716adc8cfb9, 0xa85507de9dfd46c0,
            0x273d9686cda3dd4b, 0x5de5e64efd965432,
            0xb99d7ed15d9d8743, 0xc3450e196da80e3a,
            0x4c2d9f413df695b1, 0x36f5ef890dc31cc8,
            0x79a59ba2c5dc31cc, 0x037deb6af5e9b8b5,
            0x8c157a32a5b7233e, 0xf6cd0afa9582aa47,
            0x4ad64994d625e4da, 0x300e395ce6106da3,
            0xbf66a804b64ef628, 0xc5bed8cc867b7f51,
            0x8aeeace74e645255, 0xf036dc2f7e51db2c,
            0x7f5e4d772e0f40a7, 0x05863dbf1e3ac9de,
            0xe1fea520be311aaf, 0x9b26d5e88e0493d6,
            0x144e44b0de5a085d, 0x6e963478ee6f8124,
            0x21c640532670ac20, 0x5b1e309b16452559,
            0xd476a1c3461bbed2, 0xaeaed10b762e37ab,
            0x37deb6af5e9b8b5b, 0x4d06c6676eae0222,
            0xc26e573f3ef099a9, 0xb8b627f70ec510d0,
            0xf7e653dcc6da3dd4, 0x8d3e2314f6efb4ad,
            0x0256b24ca6b12f26, 0x788ec2849684a65f,
            0x9cf65a1b368f752e, 0xe62e2ad306bafc57,
            0x6946bb8b56e467dc, 0x139ecb4366d1eea5,
            0x5ccebf68aecec3a1, 0x2616cfa09efb4ad8,
            0xa97e5ef8cea5d153, 0xd3a62e30fe90582a,
            0xb0c7b7e3c7593bd8, 0xca1fc72bf76cb2a1,
            0x45775673a732292a, 0x3faf26bb9707a053,
            0x70ff52905f188d57, 0x0a2722586f2d042e,
            0x854fb3003f739fa5, 0xff97c3c80f4616dc,
            0x1bef5b57af4dc5ad, 0x61372b9f9f784cd4,
            0xee5fbac7cf26d75f, 0x9487ca0fff135e26,
            0xdbd7be24370c7322, 0xa10fceec0739fa5b,
            0x2e675fb4576761d0, 0x54bf2f7c6752e8a9,
            0xcdcf48d84fe75459, 0xb71738107fd2dd20,
            0x387fa9482f8c46ab, 0x42a7d9801fb9cfd2,
            0x0df7adabd7a6e2d6, 0x772fdd63e7936baf,
            0xf8474c3bb7cdf024, 0x829f3cf387f8795d,
            0x66e7a46c27f3aa2c, 0x1c3fd4a417c62355,
            0x935745fc4798b8de, 0xe98f353477ad31a7,
            0xa6df411fbfb21ca3, 0xdc0731d78f8795da,
            0x536fa08fdfd90e51, 0x29b7d047efec8728,
        };
    }
}
