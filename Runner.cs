using System;
using System.Windows.Forms;

class Runner
{
    [STAThread] 
    public static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      var form = new WindowSelector();
      form.Visible = false;
      Application.Run();
    }
}
