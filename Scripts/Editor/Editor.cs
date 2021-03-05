using Godot;
using System;

public class Editor : Node
{
    public override void _Ready()
    {
        OS.WindowBorderless = false;
    }
}
