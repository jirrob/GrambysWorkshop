using Godot;
using System;

public class Attachment : Spatial
{
    public enum AttachmentType
    {
        Any,
        Preferred,
        Specific,
    }

    [Export]
    public AttachmentType Type;

    public Attachment(Transform transform) : base()
    {
        Transform = transform;
    }
}
