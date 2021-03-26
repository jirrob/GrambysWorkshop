using Godot;
using System;
using System.Collections.Generic;

public class Attachment : Spatial
{
    public enum AttachmentType
    {
        Cup,
        Ball,
    }

    [Export]
    public AttachmentType Type = AttachmentType.Cup;

    [Export]
    public string OnlyBall;

    public bool Hidden
    {
        get => !Meshes[0].Visible;
        set
        {
            foreach (var mesh in Meshes)
            {
                mesh.Visible = !value;
            }
        }
    }

    private List<VisualInstance> Meshes;

    public GrambyObject Attached
    {
        get
        {
            foreach (Node child in GetChildren())
            {
                if (child is GrambyObject grambyObject)
                {
                    return grambyObject;
                }
            }
            return null;
        }
    }

    public override void _Ready()
    {
        if (Type == AttachmentType.Ball) // TODO: put this in a setter
        {
            Visible = false;
        }
        Meshes = new List<VisualInstance>();
        foreach (var child in GetChildren())
        {
            if (child is VisualInstance vi)
            {
                Meshes.Add(vi);
            }
        }
        Hidden = false;
    }
}
