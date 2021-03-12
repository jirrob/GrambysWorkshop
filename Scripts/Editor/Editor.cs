using Godot;
using Newtonsoft.Json;
using System;

public class Editor : Node
{
    public override void _Ready()
    {
        OS.WindowBorderless = false;
    }

    public override void _Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventKey e:
                switch ((KeyList)e.Scancode)
                {
                    case KeyList.Z:
                        GetNode<BuildTree>("UI/MainArea/RightPanel/Build/Control/Tree").ReflectGrambyObject(GetNode<Part>("Part"));
                        break;
                    case KeyList.X:
                        var serialized = JsonConvert.SerializeObject(
                                new RTGFile(GetNode<Part>("Part")),
                                new JsonConverter[] { new RTGFile.RTGElement.RTGElementConverter() }
                            );
                        var utf8bytes = System.Text.Encoding.UTF8.GetBytes(serialized);
                        var code = System.Convert.ToBase64String(utf8bytes);
                        GD.Print(serialized);
                        GD.Print(code);
                        break;
                }
                break;
        }
    }
}
