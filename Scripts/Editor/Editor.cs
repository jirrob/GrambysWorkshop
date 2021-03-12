using Godot;
using Newtonsoft.Json;
using System;

public class Editor : Node
{
    public GrambyObject Root;

    public override void _Ready()
    {
        OS.WindowBorderless = false;
        // TODO temporary
        Root = GetNode<GrambyObject>("Part");
        GetNode<BuildTree>("UI/MainArea/RightPanel/Build/Control/Tree").ReflectGrambyObject(GetNode<Part>("Part"));
    }
}
