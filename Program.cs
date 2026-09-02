using System;
using System.Windows;
using Winboard;

class Program
{
    public static Winboard.ViewModel.UI uI;

    [STAThread]
    static void Main(string[] args)
    {
        Application app = new();
        Winboard.ViewModel.UI w = new();
        uI = w;

        Gettext.intextfield();

        app.Run(w);

       
    }
}



