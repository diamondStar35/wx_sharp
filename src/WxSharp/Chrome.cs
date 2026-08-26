using System;
using System.Collections.Generic;


namespace WxSharp;

/// <summary>What kind of entry a menu or toolbar item is.</summary>
public enum MenuItemKind
{
    Normal = 0,
    /// <summary>A checkable item that toggles independently.</summary>
    Check = 1,
    /// <summary>A checkable item that is mutually exclusive with the radio items adjacent to it.</summary>
    Radio = 2,
    /// <summary>A separator line. Returned by <see cref="MenuItem.Kind"/>; not something you append directly -
    /// use <see cref="Menu.AppendSeparator"/>.</summary>
    Separator = 3,
}

/// <summary>The wxWidgets stock identifiers. Using one gives an item the platform's own label, icon,
/// accelerator and, on macOS, its correct placement in the application menu - which is also what a screen
/// reader announces, so prefer these over ad-hoc IDs for standard commands.</summary>
public static class StandardId
{
    // The ordinals here are the wire format; wxsharp_stock_id() in chrome.cpp maps them to wxID_* values.
    // Keep the two lists in the same order.
    private static readonly int[] Cache = new int[39];
    private static readonly bool[] Loaded = new bool[39];

    private static int Get(int which)
    {
        if (Loaded[which]) return Cache[which];
        Cache[which] = NativeMethods.wxsharp_stock_id(which);
        Loaded[which] = true;
        return Cache[which];
    }

    public static int Any => Get(0);
    public static int Ok => Get(1);
    public static int Cancel => Get(2);
    public static int Yes => Get(3);
    public static int No => Get(4);
    public static int Apply => Get(5);
    public static int Close => Get(6);
    public static int Help => Get(7);
    public static int Exit => Get(8);
    public static int New => Get(9);
    public static int Open => Get(10);
    public static int Save => Get(11);
    public static int SaveAs => Get(12);
    public static int Preferences => Get(13);
    public static int About => Get(14);
    public static int Undo => Get(15);
    public static int Redo => Get(16);
    public static int Cut => Get(17);
    public static int Copy => Get(18);
    public static int Paste => Get(19);
    public static int Delete => Get(20);
    public static int SelectAll => Get(21);
    public static int Find => Get(22);
    public static int Replace => Get(23);
    public static int Add => Get(24);
    public static int Remove => Get(25);
    public static int Edit => Get(26);
    public static int Refresh => Get(27);
    public static int Properties => Get(28);
    public static int Print => Get(29);
    public static int Stop => Get(30);
    public static int Clear => Get(31);
    public static int Up => Get(32);
    public static int Down => Get(33);
    public static int Backward => Get(34);
    public static int Forward => Get(35);
    public static int Revert => Get(37);
    public static int None => Get(38);
}

/// <summary>Allocates window and command IDs that are guaranteed not to collide with wxWidgets' own, for the
/// menu items and accelerators an application invents at runtime. This is Phoenix's <c>wx.NewIdRef</c>: the
/// ID is reserved until it is released.</summary>
public static class IdManager
{
    /// <summary>Reserves and returns a fresh command ID.</summary>
    public static int NewId()
    {
        _ = App.RequireCurrent();
        return NativeMethods.wxsharp_new_id();
    }

    /// <summary>Returns an ID from <see cref="NewId"/> to the pool. Only release IDs nothing still refers to.</summary>
    public static void Release(int id)
    {
        _ = App.RequireCurrent();
        NativeMethods.wxsharp_release_id(id);
    }
}

/// <summary>The modifier keys of an accelerator. The values match wxWidgets' own accelerator flags.</summary>
[Flags]
public enum AcceleratorModifiers
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    /// <summary>The physical Control key. The same as <see cref="Control"/> everywhere except macOS, where
    /// <see cref="Control"/> means Command.</summary>
    RawControl = 8,
}

/// <summary>One entry in an accelerator table: a key combination and the command ID it sends.</summary>
public readonly record struct AcceleratorEntry(AcceleratorModifiers Modifiers, int KeyCode, int CommandId)
{
    /// <summary>Parses a wxWidgets accelerator string - "Ctrl+O", "Alt+Shift+F4", "F11" - and binds it to
    /// <paramref name="commandId"/>. Returns false when the string names no valid combination, which is what
    /// a user-configurable shortcut needs rather than an exception.</summary>
    public static bool TryParse(string text, int commandId, out AcceleratorEntry entry)
    {
        ArgumentNullException.ThrowIfNull(text);
        _ = App.RequireCurrent();
        entry = default;
        if (!NativeMethods.wxsharp_accelerator_parse(text, out var modifiers, out var keyCode))
            return false;
        entry = new AcceleratorEntry((AcceleratorModifiers)modifiers, keyCode, commandId);
        return true;
    }

    /// <summary>Parses a wxWidgets accelerator string, throwing when it is not valid.</summary>
    public static AcceleratorEntry Parse(string text, int commandId)
        => TryParse(text, commandId, out var entry)
            ? entry
            : throw new FormatException($"'{text}' is not a valid accelerator.");

    /// <summary>Formats the combination the way wxWidgets writes it, suitable for a menu label's accelerator
    /// suffix or for showing the user their current shortcut.</summary>
    public override unsafe string ToString()
    {
        _ = App.RequireCurrent();
        var length = NativeMethods.wxsharp_accelerator_format((int)Modifiers, KeyCode, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer)
            _ = NativeMethods.wxsharp_accelerator_format((int)Modifiers, KeyCode, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }
}

/// <summary>One entry in a <see cref="Menu"/>. Items are owned by the menu that holds them; the wrapper never
/// deletes the underlying item on its own.</summary>
public sealed class MenuItem
{
    private nint _handle;

    internal MenuItem(nint handle)
    {
        _handle = handle != 0 ? handle : throw new InvalidOperationException("wxWidgets failed to create the menu item.");
        Id = NativeMethods.wxsharp_menuitem_get_id(handle);
    }

    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(MenuItem));

    /// <summary>The command ID this item sends when chosen.</summary>
    public int Id { get; }

    public MenuItemKind Kind => (MenuItemKind)NativeMethods.wxsharp_menuitem_get_kind(Handle);

    /// <summary>The item's text, including its <c>&amp;</c> mnemonic and any <c>"\tCtrl+O"</c> accelerator
    /// suffix. The accelerator part is what a screen reader announces alongside the item.</summary>
    public unsafe string Label
    {
        get
        {
            var length = NativeMethods.wxsharp_menuitem_get_label(Handle, null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_menuitem_get_label(Handle, p, buffer.Length);
            return Utf8String.Decode(buffer, length);
        }
        set => NativeMethods.wxsharp_menuitem_set_label(Handle, value ?? string.Empty);
    }

    /// <summary>The item's help string, shown in the status bar and read as its description.</summary>
    public unsafe string Help
    {
        get
        {
            var length = NativeMethods.wxsharp_menuitem_get_help(Handle, null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_menuitem_get_help(Handle, p, buffer.Length);
            return Utf8String.Decode(buffer, length);
        }
        set => NativeMethods.wxsharp_menuitem_set_help(Handle, value ?? string.Empty);
    }

    /// <summary>Whether the item can be chosen.</summary>
    ///
    /// <remarks>
    /// Setting this pushes a decision that has to be repeated from every code path that could change the
    /// answer, which is how menu items end up stale. Answering <see cref="WxEvents.UpdateUI"/> for the
    /// command's ID instead lets wxWidgets ask whenever it needs to know. Reading it is always fine, and
    /// setting it is still right for state that genuinely changes in one place.
    /// </remarks>
    public bool Enabled
    {
        get => NativeMethods.wxsharp_menuitem_is_enabled(Handle);
        set => NativeMethods.wxsharp_menuitem_enable(Handle, value);
    }

    /// <summary>Whether this item is a check or radio item at all.</summary>
    public bool IsCheckable => NativeMethods.wxsharp_menuitem_is_checkable(Handle);

    /// <summary>The check state. Always false, and ignored on set, for an item that is not checkable.
    /// As with <see cref="Enabled"/>, answering <see cref="WxEvents.UpdateUI"/> is usually better than
    /// pushing this from every place the underlying state changes.</summary>
    public bool Checked
    {
        get => NativeMethods.wxsharp_menuitem_is_checked(Handle);
        set => NativeMethods.wxsharp_menuitem_check(Handle, value);
    }

    /// <summary>The submenu this item opens, or null for an ordinary item.</summary>
    public Menu? SubMenu
    {
        get
        {
            var handle = NativeMethods.wxsharp_menuitem_get_submenu(Handle);
            return handle == 0 ? null : Menu.Attach(handle);
        }
    }

    /// <summary>Sets the item's icon, or clears it when passed null.</summary>
    public void SetBitmap(Bitmap? bitmap) => NativeMethods.wxsharp_menuitem_set_bitmap(Handle, bitmap?.Handle ?? 0);

    /// <summary>Sets the label's accelerator suffix, replacing any existing one. Passing null removes it.
    /// This is the accelerator the platform draws and announces; it does not by itself register a shortcut -
    /// a menu item's accelerator is handled by wxWidgets, while a shortcut with no menu item needs an entry
    /// in the window's accelerator table.</summary>
    public void SetAccelerator(AcceleratorEntry? accelerator)
    {
        var label = Label;
        var tab = label.IndexOf('\t');
        var text = tab < 0 ? label : label[..tab];
        Label = accelerator is AcceleratorEntry entry ? $"{text}\t{entry}" : text;
    }

    internal void Detach() => _handle = 0;
}

/// <summary>A native <c>wxMenu</c>: a menu bar drop-down, a submenu, or a context menu shown with
/// <see cref="Window.PopupMenu"/>.</summary>
///
/// <remarks>
/// Build the menu once, then answer <see cref="WxEvents.UpdateUI"/> for each command rather than pushing
/// its state:
/// <code>
/// fileMenu.Append(playId, "&amp;Play\tSpace");
/// frame.Bind(WxEvents.UpdateUI, (_, e) =&gt; e.Enable(playlist.Count &gt; 0), playId);
/// frame.Bind(WxEvents.UpdateUI, (_, e) =&gt; e.Check(settings.Repeat), repeatId);
/// </code>
/// wxWidgets asks on idle and every time the menu is about to open, so the menu cannot go stale.
/// <see cref="Enable"/> and <see cref="Check"/> remain for the cases where pushing really is simpler.
/// </remarks>
public sealed class Menu : IDisposable
{
    private nint _handle;
    private bool _owned = true;
    private readonly List<MenuItem> _items = new();

    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Menu));

    public Menu(string title = "")
    {
        App.RequireCurrent();
        _handle = NativeMethods.wxsharp_menu_create();
        if (_handle == 0) throw new InvalidOperationException("wxWidgets failed to create the menu.");
        if (!string.IsNullOrEmpty(title)) Title = title;
    }

    private Menu(nint handle, bool owned) { _handle = handle; _owned = owned; }

    /// <summary>Wraps a menu wxWidgets already owns, such as one reached through <see cref="MenuItem.SubMenu"/>.</summary>
    internal static Menu Attach(nint handle) => new(handle, owned: false);

    /// <summary>The menu's own title. On a menu bar this is the top-level label; on a context menu it is an
    /// optional heading.</summary>
    public unsafe string Title
    {
        get
        {
            var length = NativeMethods.wxsharp_menu_get_title(Handle, null, 0);
            if (length <= 0) return string.Empty;
            var buffer = new byte[length + 1];
            fixed (byte* p = buffer) _ = NativeMethods.wxsharp_menu_get_title(Handle, p, buffer.Length);
            return Utf8String.Decode(buffer, length);
        }
        set => NativeMethods.wxsharp_menu_set_title(Handle, value ?? string.Empty);
    }

    /// <summary>How many entries the menu holds, separators included.</summary>
    public int Count => NativeMethods.wxsharp_menu_count(Handle);

    /// <summary>The item at <paramref name="index"/>.</summary>
    public MenuItem this[int index]
    {
        get
        {
            var handle = NativeMethods.wxsharp_menu_item_at(Handle, index);
            if (handle == 0) throw new ArgumentOutOfRangeException(nameof(index));
            return Track(handle);
        }
    }

    /// <summary>Adds an item. Put a <c>&amp;</c> before the mnemonic letter and an accelerator after a tab -
    /// <c>"&amp;Open...\tCtrl+O"</c> - so the platform draws and announces both.</summary>
    public MenuItem Append(int id, string text, string help = "", MenuItemKind kind = MenuItemKind.Normal)
        => Track(NativeMethods.wxsharp_menu_append(Handle, id, text, help, (int)kind));

    /// <summary>Adds a check item, which toggles on its own.</summary>
    public MenuItem AppendCheckItem(int id, string text, string help = "")
        => Append(id, text, help, MenuItemKind.Check);

    /// <summary>Adds a radio item. Consecutive radio items form one mutually exclusive group.</summary>
    public MenuItem AppendRadioItem(int id, string text, string help = "")
        => Append(id, text, help, MenuItemKind.Radio);

    /// <summary>Adds a submenu. The parent menu takes ownership of <paramref name="submenu"/>; do not dispose
    /// it separately.</summary>
    public MenuItem AppendSubMenu(Menu submenu, string text, string help = "")
    {
        ArgumentNullException.ThrowIfNull(submenu);
        var item = Track(NativeMethods.wxsharp_menu_append_submenu(Handle, WindowId.Any, text, submenu.Handle, help));
        submenu._owned = false;
        return item;
    }

    public MenuItem AppendSeparator() => Track(NativeMethods.wxsharp_menu_append_separator(Handle));

    /// <summary>Inserts an item at <paramref name="position"/>, shifting the rest down.</summary>
    public MenuItem Insert(int position, int id, string text, string help = "",
        MenuItemKind kind = MenuItemKind.Normal)
        => Track(NativeMethods.wxsharp_menu_insert(Handle, position, id, text, help, (int)kind));

    public MenuItem InsertSubMenu(int position, Menu submenu, string text, string help = "")
    {
        ArgumentNullException.ThrowIfNull(submenu);
        var item = Track(NativeMethods.wxsharp_menu_insert_submenu(Handle, position, WindowId.Any, text,
            submenu.Handle, help));
        submenu._owned = false;
        return item;
    }

    public MenuItem InsertSeparator(int position)
        => Track(NativeMethods.wxsharp_menu_insert_separator(Handle, position));

    /// <summary>Finds the item with <paramref name="id"/>, searching submenus too. Null when there is none.</summary>
    public MenuItem? FindItem(int id)
    {
        var handle = NativeMethods.wxsharp_menu_find_item(Handle, id);
        return handle == 0 ? null : Track(handle);
    }

    /// <summary>Detaches an item without deleting it, so it can be inserted elsewhere.</summary>
    public bool Remove(MenuItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (!NativeMethods.wxsharp_menu_remove(Handle, item.Handle)) return false;
        _items.Remove(item);
        return true;
    }

    /// <summary>Removes and deletes an item, along with any submenu it owns.</summary>
    public bool Delete(MenuItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        var handle = item.Handle;
        if (!NativeMethods.wxsharp_menu_delete(Handle, handle)) return false;
        _items.Remove(item);
        item.Detach();
        return true;
    }

    /// <summary>Deletes the item with <paramref name="id"/> wherever it is in this menu or its submenus.</summary>
    public bool Delete(int id) => FindItem(id) is MenuItem item && Delete(item);

    /// <summary>Enables or disables an item by command ID. See the note on <see cref="MenuItem.Enabled"/>:
    /// answering <see cref="WxEvents.UpdateUI"/> is usually the better way to keep this correct.</summary>
    public void Enable(int id, bool enable = true) => NativeMethods.wxsharp_menu_enable(Handle, id, enable);

    /// <summary>Ticks or unticks an item by command ID. Answering <see cref="WxEvents.UpdateUI"/> is
    /// usually better.</summary>
    public void Check(int id, bool check = true) => NativeMethods.wxsharp_menu_check(Handle, id, check);

    public bool IsChecked(int id) => NativeMethods.wxsharp_menu_is_checked(Handle, id);

    private MenuItem Track(nint handle)
    {
        foreach (var existing in _items)
            if (existing.Handle == handle) return existing;
        var item = new MenuItem(handle);
        _items.Add(item);
        return item;
    }

    /// <summary>Hands the native menu to a new owner - a menu bar, or a parent menu.</summary>
    internal nint TransferOwnership() { var value = Handle; _owned = false; return value; }

    public void Dispose()
    {
        // Only a menu still owned by managed code is destroyed here: once it has been attached to a menu bar
        // or a parent menu, wxWidgets owns it.
        if (_handle != 0 && _owned) NativeMethods.wxsharp_menu_destroy(_handle);
        _handle = 0;
        foreach (var item in _items) item.Detach();
        _items.Clear();
    }
}

/// <summary>A frame's menu bar. Menus added to it belong to it.</summary>
public sealed class MenuBar : IDisposable
{
    private nint _handle;
    private bool _owned = true;
    private readonly List<Menu> _menus = new();

    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(MenuBar));

    public MenuBar()
    {
        App.RequireCurrent();
        _handle = NativeMethods.wxsharp_menubar_create();
        if (_handle == 0) throw new InvalidOperationException("wxWidgets failed to create the menu bar.");
    }

    /// <summary>How many top-level menus the bar holds.</summary>
    public int Count => NativeMethods.wxsharp_menubar_count(Handle);

    /// <summary>Adds a top-level menu. The bar takes ownership of <paramref name="menu"/>.</summary>
    public bool Append(Menu menu, string title)
    {
        ArgumentNullException.ThrowIfNull(menu);
        if (!NativeMethods.wxsharp_menubar_append(Handle, menu.Handle, title)) return false;
        _ = menu.TransferOwnership();
        _menus.Add(menu);
        return true;
    }

    /// <summary>Inserts a top-level menu at <paramref name="position"/>.</summary>
    public bool Insert(int position, Menu menu, string title)
    {
        ArgumentNullException.ThrowIfNull(menu);
        if (!NativeMethods.wxsharp_menubar_insert(Handle, position, menu.Handle, title)) return false;
        _ = menu.TransferOwnership();
        _menus.Insert(Math.Clamp(position, 0, _menus.Count), menu);
        return true;
    }

    /// <summary>Detaches the menu at <paramref name="position"/> and returns it. The caller then owns it.</summary>
    public Menu? Remove(int position)
    {
        var handle = NativeMethods.wxsharp_menubar_remove(Handle, position);
        if (handle == 0) return null;
        for (var i = 0; i < _menus.Count; ++i)
            if (_menus[i].Handle == handle) { var menu = _menus[i]; _menus.RemoveAt(i); return menu; }
        return Menu.Attach(handle);
    }

    /// <summary>The top-level menu at <paramref name="position"/>.</summary>
    public Menu this[int position]
    {
        get
        {
            var handle = NativeMethods.wxsharp_menubar_menu_at(Handle, position);
            if (handle == 0) throw new ArgumentOutOfRangeException(nameof(position));
            foreach (var menu in _menus)
                if (menu.Handle == handle) return menu;
            return Menu.Attach(handle);
        }
    }

    /// <summary>Enables or disables a whole top-level menu.</summary>
    public void EnableTop(int position, bool enable = true)
        => NativeMethods.wxsharp_menubar_enable_top(Handle, position, enable);

    /// <summary>The label of a top-level menu.</summary>
    public unsafe string GetLabelTop(int position)
    {
        var length = NativeMethods.wxsharp_menubar_get_label_top(Handle, position, null, 0);
        if (length <= 0) return string.Empty;
        var buffer = new byte[length + 1];
        fixed (byte* p = buffer) _ = NativeMethods.wxsharp_menubar_get_label_top(Handle, position, p, buffer.Length);
        return Utf8String.Decode(buffer, length);
    }

    public void SetLabelTop(int position, string label)
        => NativeMethods.wxsharp_menubar_set_label_top(Handle, position, label ?? string.Empty);

    /// <summary>Finds an item by command ID anywhere in the bar, including inside submenus.</summary>
    public MenuItem? FindItem(int id)
    {
        var handle = NativeMethods.wxsharp_menubar_find_item(Handle, id);
        return handle == 0 ? null : new MenuItem(handle);
    }

    internal nint TransferOwnership() { var value = Handle; _owned = false; return value; }

    public void Dispose()
    {
        if (_handle != 0 && _owned) NativeMethods.wxsharp_menubar_destroy(_handle);
        _handle = 0;
        _menus.Clear();
    }
}

public class StatusBar : Control
{
    public StatusBar(Frame frame, int fields = 1) : base(frame, WindowId.Any)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(fields);
        Initialize(NativeMethods.wxsharp_statusbar_create(frame.Handle, fields, Token));
    }
    public void SetText(string text, int field = 0) => NativeMethods.wxsharp_statusbar_set_text(Handle, text, field);
    public unsafe string GetText(int field = 0)
    {
        var length = NativeMethods.wxsharp_statusbar_get_text(Handle, field, null, 0); if (length <= 0) return string.Empty;
        var bytes = new byte[length + 1]; fixed (byte* buffer = bytes) _ = NativeMethods.wxsharp_statusbar_get_text(Handle, field, buffer, bytes.Length);
        return Utf8String.Decode(bytes, length);
    }
}

public class ToolBar : Control
{
    /// <summary>The pointer moved onto a tool, or off every tool (with an ID of -1).</summary>
    public event EventHandler<CommandEventArgs> ToolEntered
    {
        add => AddHandler(WxEvents.ToolEntered, value);
        remove => RemoveHandler(WxEvents.ToolEntered, value);
    }

    public event EventHandler<CommandEventArgs> ToolRightClicked
    {
        add => AddHandler(WxEvents.ToolRightClicked, value);
        remove => RemoveHandler(WxEvents.ToolRightClicked, value);
    }

    /// <summary>A dropdown tool arrow was pressed.</summary>
    public event EventHandler<CommandEventArgs> ToolDropDown
    {
        add => AddHandler(WxEvents.ToolDropDown, value);
        remove => RemoveHandler(WxEvents.ToolDropDown, value);
    }

    public ToolBar(Frame frame) : base(frame, WindowId.Any)
        => Initialize(NativeMethods.wxsharp_toolbar_create(frame.Handle, Token));
    public void AddTool(int id, string label, string help = "", MenuItemKind kind = MenuItemKind.Normal)
        => NativeMethods.wxsharp_toolbar_add_tool(Handle, id, label, help, (int)kind);
    public void AddSeparator() => NativeMethods.wxsharp_toolbar_add_separator(Handle);
    public void Realize() => NativeMethods.wxsharp_toolbar_realize(Handle);
    public void EnableTool(int id, bool enable = true) => NativeMethods.wxsharp_toolbar_enable(Handle, id, enable);
    public void ToggleTool(int id, bool toggle = true) => NativeMethods.wxsharp_toolbar_toggle(Handle, id, toggle);
}
