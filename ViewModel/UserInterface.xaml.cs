using K4os.Compression.LZ4.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Winboard.ViewModel.triggers;
using WinboardDesgin;
using WindowsInput;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using formS = System.Windows.Forms;
using WPF = System.Windows.Input;
namespace Winboard.ViewModel
{ 
    public partial class UI :System.Windows. Window
    {


        InputSimulator sime = new();
        private bool _IsWpfWindowHidden = false;
       
        ClipboardMonitor clipboardmonitor;

        formS.Cursor cursor;

        HoloParticles holoParticles = new(450, 30);
        
        private GlobalHotKey W_Alt_hotkey;

        List<string> PinList = GetData.PinLists;
        List<BitmapSource> pictures = GetData.PicturesPinLists;

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
            this.TriReactangle.Text = "writ";
            
            this.Focusable = false;
            SourceInitialized += (s, e) => new Blureffect().EnableBlur(this);

            SourceInitialized += MainWindow_SourceInitialized;

            SourceInitialized += (s, e) => Wincorner.ChangeCornerStyle(this, Wincorner.CornerStyle.Round);



            AddPreviousPinItem(PinList , pictures);

            ClipboardPopup.MouseMove += new WPF.MouseEventHandler(ClipMenu_MouseMove);
           
            holoParticles.AddParticles(this);
        }

        private void MainWindow_SourceInitialized(object sender, EventArgs e)
        {
            var source = (HwndSource)PresentationSource.FromVisual(this);
            source.AddHook(WndProc);


            var handle = new WindowInteropHelper(this).Handle;

            W_Alt_hotkey = new GlobalHotKey(Keys.W, Keys.Alt, handle, GetHashCode());
            W_Alt_hotkey.register();

        }

       

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // is this a HotKey ?

            var MSG =(WindowMessage)msg;
            
            HideAndShowWindow(MSG);
            
            return IntPtr.Zero;
        }

        private void HideAndShowWindow(WindowMessage MSG)
        {

            if (MSG == WindowMessage.WM_HotKey && _IsWpfWindowHidden == false)
            {

                this.Hide();
                
                ClipboardPopup.IsOpen = false;
                _IsWpfWindowHidden = true;
             
            }

            else if (MSG == WindowMessage.WM_HotKey && _IsWpfWindowHidden == true)
            {
               
                this.Show();
                _IsWpfWindowHidden = false;
            
            }
        }

        private void holdUI(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(ClipboardPopup.IsMouseOver == false)
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

        private bool _SkipScondCopy = false;
        private void ClipboardMOnitor_ClipboardChange(object sender, EventArgs e)
        {

            if (System.Windows.Clipboard.ContainsText())
            {
                
                string usercopy = System.Windows.Clipboard.GetText();

                //prevent adding the same copy to the list

               if (  !Entries.Any(f=>f.UserCopy == usercopy) )
                Entries.Insert( 0 ,new ItemViewModel { UserCopy = usercopy , IsChecked = false ,Datatype  = typeof(string)});

            }
            else if (System.Windows.Clipboard.ContainsImage())
            {
                BitmapSource userimage = System.Windows.Clipboard.GetImage();

                if (_SkipScondCopy)
                {
                    Entries.Insert(0, new ItemViewModel { IsChecked = false, UserImage = userimage, Datatype = typeof(BitmapSource) });
                }
                
                _SkipScondCopy = !_SkipScondCopy;

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


        private bool OnBoardList = false;
        private void ListBoard_MouseMove(object sender, WPF.MouseEventArgs e)
        {

            

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                
                if (OnBoardList == false) OnBoardList = true;

            }
            else OnBoardList = false;
        }

        private void ClipMenu_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed && OnBoardList == false)
            {
                
                var _finalpos = GetMousePos();
                var Dx = _finalpos.X - _MouseInitialPos.X;
                var Dy = _finalpos.Y - _MouseInitialPos.Y;

                ClipboardPopup.HorizontalOffset += Dx;
                ClipboardPopup.VerticalOffset += Dy;

                _MouseInitialPos = _finalpos;

            }

        }

        private void TrashIcon_click(object sender, RoutedEventArgs e)
        {
            
            var ItemIdx = this.UserCopiesList.SelectedIndex;

            Entries.RemoveAt(ItemIdx);
        }

        private void AddPreviousPinItem(List<string> Items , List<BitmapSource> images)
        {
            foreach(var i in Items)
            {
                Entries.Add(new ItemViewModel { UserCopy = i , IsChecked = true , Datatype=typeof(string)});
            }
            foreach (var i in images)
            {
                Entries.Add(new ItemViewModel { UserImage = i, IsChecked = true , Datatype = typeof(BitmapSource)});
            }

        }
        private void PinIcon_Click(object sender, RoutedEventArgs e)
        {
            var ItemString = this.UserCopiesList.SelectedItem as ItemViewModel;


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

            public Type Datatype { get; set; }

            public BitmapSource UserImage { get; set; }
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
            var L2 = new List<BitmapSource>();
            foreach(var item in Entries)
            {
                if (item.IsChecked == true)
                {
                    if (item.Datatype == typeof(string)) L.Add((item.UserCopy));
                    if (item.Datatype == typeof(BitmapSource)) L2.Add(item.UserImage);
                }
            }
            GetData.UpdatePinItemData(L);
            GetData.UpdatePinPictures(L2);

            //unregiser the app hot-key
            W_Alt_hotkey?.Unregister();
        }

        private void ListBoard_MouseDown(object sender, MouseButtonEventArgs e)

        {



            var item = UserCopiesList.SelectedItem;



            var obj = item as ItemViewModel;

            if (obj.Datatype == typeof(string))
            {
                var text = obj.UserCopy;
                System.Windows.Clipboard.SetText(text);



                sime.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_V);

                Thread.Sleep(15);


                System.Windows.Clipboard.Clear();
            }

            else if(obj.Datatype == typeof(BitmapSource))
            {
                var Image = obj.UserImage;
                System.Windows.Clipboard.SetImage(Image);



                sime.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_V);

                Thread.Sleep(15);


                System.Windows.Clipboard.Clear();
            }

        }
        private void Clipboard_button_MouseMove(object sender, WPF.MouseEventArgs e)
        {
            var point = GetMousePos();

            if (ClipboardPopup.IsOpen == false)
            {
                ClipboardPopup.HorizontalOffset = point.X;
                ClipboardPopup.VerticalOffset = point.Y;
            }



        }


    }


}
