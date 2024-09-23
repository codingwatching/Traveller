using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ColdMint.scripts.map.events;

namespace ColdMint.scripts.inventory;

public abstract class ItemContainerDisplayTemplate : IItemContainerDisplay
{
    protected readonly List<IItemDisplay> ItemDisplayList = [];
    private IItemContainer? _itemContainer;

    public void BindItemContainer(IItemContainer itemContainer)
    {
        if (_itemContainer == itemContainer)
        {
            return;
        }

        if (_itemContainer != null)
        {
            _itemContainer.SelectedItemChangeEvent -= OnSelectedItemChangeEvent;
            _itemContainer.ItemDataChangeEvent -= OnItemDataChangeEvent;
        }

        _itemContainer = itemContainer;
        _itemContainer.SelectedItemChangeEvent += OnSelectedItemChangeEvent;
        _itemContainer.ItemDataChangeEvent += OnItemDataChangeEvent;
        var totalCapacity = itemContainer.GetTotalCapacity();
        var currentCapacity = ItemDisplayList.Count;
        var capacityDifference = totalCapacity - currentCapacity;
        var adjustedEndIndex = totalCapacity;
        if (capacityDifference > 0)
        {
            //There are those that need to be added, and we create them.
            //有需要添加的，我们创建他们。
            for (var i = 0; i < capacityDifference; i++)
            {
                AddItemDisplay();
            }
        }
        else if (capacityDifference < 0)
        {
            //There are things that need to be hidden
            //有需要被隐藏的
            adjustedEndIndex += capacityDifference;
            var loopEndIndex = currentCapacity + capacityDifference;
            for (var i = currentCapacity - 1; i >= loopEndIndex; i--)
            {
                var itemDisplay = ItemDisplayList[i];
                itemDisplay.Update(null);
                itemDisplay.HideSelf();
            }
        }

        UpdateData(itemContainer, adjustedEndIndex);
    }

    private void OnItemDataChangeEvent(ItemDataChangeEvent itemDataChangeEvent)
    {
        if (_itemContainer == null)
        {
            return;
        }

        var usedCapacity = _itemContainer.GetUsedCapacity();
        UpdateDataForSingleLocation(_itemContainer, itemDataChangeEvent.NewIndex, usedCapacity);
    }

    private void OnSelectedItemChangeEvent(SelectedItemChangeEvent selectedItemChangeEvent)
    {
        if (_itemContainer == null)
        {
            return;
        }

        var usedCapacity = _itemContainer.GetUsedCapacity();
        UpdateDataForSingleLocation(_itemContainer, selectedItemChangeEvent.OldIndex, usedCapacity);
        UpdateDataForSingleLocation(_itemContainer, selectedItemChangeEvent.NewIndex, usedCapacity);
    }

    /// <summary>
    /// <para>Update data</para>
    /// <para>更新数据</para>
    /// </summary>
    /// <remarks>
    ///<para>Used to batch update the Item data in itemContainer to the display.</para>
    ///<para>用于将itemContainer内的Item数据批量更新到显示器内。</para>
    /// </remarks>
    /// <param name="itemContainer">
    ///<para>Item container data</para>
    ///<para>物品容器数据</para>
    /// </param>
    /// <param name="endIndex">
    ///<para>endIndex</para>
    ///<para>结束位置</para>
    /// </param>
    /// <param name="startIndex">
    ///<para>startIndex</para>
    ///<para>起始位置</para>
    /// </param>
    private void UpdateData(IItemContainer itemContainer, int endIndex, int startIndex = 0)
    {
        var usedCapacity = itemContainer.GetUsedCapacity();
        for (var i = startIndex; i < endIndex; i++)
        {
            var itemDisplay = ItemDisplayList[i];
            itemDisplay.Update(i < usedCapacity ? itemContainer.GetItem(i) : null);
            itemDisplay.ShowSelf();
        }
    }

    /// <summary>
    /// <para>Update data for a single location</para>
    /// <para>更新单个位置的数据</para>
    /// </summary>
    /// <param name="itemContainer"></param>
    /// <param name="index"></param>
    /// <param name="usedCapacity"></param>
    private void UpdateDataForSingleLocation(IItemContainer itemContainer, int index, int usedCapacity)
    {
        var itemDisplay = ItemDisplayList[index];
        itemDisplay.Update(index < usedCapacity ? itemContainer.GetItem(index) : null);
    }

    /// <summary>
    /// <para>Add item display</para>
    /// <para>添加物品显示器</para>
    /// </summary>
    protected abstract void AddItemDisplay();

    public IEnumerator<IItemDisplay> GetEnumerator()
    {
        return ItemDisplayList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}