using Godot;
using System;

public class Part : GrambyObject
{
    public Material Material;

    public override void InstantiateMaterial()
    {
        var mesh = GetNode<MeshInstance>("Part");
        Material = (Material)mesh.GetSurfaceMaterial(0).Duplicate(true);
        mesh.SetSurfaceMaterial(0, Material);
    }

    protected override void SetSelectedMaterial(bool selected)
    {
        ((ShaderMaterial)Material.NextPass).SetShaderParam("selected", selected);
    }
}
