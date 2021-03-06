using Godot;
using Newtonsoft.Json;
using System;

public class RTGBFile
{
    public static string Version = "0.1.0";

    public class GrambyObjectConverter : JsonConverter<GrambyObject>
    {
        public override GrambyObject ReadJson(JsonReader reader, Type objectType, GrambyObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, GrambyObject value, JsonSerializer serializer)
        {
            // Write this node
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(value.GetType().Name);
            writer.WritePropertyName("Position");
            serializer.Serialize(writer, value.GlobalTransform.origin);
            writer.WritePropertyName("Properties");
            writer.WriteValue(value.Properties);

            // Write children
            writer.WritePropertyName("Children");
            writer.WriteStartArray();
            foreach (Node child in value.GetChildren())
            {
                if (child is GrambyObject c)
                {
                    WriteJson(writer, c, serializer);
                }
            }
            writer.WriteEndArray();
        }
    }

    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(value.x);
            writer.WritePropertyName("y");
            writer.WriteValue(value.y);
            writer.WritePropertyName("z");
            writer.WriteValue(value.z);
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
