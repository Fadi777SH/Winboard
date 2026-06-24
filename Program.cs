using C_Panel.UserString;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Documents;
using System.Xaml;



class Run
{

    public static AutoResetEvent TakeInputFirst = new AutoResetEvent(true);
    public static AutoResetEvent CheckSpell = new AutoResetEvent(false);
    static async Task Main(string[] args)
    {
        BasicsInput basicsInput = new BasicsInput();
        SpellingCheckerBySingleWord spellingChecker = new SpellingCheckerBySingleWord();

        while (true)
        {
               
            string x = await Task.Run(() => basicsInput.GetFocuedElement(AutomationElement.FocusedElement));
            string xx = await Task.Run(() => spellingChecker.Correct(x));
            Console.Write($"\x1b[2K\r{x}||{xx}");
            Thread.Sleep(1000);

        }
    }

}

