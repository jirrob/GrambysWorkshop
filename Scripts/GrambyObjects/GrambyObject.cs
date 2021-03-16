using Godot;
using System;
using System.Collections.Generic;

public abstract class GrambyObject : Spatial
{
    public static PackedScene[] AllObjectTypes = new PackedScene[] {
        ResourceLoader.Load<PackedScene>("res://Scripts/GrambyObjects/Part/Part.tscn")
    };

    public Dictionary<string, object> Properties;

    private NodePath _defaultAttachment;

    [Export]
    public NodePath DefaultAttachment
    {
        get
        {
            return _defaultAttachment;
        }
        set
        {
            _defaultAttachment = value;
            var attachment = GetNodeOrNull<Attachment>(value);
            if (attachment != null)
            {
                // TODO: Might want to add a child spatial and change *its* 
                //       transform instead of having a GrambyObject change its
                //       own transform, which might mess some things up.
                Transform = attachment.Transform.Inverse();
            }
        }
    }

    public override void _Ready()
    {
        DefaultAttachment = DefaultAttachment;
    }

    public string ClassName()
    {
        return GetType().Name;
    }

    public Dictionary<Attachment, GrambyObject> Children()
    {
        var children = new Dictionary<Attachment, GrambyObject>();
        foreach (Node child in GetChildren())
        {
            if (child is Attachment attachment)
            {
                children.Add(attachment, attachment.Attached);
            }
        }
        return children;
    }
}
