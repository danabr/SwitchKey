using System;
using System.Runtime.InteropServices;

class WindowManager
{
  public static int SW_RESTORE = 9;

  [DllImport("user32")]
  public extern static int BringWindowToTop (IntPtr hWnd);

  [DllImport("user32")]
  public extern static int SetForegroundWindow (IntPtr hWnd);

  [DllImport("user32")]
  public extern static int ShowWindow(IntPtr hWnd, int nCmdShow);
}
