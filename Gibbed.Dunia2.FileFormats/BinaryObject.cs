﻿/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
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
using System.Collections.Generic;
using System.IO;
using Gibbed.IO;

namespace Gibbed.Dunia2.FileFormats
{
    public class BinaryObject
    {
        private long _Position;
        private uint _NameHash;
        private string _Uid;
        private int _ChildIndex = 0;
        private readonly Dictionary<uint, byte[]> _Fields = new Dictionary<uint, byte[]>();
        private readonly List<BinaryObject> _Children = new List<BinaryObject>();

        public long Position
        {
            get { return this._Position; }
            set { this._Position = value; }
        }

        public uint NameHash
        {
            get { return this._NameHash; }
            set { this._NameHash = value; }
        }

        public Dictionary<uint, byte[]> Fields
        {
            get { return this._Fields; }
        }

        public List<BinaryObject> Children
        {
            get { return this._Children; }
        }

        public string Uid
        {
            get { return this._Uid; }
            set { this._Uid = value; }
        }

        public int ChildIndex
        {
            get { return this._ChildIndex; }
            set { this._ChildIndex = value; }
        }

        public void Serialize(Stream output,
                              ref uint totalObjectCount,
                              ref uint totalValueCount,
                              Endian endian)
        {
            totalObjectCount += (uint)this.Children.Count;
            totalValueCount += (uint)this.Fields.Count;

            output.WriteCount(this.Children.Count, false, endian);

            output.WriteValueU32(this.NameHash, endian);

            output.WriteCount(this.Fields.Count, false, endian);
            foreach (var kv in this.Fields)
            {
                output.WriteValueU32(kv.Key, endian);
                output.WriteCount(kv.Value.Length, false, endian);
                output.WriteBytes(kv.Value);
            }

            foreach (var child in this.Children)
            {
                child.Serialize(output,
                                ref totalObjectCount,
                                ref totalValueCount,
                                endian);
            }
        }

        public static BinaryObject Deserialize(BinaryObject parent,
                                               Stream input,
                                               List<BinaryObject> pointers,
                                               Endian endian, string depth = "0",
                                               int index = 0)
        {
            long position = input.Position;

            bool isOffset;
            var childCount = input.ReadCount(out isOffset, endian);

            if (isOffset == true)
            {
                //Doing it the new way is way more inefficient, but actually has each object it's own entity which is useful when editing the BinaryObject directly.
                //Tests show it takes from 0-1 seconds longer to run depending on the filesize. 
                //return pointers[(int)childCount];
                return CloneObject(pointers[(int)childCount], depth, index);
            }

            var child = new BinaryObject();
            child.Position = position;

            child.Uid = depth;
            child.ChildIndex = index;

            pointers.Add(child);

            child.Deserialize(input, childCount, pointers, endian, depth);
            return child;
        }

        private void Deserialize(Stream input,
                                 uint childCount,
                                 List<BinaryObject> pointers,
                                 Endian endian, string depth)
        {
            bool isOffset;

            this.NameHash = input.ReadValueU32(endian);

            var valueCount = input.ReadCount(out isOffset, endian);
            if (isOffset == true)
            {
                throw new NotImplementedException();
            }

            this.Fields.Clear();
            for (var i = 0; i < valueCount; i++)
            {
                var nameHash = input.ReadValueU32(endian);
                byte[] value;

                var position = input.Position;
                var size = input.ReadCount(out isOffset, endian);
                if (isOffset == true)
                {
                    input.Seek(position - size, SeekOrigin.Begin);

                    size = input.ReadCount(out isOffset, endian);
                    if (isOffset == true)
                    {
                        throw new FormatException();
                    }

                    value = input.ReadBytes(size);

                    input.Seek(position, SeekOrigin.Begin);
                    input.ReadCount(out isOffset, endian);
                }
                else
                {
                    value = input.ReadBytes(size);
                }

                this.Fields.Add(nameHash, value);
            }

            this.Children.Clear();
            for (var i = 0; i < childCount; i++)
            {
                this.Children.Add(Deserialize(this, input, pointers, endian, depth + $":{i}", i));
            }
        }

        private static BinaryObject CloneObject(BinaryObject obj, string depth, int index)
        {
            BinaryObject newObj = new BinaryObject();
            newObj.ChildIndex = index;
            newObj.Uid = depth;
            newObj.NameHash = obj.NameHash;
            newObj.Position = obj.Position;
            
            foreach (uint key in obj.Fields.Keys)
            {
                newObj.Fields.Add(key, obj.Fields[key]);
            }

            for (int i = 0; i < obj.Children.Count; i++)
            {
                newObj.Children.Add(CloneObject(obj.Children[i], depth + $":{i}", i));
            }
            return newObj;
        }
    }
}
