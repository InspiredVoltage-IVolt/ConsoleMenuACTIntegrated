﻿using System;
using System.Collections.Generic;

namespace ConsoleTools;

internal sealed class ItemsCollection
{
  private readonly List<MenuItem> menuItems = new List<MenuItem>();
  private readonly MenuConfig config = new MenuConfig();
  private int? selectedIndex;
  private string? selectedName;
  private int currentItemIndex;

  public ItemsCollection()
  {
  }

  public ItemsCollection(string[] args, int level)
  {
    this.SetSelectedItems(args, level);
  }

  public List<MenuItem> Items => this.menuItems;

  public MenuItem CurrentItem
  {
    get => this.menuItems[this.currentItemIndex];
    set => this.menuItems[this.currentItemIndex] = value;
  }

  public void Add(string name, Action action)
  {
    this.menuItems.Add(new MenuItem(name, action, this.menuItems.Count));
  }

  public void ResetCurrentIndex()
  {
    this.selectedIndex = 0;
  }

  public void SetSelectedItems(string[] args, int level)
  {
    var arg = Array.Find(args, a => a.StartsWith(this.config.ArgsPreselectedItemsKey));
    this.SetSelectedItems(level, this.config.ArgsPreselectedItemsKey, ref arg);
  }

  public MenuItem? GetSeletedItem()
  {
    if (this.selectedIndex < this.menuItems.Count)
    {
      return this.menuItems[this.selectedIndex.Value];
    }

    if (this.selectedName != null)
    {
      return this.menuItems.Find(item => item.Name == this.selectedName);
    }

    return null;
  }

  public void SelectClosestVisibleItem(VisibilityManager visibility)
  {
    this.currentItemIndex = visibility.IndexOfClosestVisibleItem(this.currentItemIndex);
  }

  public void SelectNextVisibleItem(VisibilityManager visibility)
  {
    this.currentItemIndex = visibility.IndexOfNextVisibleItem(this.currentItemIndex);
  }

  public void SelectPreviousVisibleItem(VisibilityManager visibility)
  {
    this.currentItemIndex = visibility.IndexOfPreviousVisibleItem(this.currentItemIndex);
  }

  public bool CanSelect(char ch)
  {
    return ch >= '0' && (ch - '0') < this.menuItems.Count; // is in range 0.._menuItems.Count
  }

  public void Select(char ch)
  {
    this.currentItemIndex = ch - '0';
  }

  public void UnsetSelectedIndex()
  {
    this.selectedIndex = null;
    this.selectedName = null;
  }

  internal bool IsSelected(MenuItem menuItem)
  {
    return this.currentItemIndex == menuItem.Index;
  }

  private void SetSelectedItems(int level, string paramKey, ref string? arg)
  {
    if (arg == null)
    {
      return;
    }

    arg = arg.Replace(paramKey, string.Empty).Trim();
    var items = arg.SplitItems(this.config.ArgsPreselectedItemsValueSeparator, '\'');
    if (level < items.Count)
    {
      var item = items[level].Trim('\'');
      if (int.TryParse(item, out var selectedIndex))
      {
        this.selectedIndex = selectedIndex;
        return;
      }

      this.selectedName = item;
    }
  }
}
