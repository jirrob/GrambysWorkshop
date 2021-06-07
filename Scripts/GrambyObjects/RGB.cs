using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

[JsonConverter(typeof(RGB.RGBConverter))]
public class RGB
{
    public byte R;
    public byte G;
    public byte B;

    public RGB(byte r = 0, byte g = 0, byte b = 0)
    {
        R = r;
        G = g;
        B = b;
    }

    public RGB(Color from)
    {
        R = (byte)from.r8;
        G = (byte)from.g8;
        B = (byte)from.b8;
    }

    public Color toColor() => Color.Color8(R, G, B);

    public class RGBConverter : JsonConverter<RGB>
    {
        public override RGB ReadJson(JsonReader reader, Type objectType, RGB existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var rgb = new RGB();
            var arr = JArray.Load(reader);
            rgb.R = (byte)arr[0];
            rgb.G = (byte)arr[1];
            rgb.B = (byte)arr[2];
            return rgb;
        }

        public override void WriteJson(JsonWriter writer, RGB value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.R);
            writer.WriteValue(value.G);
            writer.WriteValue(value.B);
            writer.WriteEndArray();
        }
    }
}
