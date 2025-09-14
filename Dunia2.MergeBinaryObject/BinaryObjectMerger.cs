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

                List<string> depth = node.Attribute("depth").Value.Split(":").ToList();

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
                        Console.WriteLine($":Added objects into {obj.Uid}");
                        break;
                    case "edit":
                        EditObject(node, obj);
                        Console.WriteLine($":Edited {obj.Uid}");
                        break;
                    case "delete":
                        parent.Children.Remove(obj);
                        Console.WriteLine($":Deleted {obj.Uid}");
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
                if (node.Attribute("hash") == null && node.Attribute("name") == null)
                    continue;

                uint id;

                if (node.Attribute("name") != null)
                {
                    string name = node.Attribute("name").Value;
                    id = CRC32.Hash(name);
                }
                else
                {
                    string hash = node.Attribute("hash").Value;
                    id = Convert.ToUInt32(hash, 16);
                }

                string value = node.Value;
                byte[] bytes = Convert.FromHexString(value);

                if (obj.Fields.ContainsKey(id))
                {
                    //Delete field if value is empty
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        obj.Fields.Remove(id);
                        continue;
                    }

                    //Replace value in field
                    obj.Fields[id] = bytes;
                } else
                {
                    obj.Fields.Add(id, bytes);
                }
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
            if (depth.Count() < depthCountThreshold)
            {
                return obj;
            }

            //Rid of child we need at the moment then recursively get through until we have our target
            if (!int.TryParse(depth.First(), out int index))
            {
                throw new FormatException("Bad depth value, is it formatted correctly?");
            }
            depth.RemoveAt(0);

            if (moddedUid && index > obj.Children.Count)
            {
                Console.WriteLine("Another mod deleted the object this was trying to edit, cancelling command.");
                return null;
            }

            //If original instead, grab the last child, if it's index is less then it's also a goner.
            if (index > obj.Children.Count && obj.Children.Last().ChildIndex < index)
            {
                Console.WriteLine("Another mod deleted the object this was trying to edit, cancelling command.");
                return null;
            }

            //If the childIndex does not match that means this depth has been modified, find the original childIndex if possible.
            if (!moddedUid && (obj.Children[index].ChildIndex != index))
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
    }
}