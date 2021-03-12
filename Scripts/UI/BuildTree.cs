using Godot;
using System;

public class BuildTree : Tree
{
    public void ReflectGrambyObject(GrambyObject grambyObject)
    {
        Clear();
        AddGrambyObject(null, grambyObject);
    }

    private void AddGrambyObject(TreeItem parent, GrambyObject grambyObject)
    {
        var root = CreateItem(parent);
        root.SetText(0, grambyObject.ClassName());
        foreach (var child in grambyObject.Children())
        {
            if (child.Value != null)
            {
                AddGrambyObject(root, child.Value);
            }
        }
    }
}
