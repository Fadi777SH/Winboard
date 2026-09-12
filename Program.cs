using System;
using Winboard;

class Program
{
    public static Winboard.ViewModel.UI WPFWindow;
    

    [STAThread]
    static void Main(string[] args)
    {
        System.Windows.Application app = new();

        WPFWindow = new();
        Gettext.intextfield();
        app.Run(WPFWindow);

       

    }
}



