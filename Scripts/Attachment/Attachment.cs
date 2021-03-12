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

    [Export]
    public NodePath AttachedPath;

    public GrambyObject Attached
    {
        get => AttachedPath == null ? null : GetNode<GrambyObject>(AttachedPath);
    }

    public override void _Ready()
    {
        if (Type == AttachmentType.Ball)
        {
            Visible = false;
        }
    }
}
