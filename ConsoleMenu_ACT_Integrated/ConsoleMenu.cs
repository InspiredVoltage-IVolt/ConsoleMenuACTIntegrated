﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ConsoleMenuTests")]

namespace ACT.Windows.Tools.ConsoleMenu;

/// <summary>
/// A simple, highly customizable, DOS-like console menu.
/// </summary>
public class ConsoleMenu : IEnumerable
{
  internal IConsole Console = new SystemConsole();
  public MenuConfig ConsoleMenuConfig = new MenuConfig();
  public ItemsCollection menuItems;
  private CloseTrigger closeTrigger;
  private ConsoleMenu? parent = null;

  /// <summary>
  /// Initializes a new instance of the <see cref="ConsoleMenu"/> class.
  /// </summary>
  public ConsoleMenu()
  {
    this.menuItems = new ItemsCollection();
    this.closeTrigger = new CloseTrigger();
  }

  public ItemsCollection CurrentMenuItems
  {
    get { return menuItems;}
    set { menuItems = value; }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ConsoleMenu"/> class
  /// with possibility to pre-select items via console parameter.
  /// </summary>
  /// <param name="args">args collection from Main.</param>
  /// <param name="level">Level of whole menu.</param>
  public ConsoleMenu(string[] args, int level)
  {
    if (args == null)
    {
      throw new ArgumentNullException(nameof(args));
    }

    if (level < 0)
    {
      throw new ArgumentException("Cannot be below 0", nameof(level));
    }

    this.menuItems = new ItemsCollection(args, level);
    this.closeTrigger = new CloseTrigger();
  }

  /// <summary>
  /// Gets menu items that can be modified.
  /// </summary>
  public IReadOnlyList<MenuItem> Items => this.menuItems.Items;

  /// <summary>
  /// Gets or sets selected menu item that can be modified.
  /// </summary>
  public MenuItem CurrentItem
  {
    get => this.menuItems.CurrentItem;
    set => this.menuItems.CurrentItem = value;
  }

  private IReadOnlyList<string> Titles
  {
    get
    {
      ConsoleMenu? current = this;
      List<string> titles = new List<string>();
      while (current != null)
      {
        titles.Add(current.ConsoleMenuConfig.Title ?? "");
        current = current.parent;
      }

      titles.Reverse();
      return titles;
    }
  }

  /// <summary>
  /// Don't run this method directly. Just pass a reference to this method.
  /// </summary>
  public static void Close() => throw new InvalidOperationException("Don't run this method directly. Just pass a reference to this method.");

  /// <summary>
  /// Close the menu before or after a menu action was triggered.
  /// </summary>
  public void CloseMenu()
  {
    this.closeTrigger.SetOn();
  }

  /// <summary>
  /// Adds a menu item into this instance.
  /// </summary>
  /// <param name="name">Name of menu item.</param>
  /// <param name="action">Action to call when menu item is chosen.</param>
  /// <returns>This instance with added menu item.</returns>
  public ConsoleMenu Add(string name, Action action)
  {
    if (name == null)
    {
      throw new ArgumentNullException(nameof(name));
    }

    if (action == null)
    {
      throw new ArgumentNullException(nameof(action));
    }

    if (action.Target is ConsoleMenu child && action == child.Show)
    {
      child.parent = this;
    }

    this.menuItems.Add(name, action);
    return this;
  }

  /// <summary>
  /// Adds a menu item into this instance.
  /// </summary>
  /// <param name="name">Name of menu item.</param>
  /// <param name="action">Action to call when menu item is chosen.</param>
  /// <returns>This instance with added menu item.</returns>
  public ConsoleMenu Add(string name, Action<ConsoleMenu> action)
  {
    if (name == null)
    {
      throw new ArgumentNullException(nameof(name));
    }

    if (action is null)
    {
      throw new ArgumentNullException(nameof(action));
    }

    this.menuItems.Add(name, () => action(this));
    return this;
  }

  /// <summary>
  /// Adds range of menu items into this instance.
  /// </summary>
  /// <param name="menuItems">Menu items to add.</param>
  /// <returns>This instance with added menu items.</returns>
  public ConsoleMenu AddRange(IEnumerable<Tuple<string, Action>> menuItems)
  {
    if (menuItems is null)
    {
      throw new ArgumentNullException(nameof(menuItems));
    }

    foreach (var item in menuItems)
    {
      this.Add(item.Item1, item.Item2);
    }

    return this;
  }

  /// <summary>
  /// Applies an configuration action on this instance.
  /// </summary>
  /// <param name="configure">Configuration action.</param>
  /// <returns>An configured instance.</returns>
  /// <exception cref="ArgumentNullException"><paramref name="configure"/> is null.</exception>
  public ConsoleMenu Configure(Action<MenuConfig> configure)
  {
    if (configure is null)
    {
      throw new ArgumentNullException(nameof(configure));
    }

    configure.Invoke(this.ConsoleMenuConfig);
    return this;
  }

  /// <summary>
  /// Displays the menu in console.
  /// </summary>
  public void Show()
  {
    new ConsoleMenuDisplay(
        this.menuItems,
        this.Console,
        new List<string>(this.Titles),
        this.ConsoleMenuConfig,
        this.closeTrigger).Show();
  }

  /// <summary>
  /// Returns an enumeration of the current menu items.
  /// See <see cref="Items"/>.
  /// </summary>
  /// <returns>An enumeration of the current menu items.</returns>
  public IEnumerator GetEnumerator() => this.Items.GetEnumerator();
}
