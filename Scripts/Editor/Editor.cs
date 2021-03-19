using Godot;
using Newtonsoft.Json;
using System;

public class Editor : Node
{
    public UI UI;

    public GrambyObject Root;

    private BuildTree Tree;

    private GrambyObject DraggedObject = null;

    private Godot.Collections.Array AreaExclusionArray;

    private Vector2 LastMousePosition = new Vector2();

    private Camera Camera;

    private PhysicsDirectSpaceState SpaceState;

    public override void _Ready()
    {
        OS.WindowBorderless = false;
        UI = GetNode<UI>("UI");
        Camera = GetViewport().GetCamera();
        SpaceState = Camera.GetWorld().DirectSpaceState;
        // TODO temporary
        Root = GetNode<GrambyObject>("Part");
        Tree = GetNode<BuildTree>("UI/MainArea/RightPanel/Build/Control/Tree");
        Tree.ReflectGrambyObject(Root);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion e)
        {
            LastMousePosition = e.GlobalPosition;
            if (DraggedObject != null)
            {
                SetDraggedObjectPosition();
            }
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
        var from = Camera.ProjectRayOrigin(LastMousePosition);
        var to = from + Camera.ProjectRayNormal(LastMousePosition) * 1000;
        var results = SpaceState.IntersectRay(
            from,
            to,
            AreaExclusionArray,
            0b1111111111111111111111111111111,
            false,
            true
        );

        Attachment attachment;
        try
        {
            var collider = (Area)results["collider"];
            attachment = (Attachment)collider.GetParent();
        }
        catch
        {
            attachment = null;
        };

        var draggedObjectParent = DraggedObject.GetParent();

        if (attachment == null)
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
