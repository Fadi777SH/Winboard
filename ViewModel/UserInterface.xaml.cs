using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media;
using WinboardDesgin;
using formS = System.Windows.Forms;
using WPF = System.Windows.Input;

namespace Winboard.ViewModel
{
    public partial class UI : Window
    {
        
        ClipboardMonitor clipboardmonitor;

        formS.Cursor cursor;

        HoloParticles holoParticles = new(450, 30);

        List<string> PinList = GetData.PinLists;

        public UI()
        {
            InitializeComponent();
            DataContext = this;
            entries = new  ObservableCollection<ItemViewModel>();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Left = (SystemParameters.VirtualScreenWidth - this.Width) * 0.5;
            this.Top = SystemParameters.VirtualScreenTop + 5;

            this.PrimaryReactangle.Text = "hello";
            this.SecondaryReactangle.Text = "Type";
            this.TranslationReactangle.Text = "مرحبا";


            SourceInitialized += (s, e) => new Blureffect().EnableBlur(this);


            SourceInitialized += (s, e) => Wincorner.ChangeCornerStyle(this, Wincorner.CornerStyle.Round);


            UserCopiesList.Items.SortDescriptions.Add(new SortDescription("Copies", ListSortDirection.Descending));

            AddPreviousPinItem(PinList);

            ClipMenu.MouseMove += new WPF.MouseEventHandler(ClipMenu_MouseMove);

            holoParticles.AddParticles(this);
        }


        private void holdUI(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();

            ShiftWindowOntoScreenHelper.ShiftWindowOntoScreen(this);
        }


        protected override void OnRender(DrawingContext e)
        {
            base.OnRender(e);
            foreach (var p in holoParticles._particles)
            {
                p.Draw(e);
            }

        }
       private  ObservableCollection<ItemViewModel> entries;
        public ObservableCollection<ItemViewModel> Entries
        {
            get { return entries; }
            set { entries = value; }
        }
        
        private void ClipboardMOnitor_ClipboardChange(object sender, EventArgs e)
        {
            if (System.Windows.Clipboard.ContainsText())
            {
                
                string usercopy = System.Windows.Clipboard.GetText();

                //prevent adding the same copy to the list

               if (    (Entries.Any(f=>f.UserCopy == usercopy))  == false )
                Entries.Insert( 0 ,new ItemViewModel { UserCopy = usercopy , IsChecked = false});

            }
            
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            clipboardmonitor = new ClipboardMonitor();
            clipboardmonitor.Start(this);
            clipboardmonitor.ClipboardChanged += ClipboardMOnitor_ClipboardChange;
        }



        private System.Windows.Point GetMousePos()
        {
            var X = MousePosition.GetCursorPosition().X;
            var Y = MousePosition.GetCursorPosition().Y;
            var MousePoint = new System.Windows.Point(X, Y);

            PresentationSource source = PresentationSource.FromVisual(this);
            Matrix transform = source.CompositionTarget.TransformFromDevice;
            var wpfPoint = transform.Transform(MousePoint);
            return wpfPoint;
        }




        private System.Windows.Point _MouseInitialPos { get; set; }

        private void ClipMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {


            if (MouseButtonState.Pressed == e.LeftButton)
            {
                _MouseInitialPos = GetMousePos();
            }
        }

        private void ClipMenu_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
            if (e.LeftButton == MouseButtonState.Pressed && OnBoardList == false)
            {

                var _finalpos = GetMousePos();
                var Dx = _finalpos.X - _MouseInitialPos.X;
                var Dy = _finalpos.Y - _MouseInitialPos.Y;

                ClipMenu.HorizontalOffset += Dx;
                ClipMenu.VerticalOffset += Dy;

                _MouseInitialPos = _finalpos;

            }

        }
        private bool OnBoardList = false;
        private void Clipboard_box_MouseMove(object sender, WPF.MouseEventArgs e)
        {

            var point = GetMousePos();
            ClipMenu.HorizontalOffset = point.X;
            ClipMenu.VerticalOffset = point.Y;

            if (e.LeftButton == MouseButtonState.Pressed && Clipboard_button.IsChecked == false)
            {
                Clipboard_button.IsChecked = true;
            }
        }

        private void ListBoard_MouseMove(object sender, WPF.MouseEventArgs e)
        {

            

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                
                if (OnBoardList == false) OnBoardList = true;

            }
            else OnBoardList = false;
        }



        private void TrashIcon_click(object sender, RoutedEventArgs e)
        {
            
            var ItemIdx = this.UserCopiesList.SelectedIndex;

            Debug.WriteLine(ItemIdx);

            Entries.RemoveAt(ItemIdx);
        }
        private List<string> _PinsItemString = new();

        private void AddPreviousPinItem(List<string> Items)
        {
            foreach(var i in Items)
            {
                Entries.Add(new ItemViewModel { UserCopy = i , IsChecked = true });
            }
            
            
        }
        private void PinIcon_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as ToggleButton;
            var ItemString = this.UserCopiesList.SelectedItem as ItemViewModel;

            //var ItemIdx = this.UserCopiesList.SelectedIndex;
            Debug.WriteLine(this.UserCopiesList.SelectedItem.GetType());

            if (ItemString.IsChecked == true)
            {
                ItemString.IsChecked = true;
            }

            else if (ItemString.IsChecked ==false)
            {
                ItemString.IsChecked = false;
            }


        }
        private void ClearAll_click(object sender, RoutedEventArgs e)
        {
            
            for (int i = Entries.Count - 1; i >= 0; i--)
            {

                if (Entries[i].IsChecked == true) continue;

                Entries.RemoveAt(i);
            }
        }

        public class ItemViewModel : INotifyPropertyChanged
        {
            public string UserCopy { get; set; }

            private bool? _isChecked;
            public bool? IsChecked
            {
                get { return _isChecked; }
                set
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
       where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            foreach (childItem child in FindVisualChildren<childItem>(obj))
            {
                return child;
            }

            return null;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var L = new List<string>();
            foreach(var item in Entries)
            {
                if (item.IsChecked == true) L.Add(item.UserCopy);
            }
            GetData.UpdatePinItemData(L);
        }
    }
}
