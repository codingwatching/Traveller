using Godot;

namespace ColdMint.scripts.inventory;

/// <summary>
/// <para>PlaceholderItem</para>
/// <para>占位物品</para>
/// </summary>
public class PlaceholderItem : IItem
{
    public string Id { get; set; }
    public Texture2D Icon { get; }
    public string Name { get; }
    public string? Description { get; } = null;
    public int Quantity { get; set; } = 1;
    public int MaxQuantity { get; } = 1;
    public bool IsSelect { get; set; }

    public int MergeableItemCount(IItem other, int unallocatedQuantity)
    {
        return 0;
    }

    public IItem? CreateItem(int number)
    {
        return null;
    }

    public void Use(Node2D? owner, Vector2 targetGlobalPosition)
    {
    }
}