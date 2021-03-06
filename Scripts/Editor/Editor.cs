using Godot;
using Newtonsoft.Json;
using System;

public class Editor : Node
{
    public override void _Ready()
    {
        OS.WindowBorderless = false;

    }

    public void _on_InsertObject_pressed()
    {
        GetNode<WindowDialog>("UI/InsertDialog").Popup_();
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey e)
        {
            if ((KeyList)e.Scancode == KeyList.Z)
            {
                // GD.Print(SerializeRTG.SerializeToString(GetNode<TestObject>("TestObject")));
                var file = new RTGBFile("Test file", "Taylor", "This is a test file!", GetNode<GrambyObject>("TestObject"));
                var json = JsonConvert.SerializeObject(file, new JsonConverter[] { new RTGBFile.GrambyObjectConverter(), new RTGBFile.Vector3Converter() });
                GD.Print(json);
            }
        }
    }
}
