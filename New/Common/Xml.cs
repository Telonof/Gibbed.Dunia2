using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Common.Extensions;

namespace Common
{
    [XmlRoot(ElementName = "object")]
    public class FcObject
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "hash")]
        public string Hash { get; set; }
        
        [XmlElement(ElementName = "field")]
        public List<FcField> Fields { get; set; }

        [XmlElement(ElementName = "object")]
        public List<FcObject> Objects { get; set; }
        
        [XmlIgnore]
        public FcObject Parent { get; set; }

        public void Serialize(string fileName)
        {
            var serializer = new XmlSerializer(typeof(FcObject));

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(true),
                NewLineChars = Environment.NewLine,
                ConformanceLevel = ConformanceLevel.Document,
                Indent = true
            };

            using (var fs = new FileStream(fileName, FileMode.Create))
            using (var writer = XmlWriter.Create(fs, settings))
            {
                serializer.Serialize(writer, this, ns);
            }
        }
        public FcObject Clone()
        {
            var serializer = new XmlSerializer(typeof(FcObject));

            var ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(true),
                NewLineChars = Environment.NewLine,
                ConformanceLevel = ConformanceLevel.Document,
                Indent = true
            };

            using (var fs = new MemoryStream())
            using (var writer = XmlWriter.Create(fs, settings))
            {
                serializer.Serialize(writer, this, ns);

                fs.Position = 0;

                return Deserialize(fs);
            }
        }

        public void SetParent(FcObject parent)
        {
            Parent = parent;
        }
        public void SetParents()
        {
            foreach (var obj in new[] { this }.Flatten(a => a.Objects))
            {
                foreach (var obj2 in obj.Objects)
                {
                    obj2.SetParent(obj);
                }
            }
        }
        
        public FcObject[] AllObjects()
        {
            return Objects.Flatten(a => a.Objects).ToArray();
        }
        public FcObject[] AllObjectsIncludingSelf()
        {
            return new[] { this }.Flatten(a => a.Objects).ToArray();
        }
        public FcField[] AllFieldsIncludingSelf()
        {
            return AllObjectsIncludingSelf().SelectMany(a => a.Fields).ToArray();
        }
        
        public static FcObject Deserialize(string fileName)
        {
            var serializer = new XmlSerializer(typeof(FcObject));
            
            using (var reader = File.OpenText(fileName))
            {
                return serializer.Deserialize(reader) as FcObject;
            }
        }
        public static FcObject Deserialize(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(FcObject));

            return serializer.Deserialize(stream) as FcObject;
        }
    }

    [XmlRoot(ElementName = "field")]
    public class FcField
    {
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "hash")]
        public string Hash { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        
        [XmlText]
        public string Text { get; set; }
    }
}
