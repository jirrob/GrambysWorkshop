using Godot;
using System;
using System.Collections.Generic;

public abstract class GrambyObject : Spatial
{
    public static PackedScene[] AllObjectTypes = new PackedScene[] {
        ResourceLoader.Load<PackedScene>("res://Scripts/GrambyObjects/Part/Part.tscn")
    };

    public Dictionary<string, object> Properties;

    [Export]
    public NodePath SelectionAudioPath;

    private AudioStreamPlayer3D SelectionAudio;

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
            SetTransform();
        }
    }

    public override void _Ready()
    {
        SetTransform();
        InstantiateMaterial();
        SelectionAudio = GetNode<AudioStreamPlayer3D>(SelectionAudioPath);
    }

    public void SetTransform()
    {
        var attachment = GetNodeOrNull<Attachment>(DefaultAttachment);
        if (attachment != null)
        {
            // TODO: Might want to add a child spatial and change *its* 
            //       transform instead of having a GrambyObject change its
            //       own transform, which might mess some things up.
            Transform = attachment.Transform.Inverse();
        }
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

    public abstract void InstantiateMaterial();

    public void SetSelected(bool selected)
    {
        SetSelectedSound(selected);
        SetSelectedMaterial(selected);
    }

    protected void SetSelectedSound(bool selected)
    {
        SelectionAudio.Playing = selected;
    }

    protected abstract void SetSelectedMaterial(bool selected);
}
