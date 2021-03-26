using Godot;
using System;

public class Settings : PanelContainer
{
    [Signal]
    public delegate void SettingsChanged();

    public bool HideAttachmentPoints
    {
        get => HideAttachmentPointsCheckBox.Pressed;
        set
        {
            HideAttachmentPointsCheckBox.Pressed = value;
            EmitSignal(nameof(SettingsChanged));
        }
    }

    private CheckBox HideAttachmentPointsCheckBox;

    public override void _Ready()
    {
        HideAttachmentPointsCheckBox = GetNode<CheckBox>("Control/HideAttachmentPoints");
        HideAttachmentPointsCheckBox.Connect("pressed", this, nameof(OnSettingsChanged));
    }

    private void OnSettingsChanged()
    {
        EmitSignal(nameof(SettingsChanged));
    }
}
