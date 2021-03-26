using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class Editor : Node
{
    public UI UI;

    public GrambyObject Root;

    private List<GrambyObject> _selection = null;

    public List<GrambyObject> Selection
    {
        get => _selection;
        set
        {
            UpdateSelection(false);
            _selection = value;
            UpdateSelection(true);
        }
    }

    private BuildTree Tree;

    private GrambyObject DraggedObject = null;

    private Godot.Collections.Array AreaExclusionArray;

    private Vector2 LastMousePosition = new Vector2();

    private Camera Camera;

    private PhysicsDirectSpaceState SpaceState;

    /// Used to determine if a selection can/should be made after letting go of the mouse
    private bool EligibleForSelection;

    public override void _Ready()
    {
        OS.WindowBorderless = false;
        UI = GetNode<UI>("UI");
        Camera = GetViewport().GetCamera();
        SpaceState = Camera.GetWorld().DirectSpaceState;
        UI.Settings.Connect(nameof(Settings.SettingsChanged), this, nameof(OnSettingsChanged));
        // TODO temporary
        Root = GetNode<GrambyObject>("Part");
        Tree = GetNode<BuildTree>("UI/MainArea/RightPanel/Build/Control/Tree");
        Tree.ReflectGrambyObject(Root);
    }

    // TODO: this is pretty awful isnt it?
    //       is there any way we could split
    //       this class up into a few others?
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion e)
        {
            LastMousePosition = e.GlobalPosition;
            if (DraggedObject != null)
            {
                SetDraggedObjectPosition();
            }
            EligibleForSelection = false;
        }

        if (UI.PaletteHover != null && Input.IsActionJustPressed("drag_object"))
        {
            DisposeDraggedObject();
            DraggedObject = (GrambyObject)UI.PaletteHover.Instance();
            AreaExclusionArray = new Godot.Collections.Array();
            SetAreaExclusionArray(DraggedObject);
            AddChild(DraggedObject);
            SetDraggedObjectPosition();
            Tree.ReflectGrambyObject(Root);
            SetAllAttachmentsHidden(Root, false);
        }
        else if (Input.IsActionJustReleased("drag_object"))
        {
            if (DraggedObject != null && DraggedObject.GetParent() == this)
            {
                DisposeDraggedObject();
            }
            DraggedObject = null;
            AreaExclusionArray = null;
            Tree.ReflectGrambyObject(Root);
            SetAllAttachmentsHidden(Root, UI.Settings.HideAttachmentPoints);
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("select"))
        {
            EligibleForSelection = true;
        }
        else if (Input.IsActionJustReleased("select"))
        {
            if (EligibleForSelection)
            {
                Godot.Collections.Dictionary results = Raycast(0b0000000000000000000000000000010);
                GrambyObject grambyObject;
                try
                {
                    var collider = (Area)results["collider"];
                    grambyObject = collider.GetParent<GrambyObject>();
                }
                catch
                {
                    grambyObject = null;
                }

                if (grambyObject != null)
                {
                    Selection = new List<GrambyObject> { grambyObject };
                }
                else
                {
                    Selection = null;
                }
            }
        }
    }

    private void SetAllAttachmentsHidden(GrambyObject root, bool hidden)
    {
        foreach (var pair in root.Children())
        {
            pair.Key.Hidden = hidden;
            if (pair.Value != null)
            {
                SetAllAttachmentsHidden(pair.Value, hidden);
            }
        }
    }

    private void OnSettingsChanged()
    {
        SetAllAttachmentsHidden(Root, UI.Settings.HideAttachmentPoints);
    }

    private void UpdateSelection(bool selected)
    {
        if (_selection != null)
        {
            foreach (var selectedObject in _selection)
            {
                selectedObject.SetSelected(selected);
            }
        }
    }

    private void SetAreaExclusionArray(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            if (child is Area area)
            {
                AreaExclusionArray.Add(area);
            }
            else
            {
                SetAreaExclusionArray(child);
            }
        }
    }

    private void SetDraggedObjectPosition()
    {
        Godot.Collections.Dictionary results = Raycast(0b0000000000000000000000000000001);

        Attachment attachment;
        try
        {
            var collider = (Area)results["collider"];
            attachment = collider.GetParent<Attachment>();
        }
        catch
        {
            attachment = null;
        };

        var draggedObjectParent = DraggedObject.GetParent();

        bool attachmentValid = attachment != null;
        if (attachmentValid)
        {
            GrambyObject attached = attachment.Attached;
            attachmentValid = (attached == null || attached == DraggedObject)
                && attachment.Type == Attachment.AttachmentType.Cup;
        }

        if (!attachmentValid)
        {
            if (draggedObjectParent != this)
            {
                if (draggedObjectParent != null)
                {
                    draggedObjectParent.RemoveChild(DraggedObject);
                }
                AddChild(DraggedObject);
            }
            DraggedObject.Translation = ProjectMousePositionToWorldPosition(LastMousePosition);
        }
        else
        {
            if (draggedObjectParent != attachment)
            {
                if (draggedObjectParent != null)
                {
                    draggedObjectParent.RemoveChild(DraggedObject);
                }
                attachment.AddChild(DraggedObject);
                DraggedObject.SetTransform();
            }
        }
    }

    private Godot.Collections.Dictionary Raycast(uint mask)
    {
        var from = Camera.ProjectRayOrigin(LastMousePosition);
        var to = from + Camera.ProjectRayNormal(LastMousePosition) * 1000;
        var results = SpaceState.IntersectRay(
            from,
            to,
            AreaExclusionArray,
            mask,
            false,
            true
        );
        return results;
    }

    private void DisposeDraggedObject()
    {
        if (DraggedObject != null)
        {
            DraggedObject.QueueFree();
        }
    }

    private Vector3 ProjectMousePositionToWorldPosition(Vector2 mousePosition)
    {
        return Camera.ProjectPosition(mousePosition, 5);
    }
}
