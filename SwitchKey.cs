using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

class WindowSelector : Form {

  private ComboBox searchBox; 
  private List<ProcessInfo> processInfo = new List<ProcessInfo>();
  private string lastSearchTerm;

  public WindowSelector() {
    this.ClientSize = new Size(600, 190);
    this.Text = "Window Selector";
    this.StartPosition = FormStartPosition.CenterScreen;
    this.lastSearchTerm = String.Empty;

    this.searchBox = new ComboBox();
    this.searchBox.Dock = System.Windows.Forms.DockStyle.Fill;
    this.searchBox.DropDownStyle = ComboBoxStyle.Simple;
    this.searchBox.Font = new Font(new FontFamily("Tahoma"), 18);
    this.searchBox.KeyUp += new KeyEventHandler(HandleSearch);
    this.searchBox.BackColor = Color.YellowGreen; //Moccasin;
    this.Controls.Add(this.searchBox);

    this.ResumeLayout(false);
    this.PerformLayout();
    this.FormBorderStyle = FormBorderStyle.None;
    SetStyle(ControlStyles.SupportsTransparentBackColor, true);
    this.BackColor = Color.Transparent;
    this.Opacity = 0.85;

    Keys key = Keys.L & ~Keys.Control & ~ Keys.Shift & ~Keys.Alt;
    Hotkeys.RegisterHotKey((IntPtr)this.Handle, this.GetHashCode(),
      (uint) Hotkeys.MOD_CONTROL, (uint) key);
  }

  protected override void WndProc(ref Message m) {
    base.WndProc(ref m);
    
    if(m.Msg == Hotkeys.WM_HOTKEY) {
      GetProcessInfoList(Process.GetProcesses());
      this.searchBox.Items.Clear();
      foreach(ProcessInfo p in processInfo) {
        this.searchBox.Items.Add(p);
      }
      this.searchBox.Text = "";
      this.Visible = true;
      this.Focus();
      this.Activate();
    }
  }

  private void GetProcessInfoList(Process[] processes) {
    this.processInfo.Clear();
    foreach(Process p in processes) {
      processInfo.Add(new ProcessInfo(p));
    } 
  }

  protected void HandleSearch(object sender, KeyEventArgs e) {
    if(e.KeyCode == Keys.Enter) {
      HandleFocus(sender, e);
      return;
    } else if(e.KeyCode == Keys.Escape) {
      this.Visible = false;
      return;
    }

    string matchText = searchBox.Text;
    if(matchText == this.lastSearchTerm) {
      return;
    } else {
      this.lastSearchTerm = matchText;
    }
    var matchRegex = new Regex(Regex.Escape(matchText),
                                RegexOptions.IgnoreCase);
    
    var matches = new List<ProcessInfo>();
    foreach(ProcessInfo p in this.processInfo) {
      var match = matchRegex.Match(p.ToString());
      if(match.Length > 0) {
        matches.Add(p);
      }
    }

    this.searchBox.Items.Clear();
    foreach(ProcessInfo processInfo in matches) {
      this.searchBox.Items.Add(processInfo);
    }
    if(matches.Count > 1) {
      this.searchBox.Select(matchText.Length, 0);
    } else {
      if(matches.Count == 1) {
        this.searchBox.Text = matches[0].ToString();
        System.Threading.Thread.Sleep(200);
        HandleFocus(sender, e);
      }
      this.searchBox.Select(matchText.Length, this.searchBox.Text.Length);
    }
  }

  protected void HandleFocus(object sender, EventArgs e) {
    var selected = (ProcessInfo)this.searchBox.SelectedItem;
    if(selected != null) {
      this.Visible = false;
      var hWnd = selected.Process.MainWindowHandle;
      WindowManager.ShowWindow(hWnd, WindowManager.SW_RESTORE);
      WindowManager.BringWindowToTop(hWnd);
      WindowManager.SetForegroundWindow(hWnd); 
    }
  }
};

class ProcessInfo {
  public Process Process { get; private set; }

  public ProcessInfo(Process p) {
    this.Process = p;
  }

  public override String ToString() {
    return Process.ProcessName + " - " + Process.MainWindowTitle;
  }
}

class Test {
  
  [STAThread] 
  public static void Main(string[] args) {
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
    var form = new WindowSelector();
    form.Visible = false;
    Application.Run();
  }

}

class WindowManager {
  public static int SW_RESTORE = 9;

  [DllImport("user32")]
  public extern static int BringWindowToTop (IntPtr hWnd);

  [DllImport("user32")]
  public extern static int SetForegroundWindow (IntPtr hWnd);

  [DllImport("user32")]
  public extern static int ShowWindow(IntPtr hWnd, int nCmdShow);
}
