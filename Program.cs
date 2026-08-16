
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using Winboard;
using Winboard.ViewModel;

class Program
{
    public static Winboard.ViewModel.UI uI;

    [STAThread]
    static void Main(string[] args)
    {
        Application app = new();

        Gettext.intextfield();
        Winboard.ViewModel.UI w = new();
        uI = w;

        app.Run(w);
        Gettext.UnsubscribePropertyChange();
    }
}



