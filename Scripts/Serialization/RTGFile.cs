using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class RTGFile : List<RTGFile.RTGElement>
{
    public class RTGElement
    {
        public string Name;
        public List<ParentInfo> ParentInfo;
        public Dictionary<string, object> Properties;

        public RTGElement(string name, List<ParentInfo> parent, Dictionary<string, object> properties)
        {
            Name = name;
            ParentInfo = parent;
            Properties = properties;
        }

        public RTGElement(GrambyObject grambyObject, int? parentIdx)
        {
            Name = grambyObject.ClassName();
            if (parentIdx != null)
            {
                var parent = grambyObject.GetParent<Attachment>();
                var attachment = grambyObject.GetNode(grambyObject.DefaultAttachment);
                ParentInfo = new List<ParentInfo> { new ParentInfo(attachment.Name, parent.Name, parentIdx.Value) }; // TODO: multiple parents
            }
            Properties = grambyObject.Properties;
        }

        public class RTGElementConverter : JsonConverter<RTGElement>
        {
            public override RTGElement ReadJson(JsonReader reader, Type objectType, RTGElement existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var array = JArray.Load(reader);
                var name = (string)array[0];
                var parentInfos = array[1].ToObject<List<ParentInfo>>();
                Dictionary<string, object> properties;
                try
                {
                    properties = array[2].ToObject<Dictionary<string, object>>();
                }
                catch
                {
                    properties = null;
                }
                return new RTGElement(name, parentInfos, properties);
            }

            public override void WriteJson(JsonWriter writer, RTGElement value, JsonSerializer serializer)
            {
                writer.WriteStartArray();
                writer.WriteValue(value.Name);
                if (value.ParentInfo != null)
                {
                    serializer.Serialize(writer, value.ParentInfo);
                }
                else
                {
                    writer.WriteStartArray();
                    writer.WriteEndArray();
                }
                if (value.Properties != null)
                {
                    writer.WriteValue(value.Properties);
                }
                else
                {
                    writer.WriteStartArray();
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
            }
        }
    }

    [JsonConverter(typeof(ParentInfoConverter))]
    public class ParentInfo
    {
        public string RootAttachment;
        public string ParentAttachment;
        public int ParentIndex;

        public ParentInfo(string rootAttachment, string parentAttachment, int parentIndex)
        {
            RootAttachment = rootAttachment;
            ParentAttachment = parentAttachment;
            ParentIndex = parentIndex;
        }

        public class ParentInfoConverter : JsonConverter<ParentInfo>
        {
            public override ParentInfo ReadJson(JsonReader reader, Type objectType, ParentInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var arr = JArray.Load(reader);
                return new ParentInfo(((int)arr[0]).ToString(), ((int)arr[1]).ToString(), (int)arr[2]);
            }

            public override void WriteJson(JsonWriter writer, ParentInfo value, JsonSerializer serializer)
            {
                writer.WriteStartArray();
                writer.WriteValue(value.RootAttachment.ToInt());
                writer.WriteValue(value.ParentAttachment.ToInt());
                writer.WriteValue(value.ParentIndex);
                writer.WriteEndArray();
            }
        }
    }

    public RTGFile(GrambyObject root)
    {
        AddNodeRecursively(root, null);
    }

    private void AddNodeRecursively(GrambyObject node, int? parentIdx)
    {
        Add(new RTGElement(node, parentIdx));
        var elementIdx = Count; // Lua arrays are 1-indexed, so this works out to the index of the object we just added
        foreach (var pair in node.Children())
        {
            var child = pair.Value;
            if (child != null)
            {
                AddNodeRecursively(pair.Value, elementIdx);
            }
        }
    }
}
