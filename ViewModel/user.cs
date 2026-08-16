using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Winboard.ViewModel
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 


    public partial class UI : Window
    {
        
        

        public UI()
        {

            InitializeComponent();
            //DataContext = new MainViewModel(); 
           
        } 

        private void holdUI(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
