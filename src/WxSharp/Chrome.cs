using System;

namespace WxSharp;

public enum MenuItemKind { Normal, Check, Radio }
[Flags]
public enum AcceleratorModifiers { None = 0, Alt = 1, Control = 2, Shift = 4, RawControl = 8 }
public readonly record struct Accelerator(AcceleratorModifiers Modifiers, int KeyCode, int CommandId);

public sealed class Menu : IDisposable
{
    private nint _handle;
    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(Menu));
    public Menu() { App.RequireCurrent(); _handle = NativeMethods.wxsharp_menu_create(); }
    public void Add(int id, string text, string help = "", MenuItemKind kind = MenuItemKind.Normal)
        => NativeMethods.wxsharp_menu_append(Handle, id, text, help, (int)kind);
    public void AddSeparator() => NativeMethods.wxsharp_menu_append_separator(Handle);
    public void Enable(int id, bool enable = true) => NativeMethods.wxsharp_menu_enable(Handle, id, enable);
    public void Check(int id, bool check = true) => NativeMethods.wxsharp_menu_check(Handle, id, check);
    public bool IsChecked(int id) => NativeMethods.wxsharp_menu_is_checked(Handle, id);
    internal nint TransferOwnership() { var value = Handle; _handle = 0; return value; }
    public void Dispose() { if (_handle != 0) NativeMethods.wxsharp_menu_destroy(_handle); _handle = 0; }
}

public sealed class MenuBar : IDisposable
{
    private nint _handle;
    internal nint Handle => _handle != 0 ? _handle : throw new ObjectDisposedException(nameof(MenuBar));
    public MenuBar() { App.RequireCurrent(); _handle = NativeMethods.wxsharp_menubar_create(); }
    public bool Add(Menu menu, string title)
    {
        ArgumentNullException.ThrowIfNull(menu);
        var nativeMenu = menu.Handle;
        var added = NativeMethods.wxsharp_menubar_append(Handle, nativeMenu, title);
        if (added) _ = menu.TransferOwnership();
        return added;
    }
    internal nint TransferOwnership() { var value = Handle; _handle = 0; return value; }
    public void Dispose() { if (_handle != 0) NativeMethods.wxsharp_menubar_destroy(_handle); _handle = 0; }
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
    public ToolBar(Frame frame) : base(frame, WindowId.Any)
        => Initialize(NativeMethods.wxsharp_toolbar_create(frame.Handle, Token));
    public void AddTool(int id, string label, string help = "", MenuItemKind kind = MenuItemKind.Normal)
        => NativeMethods.wxsharp_toolbar_add_tool(Handle, id, label, help, (int)kind);
    public void AddSeparator() => NativeMethods.wxsharp_toolbar_add_separator(Handle);
    public void Realize() => NativeMethods.wxsharp_toolbar_realize(Handle);
    public void EnableTool(int id, bool enable = true) => NativeMethods.wxsharp_toolbar_enable(Handle, id, enable);
    public void ToggleTool(int id, bool toggle = true) => NativeMethods.wxsharp_toolbar_toggle(Handle, id, toggle);
}
