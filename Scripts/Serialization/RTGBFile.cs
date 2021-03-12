using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

public class RTGBFile
{
    [JsonProperty]
    public static string Version = "0.1.0";

    public class GrambyObjectConverter : JsonConverter<GrambyObject>
    {
        public GrambyObject ConvertJObject(JObject jObject)
        {
            var type = (string)jObject["Type"];
            var properties = jObject["Properties"].ToObject<Dictionary<string, object>>();
            GrambyObject grambyObject;
            switch (type)
            {
                case "Part":
                    grambyObject = (Part)ResourceLoader.Load<PackedScene>("res://Scripts/GrambyObjects/Part/Part.tscn").Instance();
                    grambyObject.Properties = properties;
                    break;
                // TODO: the rest of the stuff :)
                default:
                    throw new JsonException($"{type} is not a supported GrambyObject type");
            }
            var children = jObject["Children"].ToObject<Dictionary<string, JObject>>();
            foreach (var pair in children)
            {
                var attachmentName = pair.Key;
                var attachedObject = ConvertJObject(pair.Value);
                var attachment = grambyObject.GetNode<Attachment>(attachmentName);
                attachment.AddChild(attachedObject);
            }
            return grambyObject;
        }

        public override GrambyObject ReadJson(JsonReader reader, Type objectType, GrambyObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            return ConvertJObject(jObject);
        }

        public override void WriteJson(JsonWriter writer, GrambyObject value, JsonSerializer serializer)
        {
            // Write this node
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(value.ClassName());
            writer.WritePropertyName("Properties");
            writer.WriteValue(value.Properties);

            // Write children
            writer.WritePropertyName("Children");
            writer.WriteStartObject();
            foreach (var pair in value.Children())
            {
                var attachment = pair.Key;
                var child = pair.Value;
                writer.WritePropertyName(attachment.Name);
                var attached = attachment.Attached;
                if (attached != null)
                {
                    WriteJson(writer, attached, serializer);
                }
                else
                {
                    writer.WriteNull();
                }
            }
            writer.WriteEndObject();
        }
    }

    public string Name = "Untitled";
    public string Author = "Anonymous";
    public string Notes = "Created with Gramby's Workshop";
    public GrambyObject RootNode;

    public RTGBFile(string name, string author, string notes, GrambyObject rootNode)
    {
        Name = name;
        Author = author;
        Notes = notes;
        RootNode = rootNode;
    }
}
