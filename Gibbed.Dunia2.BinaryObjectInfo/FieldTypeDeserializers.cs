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

using Gibbed.Dunia2.BinaryObjectInfo.Definitions;
using Gibbed.Dunia2.FileFormats;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gibbed.Dunia2.BinaryObjectInfo
{
    public static class FieldTypeDeserializers
    {
        private static bool HasLeft(byte[] data, int offset, int count, int needCount)
        {
            return data != null &&
                   data.Length >= offset + count &&
                   offset + needCount <= offset + count;
        }

        public static object Deserialize(FieldType fieldType, byte[] data, int offset, int count, out int read)
        {
            if (data.Length == 0)
            {
                read = 0;

                // ToDo: This is not correct for non-numeric types.
                return 0;
            }

            switch (fieldType)
            {
                case FieldType.Boolean:
                {
                    if (HasLeft(data, offset, count, 1) == false)
                    {
                        throw new FormatException("field type Boolean requires 1 byte");
                    }

                    if (data[offset] != 0 &&
                        data[offset] != 1)
                    {
                        throw new FormatException("invalid value for field type Boolean");
                    }

                    read = 1;
                    return data[offset] != 0;
                }

                case FieldType.UInt8:
                {
                    if (HasLeft(data, offset, count, 1) == false)
                    {
                        throw new FormatException("field type UInt8 requires 1 byte");
                    }

                    read = 1;
                    return data[offset];
                }

                case FieldType.Int8:
                {
                    if (HasLeft(data, offset, count, 1) == false)
                    {
                        throw new FormatException("field type Int8 requires 1 byte");
                    }

                    read = 1;
                    return (sbyte)data[offset];
                }

                case FieldType.UInt16:
                {
                    if (HasLeft(data, offset, count, 2) == false)
                    {
                        throw new FormatException("field type UInt16 requires 2 bytes");
                    }

                    read = 2;
                    return BitConverter.ToUInt16(data, offset);
                }

                case FieldType.Int16:
                {
                    if (HasLeft(data, offset, count, 2) == false)
                    {
                        throw new FormatException("field type Int16 requires 2 bytes");
                    }

                    read = 2;
                    return BitConverter.ToInt16(data, offset);
                }

                case FieldType.UInt32:
                {
                    if (HasLeft(data, offset, count, 4) == false)
                    {
                        throw new FormatException("field type UInt32 requires 4 bytes");
                    }

                    read = 4;
                    return BitConverter.ToUInt32(data, offset);
                }

                case FieldType.Int32:
                {
                    if (HasLeft(data, offset, count, 4) == false)
                    {
                        throw new FormatException("field type Int32 requires 4 bytes");
                    }

                    read = 4;
                    return BitConverter.ToInt32(data, offset);
                }

                case FieldType.UInt64:
                {
                    if (HasLeft(data, offset, count, 8) == false)
                    {
                        throw new FormatException("field type UInt64 requires 8 bytes");
                    }

                    read = 8;
                    return BitConverter.ToUInt64(data, offset);
                }

                case FieldType.Int64:
                {
                    if (HasLeft(data, offset, count, 8) == false)
                    {
                        throw new FormatException("field type Int64 requires 8 bytes");
                    }

                    read = 8;
                    return BitConverter.ToInt64(data, offset);
                }

                case FieldType.Float32:
                {
                    if (HasLeft(data, offset, count, 4) == false)
                    {
                        throw new FormatException("field type Float32 requires 4 bytes");
                    }

                    read = 4;
                    return BitConverter.ToSingle(data, offset);
                }

                case FieldType.Float64:
                {
                    if (HasLeft(data, offset, count, 8) == false)
                    {
                        throw new FormatException("field type Float64 requires 8 bytes");
                    }

                    read = 8;
                    return BitConverter.ToDouble(data, offset);
                }

                case FieldType.Vector2:
                {
                    if (HasLeft(data, offset, count, 8) == false)
                    {
                        throw new FormatException("field type Vector2 requires 8 bytes");
                    }

                    read = 8;
                    return new Vector2
                    {
                        X = BitConverter.ToSingle(data, offset + 0),
                        Y = BitConverter.ToSingle(data, offset + 4),
                    };
                }

                case FieldType.Vector3:
                {
                    if (HasLeft(data, offset, count, 12) == false)
                    {
                        throw new FormatException("field type Vector3 requires 12 bytes");
                    }

                    read = 12;
                    return new Vector3
                    {
                        X = BitConverter.ToSingle(data, offset + 0),
                        Y = BitConverter.ToSingle(data, offset + 4),
                        Z = BitConverter.ToSingle(data, offset + 8),
                    };
                }

                case FieldType.Vector4:
                {
                    if (HasLeft(data, offset, count, 16) == false)
                    {
                        throw new FormatException("field type Vector4 requires 16 bytes");
                    }

                    read = 16;
                    return new Vector4
                    {
                        X = BitConverter.ToSingle(data, offset + 0),
                        Y = BitConverter.ToSingle(data, offset + 4),
                        Z = BitConverter.ToSingle(data, offset + 8),
                        W = BitConverter.ToSingle(data, offset + 12),
                    };
                }

                case FieldType.String:
                {
                    if (HasLeft(data, offset, count, 1) == false)
                        throw new FormatException("field type String requires at least 1 byte");

                    int length = 0;

                    while (data[length] != 0)
                        length++;

                    if (data[data.Length - 1] != 0)
                        throw new FormatException("invalid trailing byte value for field type String");

                    read = data.Length;

                    return Encoding.UTF8.GetString(data, offset, length);
                }

                case FieldType.String16:
                {
                    if (HasLeft(data, offset, count, 2) == false)
                        throw new FormatException("field type String16 requires at least 2 bytes.");

                    if (data[data.Length - 1] != 0)
                        throw new FormatException("invalid trailing byte value for field type String16");

                    read = data.Length;

                    return Encoding.Unicode.GetString(data, offset, read - 2);
                }

                case FieldType.Enum:
                {
                    if (HasLeft(data, offset, count, 4) == false)
                    {
                        throw new FormatException("field type Enum requires 4 bytes");
                    }

                    read = 4;
                    return BitConverter.ToInt32(data, offset);
                }

                case FieldType.Hash32:
                {
                    if (HasLeft(data, offset, count, 4) == false)
                    {
                        throw new FormatException("field type Hash32 requires 4 bytes");
                    }

                    read = 4;

                    return BitConverter.ToUInt32(data, offset);
                }

                case FieldType.Hash64:
                {
                    if (HasLeft(data, offset, count, 8) == false)
                    {
                        throw new FormatException("field type Hash64 requires 8 bytes");
                    }

                    read = 8;
                    return BitConverter.ToUInt64(data, offset);
                }

                case FieldType.Id32:
                {
                    if (HasLeft(data, offset, count, 4) == false)
                    {
                        throw new FormatException("field type Id32 requires 4 bytes");
                    }

                    read = 4;
                    return BitConverter.ToUInt32(data, offset);
                }

                case FieldType.Id64:
                {
                    if (HasLeft(data, offset, count, 8) == false)
                    {
                        throw new FormatException("field type Id64 requires 8 bytes");
                    }

                    read = 8;

                    return BitConverter.ToUInt64(data, offset);
                }

                case FieldType.Vector8:
                    {
                        if (HasLeft(data, offset, count, 32) == false)
                        {
                            throw new FormatException("field type Vector4 requires 32 bytes");
                        }

                        read = 32;
                        return new Vector8
                        {
                            min = new Vector4
                            {
                                X = BitConverter.ToSingle(data, offset + 0),
                                Y = BitConverter.ToSingle(data, offset + 4),
                                Z = BitConverter.ToSingle(data, offset + 8),
                                W = BitConverter.ToSingle(data, offset + 12)
                            },

                            max = new Vector4
                            {
                                X = BitConverter.ToSingle(data, offset + 16),
                                Y = BitConverter.ToSingle(data, offset + 20),
                                Z = BitConverter.ToSingle(data, offset + 24),
                                W = BitConverter.ToSingle(data, offset + 28)
                            }

                        };
                    }
                case FieldType.Matrix4:
                    {
                        if (HasLeft(data, offset, count, 64) == false)
                        {
                            throw new FormatException("field type Vector4 requires 64 bytes");
                        }

                        read = 64;
                        return new Matrix4
                        {
                            MatrixTop = new Vector4
                            {
                                X = BitConverter.ToSingle(data, offset + 0),
                                Y = BitConverter.ToSingle(data, offset + 4),
                                Z = BitConverter.ToSingle(data, offset + 8),
                                W = BitConverter.ToSingle(data, offset + 12)
                            },

                            MatrixMiddle = new Vector4
                            {
                                X = BitConverter.ToSingle(data, offset + 16),
                                Y = BitConverter.ToSingle(data, offset + 20),
                                Z = BitConverter.ToSingle(data, offset + 24),
                                W = BitConverter.ToSingle(data, offset + 28)
                            },

                            MatrixBottom = new Vector4
                            {
                                X = BitConverter.ToSingle(data, offset + 32),
                                Y = BitConverter.ToSingle(data, offset + 36),
                                Z = BitConverter.ToSingle(data, offset + 40),
                                W = BitConverter.ToSingle(data, offset + 44)
                            },

                            Position = new Vector4
                            {
                                X = BitConverter.ToSingle(data, offset + 48),
                                Y = BitConverter.ToSingle(data, offset + 52),
                                Z = BitConverter.ToSingle(data, offset + 56),
                                W = BitConverter.ToSingle(data, offset + 60)
                            }

                        };
                    }

                default:
                {
                    throw new NotSupportedException("unsupported field type");
                }
            }
        }

        public static TType Deserialize<TType>(FieldType fieldType, byte[] data)
        {
            int read;
            var value = (TType)Deserialize(fieldType, data, 0, data.Length, out read);
            if (read != data.Length)
            {
                throw new FormatException();
            }
            return value;
        }

        public static TType Deserialize<TType>(FieldType fieldType, byte[] data, int offset, int count, out int read)
        {
            return (TType)Deserialize(fieldType, data, offset, count, out read);
        }

        public static string DeserializeToString(FieldType fieldType, byte[] data, int offset, int count, out int read)
        {
            switch (fieldType)
            {
                case FieldType.Boolean:
                    {
                        var value = Deserialize<bool>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.UInt8:
                    {
                        var value = Deserialize<byte>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.Int8:
                    {
                        var value = Deserialize<sbyte>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.UInt16:
                    {
                        var value = Deserialize<ushort>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.Int16:
                    {
                        var value = Deserialize<short>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.UInt32:
                    {
                        var value = Deserialize<uint>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.Int32:
                    {
                        var value = Deserialize<int>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.UInt64:
                    {
                        var value = Deserialize<ulong>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.Int64:
                    {
                        var value = Deserialize<long>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.Float32:
                    {
                        var value = Deserialize<float>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.Float64:
                    {
                        var value = Deserialize<double>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.Vector2:
                    {
                        var value = Deserialize<Vector2>(fieldType, data, 0, count, out read);
                        return string.Format("{0},{1}",
                                                         value.X.ToString(CultureInfo.InvariantCulture),
                                                         value.Y.ToString(CultureInfo.InvariantCulture));
                    }

                case FieldType.Vector3:
                    {
                        var value = Deserialize<Vector3>(fieldType, data, 0, count, out read);
                        return string.Format("{0},{1},{2}",
                                                         value.X.ToString(CultureInfo.InvariantCulture),
                                                         value.Y.ToString(CultureInfo.InvariantCulture),
                                                         value.Z.ToString(CultureInfo.InvariantCulture));
                    }

                case FieldType.Vector4:
                    {
                        var value = Deserialize<Vector4>(fieldType, data, 0, count, out read);
                        return string.Format("{0},{1},{2},{3}",
                                                         value.X.ToString(CultureInfo.InvariantCulture),
                                                         value.Y.ToString(CultureInfo.InvariantCulture),
                                                         value.Z.ToString(CultureInfo.InvariantCulture),
                                                         value.W.ToString(CultureInfo.InvariantCulture));
                    }

                case FieldType.String16:
                case FieldType.String:
                    {
                        var value = Deserialize<string>(fieldType, data, 0, count, out read);
                        return value;
                    }

                case FieldType.Hash32:
                    {
                        var value = Deserialize<uint>(fieldType, data, 0, count, out read);
                        return value.ToString("X8", CultureInfo.InvariantCulture);
                    }

                case FieldType.Hash64:
                    {
                        var value = Deserialize<ulong>(fieldType, data, 0, count, out read);
                        return value.ToString("X16", CultureInfo.InvariantCulture);
                    }

                case FieldType.Id32:
                    {
                        var value = Deserialize<uint>(fieldType, data, 0, count, out read);
                        return value.ToString(CultureInfo.InvariantCulture);
                    }

                case FieldType.Id64:
                    {
                        string strValue = "-1";
                        var value = Deserialize<ulong>(fieldType, data, 0, count, out read);

                        if (value != ulong.MaxValue)
                            strValue = value.ToString(CultureInfo.InvariantCulture);

                        return strValue;
                    }

                case FieldType.Vector8:
                    {
                        var value = Deserialize<Vector8>(fieldType, data, 0, count, out read);
                        return string.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                                 value.min.X.ToString(CultureInfo.InvariantCulture),
                                 value.min.Y.ToString(CultureInfo.InvariantCulture),
                                 value.min.Z.ToString(CultureInfo.InvariantCulture),
                                 value.min.W.ToString(CultureInfo.InvariantCulture),
                                 value.max.X.ToString(CultureInfo.InvariantCulture),
                                 value.max.Y.ToString(CultureInfo.InvariantCulture),
                                 value.max.Z.ToString(CultureInfo.InvariantCulture),
                                 value.max.W.ToString(CultureInfo.InvariantCulture));
                    }
                case FieldType.Matrix4:
                    {
                        var value = Deserialize<Matrix4>(fieldType, data, 0, count, out read);
                        return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                                 value.MatrixTop.X.ToString(CultureInfo.InvariantCulture),
                                 value.MatrixTop.Y.ToString(CultureInfo.InvariantCulture),
                                 value.MatrixTop.Z.ToString(CultureInfo.InvariantCulture),
                                 value.MatrixTop.W.ToString(CultureInfo.InvariantCulture),
                                 value.MatrixMiddle.X.ToString(CultureInfo.InvariantCulture),
                                 value.MatrixMiddle.Y.ToString(CultureInfo.InvariantCulture),
                                 value.MatrixMiddle.Z.ToString(CultureInfo.InvariantCulture),
                                 value.MatrixMiddle.W.ToString(CultureInfo.InvariantCulture),
                                 value.MatrixBottom.X.ToString(CultureInfo.InvariantCulture),
                                 value.MatrixBottom.Y.ToString(CultureInfo.InvariantCulture),
                                 value.MatrixBottom.Z.ToString(CultureInfo.InvariantCulture),
                                 value.MatrixBottom.W.ToString(CultureInfo.InvariantCulture),
                                 value.Position.X.ToString(CultureInfo.InvariantCulture),
                                 value.Position.Y.ToString(CultureInfo.InvariantCulture),
                                 value.Position.Z.ToString(CultureInfo.InvariantCulture),
                                 value.Position.W.ToString(CultureInfo.InvariantCulture));
                    }

                default:
                    {
                        throw new NotSupportedException("unsupported field type");
                    }
            }
        }

        private static void Deserialize(XmlWriter writer,
                                        FieldType fieldType,
                                        byte[] data,
                                        int offset,
                                        int count,
                                        out int read)
        {
            writer.WriteString(DeserializeToString(fieldType, data, offset, count, out read));
        }

        public static void Deserialize(XmlWriter writer, FieldType type, byte[] data)
        {
            FieldDefinition fieldDefinition = new FieldDefinition
            {
                Type = type
            };

            Deserialize(writer, fieldDefinition, data);
        }

        public static void Deserialize(XmlWriter writer,
                                       FieldDefinition fieldDef,
                                       byte[] data)
        {
            int read;

            switch (fieldDef.Type)
            {
                case FieldType.BinHex:
                {
                    writer.WriteBinHex(data, 0, data.Length);
                    read = data.Length;
                    break;
                }

                case FieldType.Boolean:
                case FieldType.UInt8:
                case FieldType.Int8:
                case FieldType.UInt16:
                case FieldType.Int16:
                case FieldType.UInt32:
                case FieldType.Int32:
                case FieldType.UInt64:
                case FieldType.Int64:
                case FieldType.Float32:
                case FieldType.Float64:
                case FieldType.Vector2:
                case FieldType.Vector3:
                case FieldType.Vector4:
                case FieldType.String:
                case FieldType.String16:
                case FieldType.Hash32:
                case FieldType.Hash64:
                case FieldType.Id32:
                case FieldType.Id64:
                case FieldType.Vector8:
                case FieldType.Matrix4:
                    {
                        Deserialize(writer, fieldDef.Type, data, 0, data.Length, out read);
                        break;
                    }

                case FieldType.Enum:
                {
                    var value = Deserialize<int>(fieldDef.Type, data, 0, data.Length, out read);

                    if (fieldDef.Enum != null)
                    {
                        var enumDef = fieldDef.Enum.Elements.FirstOrDefault(ed => ed.Value == value);
                        if (enumDef != null)
                        {
                            writer.WriteString(enumDef.Name);
                            break;
                        }
                    }

                    writer.WriteString(value.ToString(CultureInfo.InvariantCulture));
                    break;
                }

                case FieldType.Array32:
                {
                    if (HasLeft(data, 0, data.Length, 4) == false)
                    {
                        throw new FormatException("field type Array32 requires at least 4 bytes");
                    }

                    var itemCount = BitConverter.ToUInt32(data, 0);

                    read = 4;
                    int offset = 4;
                    int remaining = data.Length - offset;
                    for (uint i = 0; i < itemCount; i++)
                    {
                        writer.WriteStartElement("item");
                        int itemRead;
                        Deserialize(writer, fieldDef.ArrayType, data, offset, remaining, out itemRead);
                        offset += itemRead;
                        remaining -= itemRead;
                        writer.WriteEndElement();

                        read += itemRead;
                    }

                    break;
                }

                default:
                {
                    throw new NotSupportedException("unsupported field type");
                }
            }

            if (read != data.Length)
            {
                if (string.IsNullOrEmpty(fieldDef.Name) == false)
                {
                    throw new FormatException(
                        string.Format("did not consume all data for field '{0}' (read {1}, total {2})",
                                      fieldDef.Name,
                                      read,
                                      data.Length));
                }

                throw new FormatException(
                    string.Format("did not consume all data for field 0x{0:X8}  (read {1}, total {2})",
                                  fieldDef.Hash,
                                  read,
                                  data.Length));
            }
        }
    }
}
