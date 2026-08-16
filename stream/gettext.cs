using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using Winboard.ViewModel;

namespace Winboard
{
    public class Gettext
    {


        static Scale.Answersblock answersblock = new();
        static BasicsInput basicsInput = new();
        static Assembly _assembly = new();
        static Translation translation = new();
        static AutomationPropertyChangedEventHandler propChangeHandler;

        static public  void intextfield()
        {

            AutomationElement e = AutomationElement.FocusedElement;
            if (e == null) return;
            if (!basicsInput.IsTextField(e))
            {
                Console.WriteLine("no text field");
                return;
            }




           
            Automation.AddAutomationPropertyChangedEventHandler(AutomationElement.FocusedElement,
                TreeScope.Element, propChangeHandler = new AutomationPropertyChangedEventHandler(OnPropertyChange),
                ValuePattern.ValueProperty);



        }

        static private void OnPropertyChange(object src, AutomationPropertyChangedEventArgs  arg)
        {


    
            if (arg.NewValue != arg.OldValue)
            {
    
            
                if (!basicsInput.IsTextField(AutomationElement.FocusedElement))
                {
                    Console.WriteLine("no text field");
                    return;
                }

                var tot = basicsInput.GetCurrentLineAndWord(AutomationElement.FocusedElement);
                answersblock = _assembly.SetOrder(tot.CurrentLine, tot.CurrentWord);
                var Prime = answersblock.Completion.Item1 != "" ?answersblock.Completion.Item1 : answersblock.Correction.Item1;
                if (Prime == null) Prime = "";

                var sec = answersblock.SecondaryCompletion.Item1 != "" ? answersblock.SecondaryCompletion.Item1 : answersblock.SecondaryCorrection.Item1;
                if (sec == null) sec = "";

                var Trans = translation.TranslateAsync(Prime, "en", "ar");
            
                Program.uI.Dispatcher.Invoke(() => Program.uI.Primaryoption.Text = Prime);
                Program.uI.Dispatcher.Invoke(() => Program.uI.Secondaryoption.Text = sec);
                Program.uI.Dispatcher.Invoke(async () => Program.uI.Translationoption.Text = await Trans);



            }

        }
        public static void UnsubscribePropertyChange()
        {
            if (propChangeHandler != null)
            {
                Automation.RemoveAutomationPropertyChangedEventHandler(AutomationElement.FocusedElement, propChangeHandler);
            }
        }
        

    }


    }
