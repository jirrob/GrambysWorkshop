using Godot;
using System;

public class WrappingGridContainer : Control
{
    private GridContainer GridContainer;
    private float DefaultChildWidth = 50.0f;

    public override void _Ready()
    {
        GridContainer = GetNode<GridContainer>("WrappingGridContainer");
        Connect("resized", this, nameof(OnResize));
    }

    private float ChildWidth()
    {
        var children = GridContainer.GetChildren();
        if (children.Count == 0)
        {
            return DefaultChildWidth;
        }
        foreach (var child in children)
        {
            if (child is Control control)
            {
                return control.RectSize.x;
            }
        }
        return DefaultChildWidth;
    }

    private void OnResize()
    {
        var hSeparation = 4;
        var columns = (int)(RectSize.x / (ChildWidth() + hSeparation));
        GridContainer.Columns = columns <= 0 ? 1 : columns;
    }
}
