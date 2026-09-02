using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using WindowsInput;
namespace Winboard.stream
{
    public class Printtext
    {
        InputSimulator sime = new();
        public void PrintText(string text)
        {
            sime.Keyboard.TextEntry(text);
        }
    }
}
