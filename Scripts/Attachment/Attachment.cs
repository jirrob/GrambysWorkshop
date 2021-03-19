using Godot;
using System;

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
        if (Type == AttachmentType.Ball)
        {
            Visible = false;
        }
    }
}
