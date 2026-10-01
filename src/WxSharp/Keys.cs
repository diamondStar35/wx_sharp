namespace WxSharp;

/// <summary>Every key code wxWidgets names, as reported by <see cref="KeyEventArgs.Code"/>.</summary>
///
/// <remarks>
/// Printable keys report their character instead, so <c>(int)Key.Space</c> is 32 and a letter key arrives as
/// its uppercase ASCII value on a key-down event and as the typed character on a <see cref="WxEvents.Char"/>
/// event. Codes above 300 are the keys with no character of their own.
///
/// These values are generated from the wxWidgets headers, not transcribed, because a wrong code here would
/// silently bind a shortcut to the wrong key.
/// </remarks>
public enum Key
{
    // ---- ASCII control codes ----
    /// <summary>No key. Reported when an event carries no key code at all.</summary>
    None = 0,
    /// <summary>Ctrl+A (<c>WXK_CONTROL_A</c>).</summary>
    ControlA = 1,
    /// <summary>Ctrl+B (<c>WXK_CONTROL_B</c>).</summary>
    ControlB = 2,
    /// <summary>Ctrl+C (<c>WXK_CONTROL_C</c>).</summary>
    ControlC = 3,
    /// <summary>Ctrl+D (<c>WXK_CONTROL_D</c>).</summary>
    ControlD = 4,
    /// <summary>Ctrl+E (<c>WXK_CONTROL_E</c>).</summary>
    ControlE = 5,
    /// <summary>Ctrl+F (<c>WXK_CONTROL_F</c>).</summary>
    ControlF = 6,
    /// <summary>Ctrl+G (<c>WXK_CONTROL_G</c>).</summary>
    ControlG = 7,
    /// <summary>Backspace.</summary>
    Back = 8,
    /// <summary>Ctrl+H (<c>WXK_CONTROL_H</c>); shares code 8 with <see cref="Back"/>.</summary>
    ControlH = 8,
    /// <summary>Ctrl+I (<c>WXK_CONTROL_I</c>); shares code 9 with <see cref="Tab"/>.</summary>
    ControlI = 9,
    /// <summary>The Tab key (<c>WXK_TAB</c>).</summary>
    Tab = 9,
    /// <summary>Ctrl+J (<c>WXK_CONTROL_J</c>), the ASCII line-feed code.</summary>
    ControlJ = 10,
    /// <summary>Ctrl+K (<c>WXK_CONTROL_K</c>).</summary>
    ControlK = 11,
    /// <summary>Ctrl+L (<c>WXK_CONTROL_L</c>).</summary>
    ControlL = 12,
    /// <summary>Ctrl+M (<c>WXK_CONTROL_M</c>); shares code 13 with <see cref="Enter"/>.</summary>
    ControlM = 13,
    /// <summary>Return or Enter on the main keyboard. <see cref="NumpadEnter"/> is the keypad one.</summary>
    Enter = 13,
    /// <summary>Ctrl+N (<c>WXK_CONTROL_N</c>).</summary>
    ControlN = 14,
    /// <summary>Ctrl+O (<c>WXK_CONTROL_O</c>).</summary>
    ControlO = 15,
    /// <summary>Ctrl+P (<c>WXK_CONTROL_P</c>).</summary>
    ControlP = 16,
    /// <summary>Ctrl+Q (<c>WXK_CONTROL_Q</c>).</summary>
    ControlQ = 17,
    /// <summary>Ctrl+R (<c>WXK_CONTROL_R</c>).</summary>
    ControlR = 18,
    /// <summary>Ctrl+S (<c>WXK_CONTROL_S</c>).</summary>
    ControlS = 19,
    /// <summary>Ctrl+T (<c>WXK_CONTROL_T</c>).</summary>
    ControlT = 20,
    /// <summary>Ctrl+U (<c>WXK_CONTROL_U</c>).</summary>
    ControlU = 21,
    /// <summary>Ctrl+V (<c>WXK_CONTROL_V</c>).</summary>
    ControlV = 22,
    /// <summary>Ctrl+W (<c>WXK_CONTROL_W</c>).</summary>
    ControlW = 23,
    /// <summary>Ctrl+X (<c>WXK_CONTROL_X</c>).</summary>
    ControlX = 24,
    /// <summary>Ctrl+Y (<c>WXK_CONTROL_Y</c>).</summary>
    ControlY = 25,
    /// <summary>Ctrl+Z (<c>WXK_CONTROL_Z</c>).</summary>
    ControlZ = 26,
    /// <summary>The Escape key (<c>WXK_ESCAPE</c>).</summary>
    Escape = 27,
    /// <summary>The Delete key (<c>WXK_DELETE</c>). <see cref="Back"/> is Backspace.</summary>
    Delete = 127,

    // ---- Modifiers, navigation and editing ----
    /// <summary>Lowest code in the non-character range (<c>WXK_START</c>); the keys below are offsets from it.</summary>
    Start = 300,
    /// <summary>The left mouse button (<c>WXK_LBUTTON</c>).</summary>
    LeftButton = 301,
    /// <summary>The right mouse button (<c>WXK_RBUTTON</c>).</summary>
    RightButton = 302,
    /// <summary>The Cancel key (<c>WXK_CANCEL</c>), typically Ctrl+Break.</summary>
    Cancel = 303,
    /// <summary>The middle mouse button (<c>WXK_MBUTTON</c>).</summary>
    MiddleButton = 304,
    /// <summary>The Clear key (<c>WXK_CLEAR</c>).</summary>
    Clear = 305,
    /// <summary>The keypad 5 with Num Lock off (<c>WXK_NUMPAD_CENTER</c>); shares code 305 with <see cref="Clear"/>.</summary>
    NumpadCenter = 305,
    /// <summary>The Shift key (<c>WXK_SHIFT</c>).</summary>
    Shift = 306,
    /// <summary>The Alt key (<c>WXK_ALT</c>).</summary>
    Alt = 307,
    /// <summary>The platform command modifier: Control on Windows and GTK, Command on macOS.</summary>
    Command = 308,
    /// <summary>Control on Windows and GTK; Command on macOS. <see cref="RawControl"/> is always the physical Control key.</summary>
    Control = 308,
    /// <summary>The physical Control key, which differs from <see cref="Control"/> only on macOS.</summary>
    RawControl = 308,
    /// <summary>The Alt key. The context-menu key is <see cref="WindowsMenu"/>.</summary>
    Menu = 309,
    /// <summary>The Pause key (<c>WXK_PAUSE</c>).</summary>
    Pause = 310,
    /// <summary>The Caps Lock key (<c>WXK_CAPITAL</c>).</summary>
    CapsLock = 311,
    /// <summary>The End key (<c>WXK_END</c>).</summary>
    End = 312,
    /// <summary>The Home key (<c>WXK_HOME</c>).</summary>
    Home = 313,
    /// <summary>The Left arrow key (<c>WXK_LEFT</c>).</summary>
    Left = 314,
    /// <summary>The Up arrow key (<c>WXK_UP</c>).</summary>
    Up = 315,
    /// <summary>The Right arrow key (<c>WXK_RIGHT</c>).</summary>
    Right = 316,
    /// <summary>The Down arrow key (<c>WXK_DOWN</c>).</summary>
    Down = 317,
    /// <summary>The Select key (<c>WXK_SELECT</c>).</summary>
    Select = 318,
    /// <summary>The Print key (<c>WXK_PRINT</c>). <see cref="PrintScreen"/> is Print Screen.</summary>
    Print = 319,
    /// <summary>The Execute key (<c>WXK_EXECUTE</c>).</summary>
    Execute = 320,
    /// <summary>The Print Screen key (<c>WXK_SNAPSHOT</c>).</summary>
    PrintScreen = 321,
    /// <summary>The Insert key (<c>WXK_INSERT</c>).</summary>
    Insert = 322,
    /// <summary>The Help key (<c>WXK_HELP</c>).</summary>
    Help = 323,

    // ---- Numeric keypad digits and operators ----
    /// <summary>The keypad 0 key (<c>WXK_NUMPAD0</c>).</summary>
    Numpad0 = 324,
    /// <summary>The keypad 1 key (<c>WXK_NUMPAD1</c>).</summary>
    Numpad1 = 325,
    /// <summary>The keypad 2 key (<c>WXK_NUMPAD2</c>).</summary>
    Numpad2 = 326,
    /// <summary>The keypad 3 key (<c>WXK_NUMPAD3</c>).</summary>
    Numpad3 = 327,
    /// <summary>The keypad 4 key (<c>WXK_NUMPAD4</c>).</summary>
    Numpad4 = 328,
    /// <summary>The keypad 5 key (<c>WXK_NUMPAD5</c>).</summary>
    Numpad5 = 329,
    /// <summary>The keypad 6 key (<c>WXK_NUMPAD6</c>).</summary>
    Numpad6 = 330,
    /// <summary>The keypad 7 key (<c>WXK_NUMPAD7</c>).</summary>
    Numpad7 = 331,
    /// <summary>The keypad 8 key (<c>WXK_NUMPAD8</c>).</summary>
    Numpad8 = 332,
    /// <summary>The keypad 9 key (<c>WXK_NUMPAD9</c>).</summary>
    Numpad9 = 333,
    /// <summary>The main (non-keypad) multiply key (<c>WXK_MULTIPLY</c>). <see cref="NumpadMultiply"/> is the keypad one.</summary>
    Multiply = 334,
    /// <summary>The main (non-keypad) add key (<c>WXK_ADD</c>). <see cref="NumpadAdd"/> is the keypad one.</summary>
    Add = 335,
    /// <summary>The separator key (<c>WXK_SEPARATOR</c>).</summary>
    Separator = 336,
    /// <summary>The main (non-keypad) subtract key (<c>WXK_SUBTRACT</c>). <see cref="NumpadSubtract"/> is the keypad one.</summary>
    Subtract = 337,
    /// <summary>The main (non-keypad) decimal key (<c>WXK_DECIMAL</c>). <see cref="NumpadDecimal"/> is the keypad one.</summary>
    Decimal = 338,
    /// <summary>The main (non-keypad) divide key (<c>WXK_DIVIDE</c>). <see cref="NumpadDivide"/> is the keypad one.</summary>
    Divide = 339,

    // ---- Function keys ----
    /// <summary>The F1 function key (<c>WXK_F1</c>).</summary>
    F1 = 340,
    /// <summary>The F2 function key (<c>WXK_F2</c>).</summary>
    F2 = 341,
    /// <summary>The F3 function key (<c>WXK_F3</c>).</summary>
    F3 = 342,
    /// <summary>The F4 function key (<c>WXK_F4</c>).</summary>
    F4 = 343,
    /// <summary>The F5 function key (<c>WXK_F5</c>).</summary>
    F5 = 344,
    /// <summary>The F6 function key (<c>WXK_F6</c>).</summary>
    F6 = 345,
    /// <summary>The F7 function key (<c>WXK_F7</c>).</summary>
    F7 = 346,
    /// <summary>The F8 function key (<c>WXK_F8</c>).</summary>
    F8 = 347,
    /// <summary>The F9 function key (<c>WXK_F9</c>).</summary>
    F9 = 348,
    /// <summary>The F10 function key (<c>WXK_F10</c>).</summary>
    F10 = 349,
    /// <summary>The F11 function key (<c>WXK_F11</c>).</summary>
    F11 = 350,
    /// <summary>The F12 function key (<c>WXK_F12</c>).</summary>
    F12 = 351,
    /// <summary>The F13 function key (<c>WXK_F13</c>).</summary>
    F13 = 352,
    /// <summary>The F14 function key (<c>WXK_F14</c>).</summary>
    F14 = 353,
    /// <summary>The F15 function key (<c>WXK_F15</c>).</summary>
    F15 = 354,
    /// <summary>The F16 function key (<c>WXK_F16</c>).</summary>
    F16 = 355,
    /// <summary>The F17 function key (<c>WXK_F17</c>).</summary>
    F17 = 356,
    /// <summary>The F18 function key (<c>WXK_F18</c>).</summary>
    F18 = 357,
    /// <summary>The F19 function key (<c>WXK_F19</c>).</summary>
    F19 = 358,
    /// <summary>The F20 function key (<c>WXK_F20</c>).</summary>
    F20 = 359,
    /// <summary>The F21 function key (<c>WXK_F21</c>).</summary>
    F21 = 360,
    /// <summary>The F22 function key (<c>WXK_F22</c>).</summary>
    F22 = 361,
    /// <summary>The F23 function key (<c>WXK_F23</c>).</summary>
    F23 = 362,
    /// <summary>The F24 function key (<c>WXK_F24</c>).</summary>
    F24 = 363,

    // ---- Locks, paging and the extended keypad ----
    /// <summary>The Num Lock key (<c>WXK_NUMLOCK</c>).</summary>
    NumLock = 364,
    /// <summary>The Scroll Lock key (<c>WXK_SCROLL</c>).</summary>
    ScrollLock = 365,
    /// <summary>The Page Up key (<c>WXK_PAGEUP</c>).</summary>
    PageUp = 366,
    /// <summary>The Page Down key (<c>WXK_PAGEDOWN</c>).</summary>
    PageDown = 367,
    /// <summary>The keypad Space key (<c>WXK_NUMPAD_SPACE</c>).</summary>
    NumpadSpace = 368,
    /// <summary>The keypad Tab key (<c>WXK_NUMPAD_TAB</c>).</summary>
    NumpadTab = 369,
    /// <summary>The keypad Enter key (<c>WXK_NUMPAD_ENTER</c>). <see cref="Enter"/> is the main one.</summary>
    NumpadEnter = 370,
    /// <summary>The keypad F1 key (<c>WXK_NUMPAD_F1</c>).</summary>
    NumpadF1 = 371,
    /// <summary>The keypad F2 key (<c>WXK_NUMPAD_F2</c>).</summary>
    NumpadF2 = 372,
    /// <summary>The keypad F3 key (<c>WXK_NUMPAD_F3</c>).</summary>
    NumpadF3 = 373,
    /// <summary>The keypad F4 key (<c>WXK_NUMPAD_F4</c>).</summary>
    NumpadF4 = 374,
    /// <summary>The keypad Home key (<c>WXK_NUMPAD_HOME</c>).</summary>
    NumpadHome = 375,
    /// <summary>The keypad Left arrow key (<c>WXK_NUMPAD_LEFT</c>).</summary>
    NumpadLeft = 376,
    /// <summary>The keypad Up arrow key (<c>WXK_NUMPAD_UP</c>).</summary>
    NumpadUp = 377,
    /// <summary>The keypad Right arrow key (<c>WXK_NUMPAD_RIGHT</c>).</summary>
    NumpadRight = 378,
    /// <summary>The keypad Down arrow key (<c>WXK_NUMPAD_DOWN</c>).</summary>
    NumpadDown = 379,
    /// <summary>The keypad Page Up key (<c>WXK_NUMPAD_PAGEUP</c>).</summary>
    NumpadPageUp = 380,
    /// <summary>The keypad Page Down key (<c>WXK_NUMPAD_PAGEDOWN</c>).</summary>
    NumpadPageDown = 381,
    /// <summary>The keypad End key (<c>WXK_NUMPAD_END</c>).</summary>
    NumpadEnd = 382,
    /// <summary>The keypad Begin key (<c>WXK_NUMPAD_BEGIN</c>), the 5 with Num Lock off on some platforms.</summary>
    NumpadBegin = 383,
    /// <summary>The keypad Insert key (<c>WXK_NUMPAD_INSERT</c>).</summary>
    NumpadInsert = 384,
    /// <summary>The keypad Delete key (<c>WXK_NUMPAD_DELETE</c>).</summary>
    NumpadDelete = 385,
    /// <summary>The keypad equals key (<c>WXK_NUMPAD_EQUAL</c>).</summary>
    NumpadEqual = 386,
    /// <summary>The keypad multiply key (<c>WXK_NUMPAD_MULTIPLY</c>). <see cref="Multiply"/> is the main one.</summary>
    NumpadMultiply = 387,
    /// <summary>The keypad add key (<c>WXK_NUMPAD_ADD</c>). <see cref="Add"/> is the main one.</summary>
    NumpadAdd = 388,
    /// <summary>The keypad separator key (<c>WXK_NUMPAD_SEPARATOR</c>).</summary>
    NumpadSeparator = 389,
    /// <summary>The keypad subtract key (<c>WXK_NUMPAD_SUBTRACT</c>). <see cref="Subtract"/> is the main one.</summary>
    NumpadSubtract = 390,
    /// <summary>The keypad decimal key (<c>WXK_NUMPAD_DECIMAL</c>). <see cref="Decimal"/> is the main one.</summary>
    NumpadDecimal = 391,
    /// <summary>The keypad divide key (<c>WXK_NUMPAD_DIVIDE</c>). <see cref="Divide"/> is the main one.</summary>
    NumpadDivide = 392,
    /// <summary>The left Windows key (<c>WXK_WINDOWS_LEFT</c>).</summary>
    WindowsLeft = 393,
    /// <summary>The right Windows key (<c>WXK_WINDOWS_RIGHT</c>).</summary>
    WindowsRight = 394,

    // ---- Windows and platform keys ----
    /// <summary>The context-menu (Application) key (<c>WXK_WINDOWS_MENU</c>).</summary>
    WindowsMenu = 395,
    /// <summary>Platform-defined extra keys, for hardware wxWidgets does not name.</summary>
    Special1 = 397,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL2</c>). See <see cref="Special1"/>.</summary>
    Special2 = 398,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL3</c>). See <see cref="Special1"/>.</summary>
    Special3 = 399,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL4</c>). See <see cref="Special1"/>.</summary>
    Special4 = 400,

    // ---- Platform-defined extras ----
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL5</c>). See <see cref="Special1"/>.</summary>
    Special5 = 401,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL6</c>). See <see cref="Special1"/>.</summary>
    Special6 = 402,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL7</c>). See <see cref="Special1"/>.</summary>
    Special7 = 403,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL8</c>). See <see cref="Special1"/>.</summary>
    Special8 = 404,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL9</c>). See <see cref="Special1"/>.</summary>
    Special9 = 405,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL10</c>). See <see cref="Special1"/>.</summary>
    Special10 = 406,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL11</c>). See <see cref="Special1"/>.</summary>
    Special11 = 407,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL12</c>). See <see cref="Special1"/>.</summary>
    Special12 = 408,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL13</c>). See <see cref="Special1"/>.</summary>
    Special13 = 409,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL14</c>). See <see cref="Special1"/>.</summary>
    Special14 = 410,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL15</c>). See <see cref="Special1"/>.</summary>
    Special15 = 411,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL16</c>). See <see cref="Special1"/>.</summary>
    Special16 = 412,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL17</c>). See <see cref="Special1"/>.</summary>
    Special17 = 413,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL18</c>). See <see cref="Special1"/>.</summary>
    Special18 = 414,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL19</c>). See <see cref="Special1"/>.</summary>
    Special19 = 415,
    /// <summary>Platform-defined extra key (<c>WXK_SPECIAL20</c>). See <see cref="Special1"/>.</summary>
    Special20 = 416,
    /// <summary>The browser Back key (<c>WXK_BROWSER_BACK</c>).</summary>
    BrowserBack = 417,
    /// <summary>The browser Forward key (<c>WXK_BROWSER_FORWARD</c>).</summary>
    BrowserForward = 418,
    /// <summary>The browser Refresh key (<c>WXK_BROWSER_REFRESH</c>).</summary>
    BrowserRefresh = 419,
    /// <summary>The browser Stop key (<c>WXK_BROWSER_STOP</c>).</summary>
    BrowserStop = 420,

    // ---- Browser, volume and media keys ----
    /// <summary>The browser Search key (<c>WXK_BROWSER_SEARCH</c>).</summary>
    BrowserSearch = 421,
    /// <summary>The browser Favorites key (<c>WXK_BROWSER_FAVORITES</c>).</summary>
    BrowserFavorites = 422,
    /// <summary>The browser Home key (<c>WXK_BROWSER_HOME</c>).</summary>
    BrowserHome = 423,
    /// <summary>The Mute media key (<c>WXK_VOLUME_MUTE</c>).</summary>
    VolumeMute = 424,
    /// <summary>The Volume Down media key (<c>WXK_VOLUME_DOWN</c>).</summary>
    VolumeDown = 425,
    /// <summary>The Volume Up media key (<c>WXK_VOLUME_UP</c>).</summary>
    VolumeUp = 426,
    /// <summary>The Next Track media key (<c>WXK_MEDIA_NEXT_TRACK</c>).</summary>
    MediaNextTrack = 427,
    /// <summary>The Previous Track media key (<c>WXK_MEDIA_PREV_TRACK</c>).</summary>
    MediaPrevTrack = 428,
    /// <summary>The Stop media key (<c>WXK_MEDIA_STOP</c>).</summary>
    MediaStop = 429,
    /// <summary>The Play/Pause media key (<c>WXK_MEDIA_PLAY_PAUSE</c>).</summary>
    MediaPlayPause = 430,
    /// <summary>The Launch Mail key (<c>WXK_LAUNCH_MAIL</c>).</summary>
    LaunchMail = 431,
    /// <summary>Application launch key 0 (<c>WXK_LAUNCH_0</c>).</summary>
    Launch0 = 432,
    /// <summary>Application launch key 1 (<c>WXK_LAUNCH_1</c>).</summary>
    Launch1 = 433,
    /// <summary>Application launch key 2 (<c>WXK_LAUNCH_2</c>).</summary>
    Launch2 = 434,
    /// <summary>Application launch key 3 (<c>WXK_LAUNCH_3</c>).</summary>
    Launch3 = 435,
    /// <summary>Application launch key 4 (<c>WXK_LAUNCH_4</c>).</summary>
    Launch4 = 436,
    /// <summary>Application launch key 5 (<c>WXK_LAUNCH_5</c>).</summary>
    Launch5 = 437,
    /// <summary>Application launch key 6 (<c>WXK_LAUNCH_6</c>).</summary>
    Launch6 = 438,
    /// <summary>Application launch key 7 (<c>WXK_LAUNCH_7</c>).</summary>
    Launch7 = 439,
    /// <summary>Application launch key 8 (<c>WXK_LAUNCH_8</c>).</summary>
    Launch8 = 440,

    // ---- Application launch keys ----
    /// <summary>Application launch key 9 (<c>WXK_LAUNCH_9</c>).</summary>
    Launch9 = 441,
    /// <summary>Application launch key A (<c>WXK_LAUNCH_A</c>); shares code 442 with <see cref="LaunchApp1"/>.</summary>
    LaunchA = 442,
    /// <summary>The Launch Application 1 key (<c>WXK_LAUNCH_APP1</c>); shares code 442 with <see cref="LaunchA"/>.</summary>
    LaunchApp1 = 442,
    /// <summary>The Launch Application 2 key (<c>WXK_LAUNCH_APP2</c>); shares code 443 with <see cref="LaunchB"/>.</summary>
    LaunchApp2 = 443,
    /// <summary>Application launch key B (<c>WXK_LAUNCH_B</c>); shares code 443 with <see cref="LaunchApp2"/>.</summary>
    LaunchB = 443,
    /// <summary>Application launch key C (<c>WXK_LAUNCH_C</c>).</summary>
    LaunchC = 444,
    /// <summary>Application launch key D (<c>WXK_LAUNCH_D</c>).</summary>
    LaunchD = 445,
    /// <summary>Application launch key E (<c>WXK_LAUNCH_E</c>).</summary>
    LaunchE = 446,
    /// <summary>Application launch key F (<c>WXK_LAUNCH_F</c>).</summary>
    LaunchF = 447,

    // ---- Other ----
    /// <summary>The Space bar (<c>WXK_SPACE</c>), reported as its character code 32.</summary>
    Space = 32,
}
