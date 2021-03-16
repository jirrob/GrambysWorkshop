using Godot;
using System;

public class ObjectPreview : ViewportContainer
{
    public Viewport Viewport;

    [Export]
    public PackedScene WhatToRender;

    public override void _Ready()
    {
        Viewport = GetNode<Viewport>("Viewport");
        Render();
    }

    public void Render()
    {
        // Instance scene to viewport
        var instance = WhatToRender.Instance();
        Viewport.AddChild(instance);
        // Also set hint text while we're here with a handle to the instance
        if (instance is GrambyObject grambyObject)
        {
            HintTooltip = grambyObject.ClassName();
        }
        // Render
        Viewport.RenderTargetUpdateMode = Viewport.UpdateMode.Once;
    }
}
