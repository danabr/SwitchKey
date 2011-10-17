using System;
using System.Runtime.InteropServices;

class Hotkeys
{
  public static int MOD_ALT = 0x1;
  public static int MOD_CONTROL = 0x2;
  public static int MOD_SHIFT = 0x4;
  public static int MOD_WIN = 0x8;
  public static int WM_HOTKEY = 0x312;

  [DllImport("user32.dll", SetLastError=true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool RegisterHotKey(IntPtr hWnd, int id,
                                            uint fsModifiers, uint vk);
}
