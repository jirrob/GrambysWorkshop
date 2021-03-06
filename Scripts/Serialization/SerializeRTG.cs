using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public static class SerializeRTG
{
    public static void SerializeLeaf(GrambyObject grambyObject, JsonTextWriter writer)
    {
        var position = grambyObject.GlobalTransform.origin;
        writer.WriteStartArray();
        writer.WriteValue(grambyObject.GetType().Name);
        writer.WriteStartArray();
        writer.WriteStartArray();
        writer.WriteValue(position.x);
        writer.WriteValue(position.y);
        writer.WriteValue(position.z);
        writer.WriteEndArray();
        writer.WriteEndArray();
        if (grambyObject.Properties != null)
        {
            writer.WriteValue(grambyObject.Properties);
        }
        else
        {
            writer.WriteStartArray();
            writer.WriteEndArray();
        }
        writer.WriteEndArray();
    }

    public static void Serialize(GrambyObject grambyObject, JsonTextWriter writer)
    {
        foreach (object child in grambyObject.GetChildren())
        {
            if (child is GrambyObject obj)
            {
                Serialize(obj, writer);
            }
        }
        SerializeLeaf(grambyObject, writer);
    }

    public static string SerializeToString(GrambyObject grambyObject)
    {
        var stringWriter = new System.IO.StringWriter();
        var jsonWriter = new JsonTextWriter(stringWriter);
        jsonWriter.WriteStartArray();
        Serialize(grambyObject, jsonWriter);
        jsonWriter.WriteEndArray();
        jsonWriter.Close();
        return stringWriter.ToString();
    }
}
