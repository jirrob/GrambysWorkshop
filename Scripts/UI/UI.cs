using Godot;
using Newtonsoft.Json;
using System;

public class UI : Control
{
    public PackedScene PaletteHover = null;

    public Settings Settings;

    private WindowDialog RTGExportWindow;
    private TextEdit RTGExportWindowTextEdit;
    private GridContainer PreviewGridContainer;

    private Editor Editor;

    public override void _Ready()
    {
        ConnectMenuButtons();
        RTGExportWindow = GetNode<WindowDialog>("RTGExport");
        RTGExportWindowTextEdit = GetNode<TextEdit>("RTGExport/TextEdit");
        PreviewGridContainer = GetNode<GridContainer>("MainArea/LeftPanel/Palette/Control/Control/WrappingGridContainer");
        Settings = GetNode<Settings>("MainArea/RightPanel/Settings");
        Editor = GetParent<Editor>();
        SetupPreviews();
    }

    private void SetupPreviews()
    {
        var objectPreview = ResourceLoader.Load<PackedScene>("res://Scripts/UI/ObjectPreview.tscn");

        foreach (var scene in GrambyObject.AllObjectTypes)
        {
            var preview = (ObjectPreview)objectPreview.Instance();
            preview.WhatToRender = scene;
            preview.Connect("mouse_entered", this, nameof(SetPaletteHover), new Godot.Collections.Array { scene });
            preview.Connect("mouse_exited", this, nameof(SetPaletteHover), new Godot.Collections.Array { null });
            PreviewGridContainer.AddChild(preview);
        }
    }

    private void SetPaletteHover(PackedScene hover)
    {
        PaletteHover = hover;
    }

    private void ConnectMenuButtons()
    {
        // File
        var fileMenuButton = GetNode<MenuButton>("TopBar/File");
        var filePopup = fileMenuButton.GetPopup();
        filePopup.Connect("id_pressed", this, nameof(FileMenuHandler));
    }

    public void FileMenuHandler(int id)
    {
        switch (id)
        {
            case 0:
                ExportToRTG();
                break;
            default:
                throw new Exception("No handler for this file menu button: " + id);
        }
    }

    public void ExportToRTG()
    {
        var serialized = JsonConvert.SerializeObject(
                new RTGFile(Editor.Root),
                new JsonConverter[] { new RTGFile.RTGElement.RTGElementConverter() }
            );
        var utf8bytes = System.Text.Encoding.UTF8.GetBytes(serialized);
        var shareCode = System.Convert.ToBase64String(utf8bytes);
        RTGExportWindowTextEdit.Text = shareCode;
        RTGExportWindowTextEdit.GrabFocus();
        RTGExportWindowTextEdit.SelectAll();
        RTGExportWindow.Show();
    }
}
