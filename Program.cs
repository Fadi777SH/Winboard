using System;
using Winboard;

class Program
{
    public static Winboard.ViewModel.UI WPFWindow;
    private const int WM_HOTKEY = 0x0312;
    

    [STAThread]
    static void Main(string[] args)
    {
        System.Windows.Application app = new();

        WPFWindow = new();

        Gettext.intextfield();

        app.Run(WPFWindow);
       

    }
}



