using Gibbed.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gibbed.Dunia2.FileFormats
{
    public class BabelDBFile
    {
        private const uint Magic = 0xDB240200;

        private uint Version;

        private uint Subversion;

        private string Name;

        public readonly List<Column> Columns = [];

        public readonly List<Row> Rows = [];

        //How many bytes does strings reserve, 32 if in TC1, 64 in TC2.
        private int StringAlloc = 32;

        public void Deseralize(Stream input)
        {
            var magic = input.ReadValueU32(Endian.Little);

            if (magic != Magic)
            {
                throw new FormatException("Bad magic.");
            }

            Version = input.ReadValueU32(Endian.Big);
            Subversion = input.ReadValueU32(Endian.Big);
            uint columnCount = input.ReadValueU32(Endian.Big);
            uint rowCount = input.ReadValueU32(Endian.Big);
            uint columnDefSize = input.ReadValueU32(Endian.Big);
            input.Seek(8, SeekOrigin.Current);

            //Get if strings use 32 bytes or 64
            if (((double)columnDefSize / 44) != columnCount)
                StringAlloc = 64;

            Name = GrabString(input);

            for (int i = 0; i < columnCount; i++)
            {
                Columns.Add(new Column
                {
                    DataType = input.ReadValueU32(Endian.Big),
                    ByteUsage = input.ReadValueU32(Endian.Big),
                    PK = input.ReadValueU32(Endian.Big) == 1,
                    Name = GrabString(input)
                });
            }

            for (int i = 0; i < rowCount; i++)
            {
                List<byte[]> data = [];
                for (int j = 0; j < columnCount; j++)
                {
                    data.Add(input.ReadBytes(Columns[0].ByteUsage * 2));
                    Array.Reverse(data[data.Count - 1]);
                }

                Rows.Add(new Row { data = data });
            }
        }

        public void Serialize(Stream output)
        {
            //Seralize data section first to get total byte count.
            List<byte> dataSection = [];
            foreach (Row row in Rows)
            {
                if (row.data.Count != Columns.Count)
                {
                    throw new FormatException("Too little/many data segements in row.");
                }

                for (int i = 0; i < row.data.Count; i++)
                {
                    int dataLength = (int)Columns[i].ByteUsage * 2;
                    byte[] data = row.data[i];
                    byte[] trimmedData = new byte[dataLength];

                    //trim or pad to match 
                    if (data.Length > dataLength)
                    {
                        Buffer.BlockCopy(data, 0, trimmedData, 0, dataLength);
                    }
                    if (data.Length < dataLength)
                    {
                        Buffer.BlockCopy(data, 0, trimmedData, 0, data.Length);
                    }
                    else
                    {
                        trimmedData = data;
                    }

                    Array.Reverse(trimmedData);
                    dataSection.AddRange(trimmedData);
                }
            }

            int columnDefSize = Columns.Count * (StringAlloc + 12);

            output.WriteValueU32(Magic, Endian.Little);
            output.WriteValueU32(Version, Endian.Big);
            output.WriteValueU32(Subversion, Endian.Big);
            output.WriteValueU32((uint)Columns.Count, Endian.Big);
            output.WriteValueU32((uint)Rows.Count, Endian.Big);
            output.WriteValueU32((uint)columnDefSize, Endian.Big);
            output.WriteValueU32((uint)dataSection.Count, Endian.Big);
            output.WriteValueU32(0x00);

            //Name
            output.WriteString(Name);
            byte[] padding = new byte[StringAlloc - Name.Length];
            output.Write(padding, 0, padding.Length);

            foreach (Column column in Columns)
            {
                output.WriteValueU32(column.DataType, Endian.Big);
                output.WriteValueU32(column.ByteUsage, Endian.Big);
                output.WriteValueU32(column.PK ? (uint)1 : 0, Endian.Big);
                output.WriteString(column.Name);
                padding = new byte[StringAlloc - column.Name.Length];
                output.Write(padding, 0, padding.Length);
            }

            output.WriteBytes(dataSection.ToArray());
        }

        public int GrabColumnIndex(string name)
        {
            Column column = Columns.Where(col => col.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (column == null)
                return -1;

            return Columns.IndexOf(column);
        }

        private string GrabString(Stream input)
        {
            byte[] name = input.ReadBytes(StringAlloc);

            int index = Array.IndexOf(name, (byte)0);

            return Encoding.ASCII.GetString(name, 0, index);
        }
    }

    public record Column
    {
        public required uint DataType;

        //This is practically 08 always
        public required uint ByteUsage;

        public required bool PK;

        public required string Name;
    }

    public record Row
    {
        public required List<byte[]> data = [];
    }
}
