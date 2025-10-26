using Gibbed.Dunia2.BinaryObjectInfo;
using Gibbed.Dunia2.FileFormats;
using System.Xml.Linq;

namespace Dunia2.MergeBinaryObject
{
    public class BabelDBMerger
    {
        public BabelDBFile Merge(BabelDBFile original, XDocument file)
        {
            foreach (XElement node in file.Root.Elements("row"))
            {
                List<byte[]> data = [];
                foreach (XElement node2 in node.Elements())
                {
                    byte[] columnData;
                    if (node2.Attribute("type") == null)
                    {
                        columnData = Convert.FromHexString(node2.Value);
                    }
                    else
                    {
                        FieldType type = (FieldType)Enum.Parse(typeof(FieldType), node2.Attribute("type").Value, true);
                        columnData = FieldTypeSerializers.Serialize(type, node2.Value);
                    }

                    data.Add(columnData);
                }
                original.Rows.Add(new Row { data = data });
            }

            return original;
        }
    }
}
