using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        static string NewElement { get; set; }
        static string CurrentElement { get; set; }
        static AutomationFocusChangedEventHandler focusHandler = null;

        static AutomationPropertyChangedEventHandler propChangeHandler;

        public static void intextfield()
        {

      
            SubscribeToPropertyChange();
            SubscribeToFocusChange();
                
            
            CurrentElement  = AutomationElement.FocusedElement.Current.Name;
           
        }


        static public void OnPropertyChange(object src, AutomationPropertyChangedEventArgs  arg)
        {

            

            if (arg.NewValue != arg.OldValue)
            {

                var tot = basicsInput.GetCurrentLineAndWord(AutomationElement.FocusedElement);
                answersblock = _assembly.SetOrder(tot.CurrentLine, tot.CurrentWord);
                var Prime = answersblock.Completion.Item1 != "" ? answersblock.Completion.Item1 : answersblock.Correction.Item1;
                if (Prime == null) Prime = "";

                var sec = answersblock.SecondaryCompletion.Item1 != "" ? answersblock.SecondaryCompletion.Item1 : answersblock.SecondaryCorrection.Item1;
                if (sec == null) sec = "";

                var Tri = answersblock.TriCompletion.Item1 !=""?answersblock.TriCompletion.Item1:answersblock.TriCorrection.Item1;

                Program.WPFWindow.Dispatcher.Invoke(() => Program.WPFWindow.PrimaryReactangle.Text = Prime);
                Program.WPFWindow.Dispatcher.Invoke(() => Program.WPFWindow.SecondaryReactangle.Text = sec);
                Program.WPFWindow.Dispatcher.Invoke(() => Program.WPFWindow.TriReactangle.Text = Tri);

            }
            
           
         
           
           

        }


        static public void SubscribeToPropertyChange()
        {
            UnsubscribePropertyChange();
            Automation.AddAutomationPropertyChangedEventHandler(AutomationElement.FocusedElement,
            TreeScope.Subtree, propChangeHandler = new AutomationPropertyChangedEventHandler(OnPropertyChange),
            ValuePattern.ValueProperty);

        }

        public static void UnsubscribePropertyChange()
        {
            if (propChangeHandler == null) return;
            if(propChangeHandler!=null)
            Automation.RemoveAutomationPropertyChangedEventHandler(AutomationElement.FocusedElement, propChangeHandler);

            propChangeHandler = null;
        }


        
        private static  void OnFocusChange(object src, AutomationFocusChangedEventArgs e)
        {
            if(AutomationElement.FocusedElement.Current.Name !=null)
            NewElement = AutomationElement.FocusedElement.Current.Name;

            //Prevent taking MainWindow of VS as a focus
            if (NewElement != CurrentElement && NewElement != "MainWindow")
            {
               
                intextfield();
               CurrentElement = NewElement;
            }
            
            
           
        }
        public static void SubscribeToFocusChange()
        {
            UnSubscribeToFocusChange();
            focusHandler = new AutomationFocusChangedEventHandler(OnFocusChange);
           
            Automation.AddAutomationFocusChangedEventHandler(focusHandler);
        }
        public static void UnSubscribeToFocusChange()
        {
            
            if (focusHandler == null) return;

            if(focusHandler!=null)
            Automation.RemoveAutomationFocusChangedEventHandler(focusHandler);

            focusHandler = null;
        }


    }


    }
