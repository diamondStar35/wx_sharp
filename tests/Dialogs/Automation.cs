using System.Runtime.InteropServices;

namespace WxSharp.Tests.Dialogs;

/// <summary>Reads this fixture's native controls without input or reflection-based COM.</summary>
internal static unsafe partial class Automation
{
    internal sealed record Element(int Type, string Name, string LegacyName, bool Focusable, int LegacyRole);

    internal static IReadOnlyList<Element> VisibleElements(nint window)
    {
        Marshal.ThrowExceptionForHR(CoInitializeEx(0, 0));
        nint automation = 0, root = 0, condition = 0, elements = 0;
        try
        {
            var classId = new Guid("ff48dba4-60ef-4201-aa87-54103eef594e");
            var interfaceId = new Guid("30cbe57d-d9d0-452a-ab13-7ac5ac4825ee");
            Marshal.ThrowExceptionForHR(CoCreateInstance(&classId, 0, 1, &interfaceId, &automation));
            // Fixed COM slots from IUIAutomation in UIAutomationClient.h.
            var fromHandle = (delegate* unmanaged[Stdcall]<nint, nint, nint*, int>)Table(automation)[6];
            var trueCondition = (delegate* unmanaged[Stdcall]<nint, nint*, int>)Table(automation)[21];
            Marshal.ThrowExceptionForHR(fromHandle(automation, window, &root));
            Marshal.ThrowExceptionForHR(trueCondition(automation, &condition));
            var findAll = (delegate* unmanaged[Stdcall]<nint, int, nint, nint*, int>)Table(root)[6];
            Marshal.ThrowExceptionForHR(findAll(root, 4, condition, &elements)); // TreeScope_Descendants.
            var length = (delegate* unmanaged[Stdcall]<nint, int*, int>)Table(elements)[3];
            var at = (delegate* unmanaged[Stdcall]<nint, int, nint*, int>)Table(elements)[4];
            var count = 0;
            Marshal.ThrowExceptionForHR(length(elements, &count));
            var result = new List<Element>();
            for (var index = 0; index < count; index++)
            {
                nint element = 0, legacy = 0;
                try
                {
                    Marshal.ThrowExceptionForHR(at(elements, index, &element));
                    if (Number(element, 38) != 0) continue; // CurrentIsOffscreen.
                    var legacyId = new Guid("828055ad-355b-4435-86d5-3b51c14a9b1b");
                    var pattern = (delegate* unmanaged[Stdcall]<nint, int, Guid*, nint*, int>)Table(element)[14];
                    var status = pattern(element, 10018, &legacyId, &legacy);
                    result.Add(new(Number(element, 21), Text(element, 23),
                        status < 0 || legacy == 0 ? string.Empty : Text(legacy, 7), Number(element, 27) != 0,
                        status < 0 || legacy == 0 ? 0 : Number(legacy, 10)));
                }
                finally { Release(legacy); Release(element); }
            }
            return result;
        }
        finally
        {
            Release(elements); Release(condition); Release(root); Release(automation);
            CoUninitialize();
        }
    }
    private static nint* Table(nint instance) => *(nint**)instance;
    private static int Number(nint instance, int slot)
    {
        var value = 0;
        Marshal.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, int*, int>)Table(instance)[slot])(instance, &value));
        return value;
    }
    private static string Text(nint instance, int slot)
    {
        nint value = 0;
        Marshal.ThrowExceptionForHR(((delegate* unmanaged[Stdcall]<nint, nint*, int>)Table(instance)[slot])(instance, &value));
        try { return value == 0 ? string.Empty : Marshal.PtrToStringBSTR(value); }
        finally { if (value != 0) Marshal.FreeBSTR(value); }
    }
    private static void Release(nint instance)
    {
        if (instance != 0) _ = ((delegate* unmanaged[Stdcall]<nint, uint>)Table(instance)[2])(instance);
    }
    [LibraryImport("ole32.dll")]
    private static partial int CoInitializeEx(nint reserved, uint mode);
    [LibraryImport("ole32.dll")]
    private static partial void CoUninitialize();
    [LibraryImport("ole32.dll")]
    private static partial int CoCreateInstance(Guid* classId, nint outer, uint context, Guid* interfaceId, nint* instance);
}
