using Gibbed.Dunia2.BinaryObjectInfo;
using Gibbed.Dunia2.FileFormats;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Dunia2.MergeBinaryObject
{
    public class BinaryObjectMerger
    {
        public BinaryObject Merge(BinaryObject root, XDocument file)
        {
            foreach (XElement node in file.Root.Elements())
            {
                bool checkForModdedUids = false;

                if (node.NodeType == System.Xml.XmlNodeType.Comment)
                    continue;

                if (node.Attribute("modded") != null && node.Attribute("modded").Value == "1")
                {
                    checkForModdedUids = true;
                }

                if (node.Attribute("depth") == null)
                {
                    Console.WriteLine("Depth not found, skipping command.");
                    continue;
                }

                string fullDepth = node.Attribute("depth").Value;
                List<string> depth = fullDepth.Split(":").ToList();

                if (depth.Count == 0)
                {
                    Console.WriteLine("Depth is empty, skipping command.");
                    continue;
                }

                //Modder should not be allowed to delete the root of a file
                if (depth[0].Equals("root") && node.Name.LocalName.Equals("delete"))
                {
                    Console.WriteLine("Deleting the entire file is not allowed ;)");
                    continue;
                }

                Console.Write($"Attempting to modify {fullDepth}... ");

                //Find parent BinaryObject (used for deleting objects)
                BinaryObject parent = Traverse(root, depth, checkForModdedUids);
                BinaryObject obj = parent;

                //Remove all but the last one so we have a faster traverse
                depth.RemoveRange(0, depth.Count - 1);

                if (!depth[0].Equals("root"))
                    obj = Traverse(parent, depth, checkForModdedUids, 1);

                if (obj == null)
                {
                    Console.WriteLine("Couldn't find BinaryObject, skipping command.");
                    continue;
                }

                switch (node.Name.LocalName)
                {
                    case "add":
                        AddObject("", node.Elements(), obj);
                        Console.WriteLine("Success");
                        break;
                    case "edit":
                        EditObject(node, obj);
                        Console.WriteLine("Success");
                        break;
                    case "editall":
                        ScanAllFields(node, obj);
                        Console.WriteLine("Success");
                        break;
                    case "delete":
                        parent.Children.Remove(obj);
                        Console.WriteLine("Success");
                        break;
                }
            }
            return root;
        }

        private void AddObject(string fileName, IEnumerable<XElement> xmlData, BinaryObject obj)
        {
            string baseName = Path.GetFileNameWithoutExtension(fileName);
            var importing = new Importing(InfoManager.Load(Gibbed.ProjectData.Manager.Load().ActiveProject.ListsPath));
            foreach (XElement element in xmlData)
            {
                BinaryObject newObject = importing.Import(null, baseName, element.CreateNavigator());
                int childPos = (obj.Children.LastOrDefault()?.ChildIndex ?? 0) + 1;
                InsertUids(newObject, obj.Uid, childPos);
                obj.Children.Add(newObject);
            }
        }

        private void EditObject(XElement xmlData, BinaryObject obj)
        {
            foreach (XElement node in xmlData.Elements())
            {
                EditField(node, obj);
            }
        }

        private void EditField(XElement node, BinaryObject obj)
        {
            if (node.Attribute("hash") == null && node.Attribute("name") == null)
                return;

            uint id = GetFieldHash(node);

            string value = node.Value;
            FieldType type = (FieldType)Enum.Parse(typeof(FieldType), node.Attribute("type").Value, true);
            byte[] bytes = FieldTypeSerializers.Serialize(type, node.Value);

            if (!obj.Fields.TryAdd(id, bytes))
            {
                //Delete field if value is empty
                if (string.IsNullOrWhiteSpace(value))
                {
                    obj.Fields.Remove(id);
                    return;
                }

                //Replace value in field
                obj.Fields[id] = bytes;
            }
        }

        //When it comes to editing all fields only fields that exist should work.
        //We do not want a new field for every object if someone sets the depth to root.
        private void ScanAllFields(XElement xmlData, BinaryObject obj)
        {
            foreach (XElement node in xmlData.Elements())
            {
                uint key = GetFieldHash(node);
                if (!obj.Fields.ContainsKey(key))
                    continue;

                EditField(node, obj);
            }

            foreach (BinaryObject child in obj.Children)
            {
                ScanAllFields(xmlData, child);
            }
        }

        private void InsertUids(BinaryObject obj, string origUid, int index)
        {
            origUid += $":{index}";
            obj.Uid = origUid;
            //New objects should not be given Child Indexes as these are designed to find original depth values.
            obj.ChildIndex = -1;
            for (int i = 0; i < obj.Children.Count; i++)
            {
                InsertUids(obj.Children[0], origUid, i);
            }
        }

        private BinaryObject Traverse(BinaryObject obj, List<string> depth, bool moddedUid, int depthCountThreshold = 2)
        {
            if (depth.Count < depthCountThreshold)
            {
                return obj;
            }

            //Rid of child we need at the moment then recursively get through until we have our target
            if (!int.TryParse(depth.First(), out int index))
            {
                throw new FormatException("Bad depth value, is it formatted correctly?");
            }
            depth.RemoveAt(0);

            if (obj == null || (moddedUid && index >= obj.Children.Count))
            {
                Console.WriteLine("Another mod deleted the object this was trying to edit, cancelling command.");
                return null;
            }

            //If original instead, grab the last child, if it's index is less then it's also a goner.
            if (index >= obj.Children.Count && obj.Children.Last().ChildIndex < index)
            {
                Console.WriteLine("Another mod deleted the object this was trying to edit, cancelling command.");
                return null;
            }

            //If the childIndex does not match that means this depth has been modified, find the original childIndex if possible.
            if (!moddedUid && (index >= obj.Children.Count || obj.Children[index].ChildIndex != index))
            {
                BinaryObject foundChild = obj.Children.FirstOrDefault(child => child.ChildIndex == index);
                if (foundChild == null)
                {
                    Console.WriteLine("Couldn't find depth, cancelling command.");
                    return null;
                }
                index = obj.Children.IndexOf(foundChild);
            }
            return Traverse(obj.Children[index], depth, moddedUid);
        }

        private uint GetFieldHash(XElement element)
        {
            string value;

            if (element.Attribute("hash") != null)
            {
                value = element.Attribute("hash").Value;
                return Convert.ToUInt32(value, 16);
            }

            if (element.Attribute("name") != null)
            {
                value = element.Attribute("name").Value;
                return CRC32.Hash(value);
            }

            return 0;
        }
    }
}