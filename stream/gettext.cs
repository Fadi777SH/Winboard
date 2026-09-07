using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using Winboard.ViewModel;

namespace Winboard
{
    public  class Gettext
    {


        static Scale.Answersblock answersblock = new();
        static BasicsInput basicsInput = new();
        static Assembly _assembly = new();
        static Translation translation = new();
        static string NewElement { get; set; }
        static string CurrentElement { get; set; }
        static AutomationFocusChangedEventHandler focusHandler = null;
        static AutomationPropertyChangedEventHandler propChangeHandler;

        public static void intextfield()
        {
            UnsubscribePropertyChange();

            AutomationElement e = AutomationElement.FocusedElement;
            if (e == null) return;
            if (!basicsInput.IsTextField(e))
            {
                System.Diagnostics.Debug.WriteLine("no text field");
                return;
            }

            Automation.AddAutomationPropertyChangedEventHandler(AutomationElement.FocusedElement,
                TreeScope.Subtree, propChangeHandler = new AutomationPropertyChangedEventHandler(OnPropertyChange),
                ValuePattern.ValueProperty);

            SubscribeToFocusChange();

            CurrentElement  = AutomationElement.FocusedElement.Current.Name;


            SubscribeToFocusChange();
            
            
            

        }

        static public void OnPropertyChange(object src, AutomationPropertyChangedEventArgs  arg)
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
                var Prime = answersblock.Completion.Item1 != "" ? answersblock.Completion.Item1 : answersblock.Correction.Item1;
                if (Prime == null) Prime = "";

                var sec = answersblock.SecondaryCompletion.Item1 != "" ? answersblock.SecondaryCompletion.Item1 : answersblock.SecondaryCorrection.Item1;
                if (sec == null) sec = "";

                var Trans = translation.TranslateAsync(Prime, "en", "ar");

                Program.WPFWindow.Dispatcher.Invoke(() => Program.WPFWindow.PrimaryReactangle.Text = Prime);
                Program.WPFWindow.Dispatcher.Invoke(() => Program.WPFWindow.SecondaryReactangle.Text = sec);
                Program.WPFWindow.Dispatcher.Invoke(async () => Program.WPFWindow.TranslationReactangle.Text = await Trans);

            }
            
           
         
           
           

        }
        public static void UnsubscribePropertyChange()
        {
            if (propChangeHandler == null) return;

            Automation.RemoveAutomationPropertyChangedEventHandler(AutomationElement.FocusedElement, propChangeHandler);
            
            propChangeHandler = null;
        }



        private static  void OnFocusChange(object src, AutomationFocusChangedEventArgs e)
        {
            if(AutomationElement.FocusedElement.Current.Name !=null)
            NewElement = AutomationElement.FocusedElement.Current.Name;

            if (NewElement != CurrentElement)
            {

                CurrentElement = NewElement;

                intextfield();
                
            }
            
            
           
        }
        public static void SubscribeToFocusChange()
        {
            //UnSubscribeToFocusChange();
            focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
            Automation.AddAutomationFocusChangedEventHandler(focusHandler);
        }
        public static void UnSubscribeToFocusChange()
        {
            if (focusHandler == null) return;
            Automation.RemoveAutomationFocusChangedEventHandler(focusHandler);
            focusHandler = null;
        }


    }


    }
