using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Winboard.ViewModel.triggers
{
    public class GlobalHotKey
    {
        //Create A shortcut to run the application and another shortcut to close it

        private readonly int Modifier;
        private readonly int key;
        private readonly IntPtr handle;
        private readonly int id;
        private const int oo = 0x0312;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hwnd, int id, int fsModifier, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hwnd, int id);

        private const int Alt_Key = 0x1;

        public GlobalHotKey(Keys key , Keys modifiers , IntPtr handle , int id)
        {
            this.key = (int)key;
            this.handle = handle;
            this.Modifier = 0;
            if (modifiers.HasFlag(Keys.Alt)) this.Modifier = Alt_Key;
        }

        public bool register() => RegisterHotKey(handle,  id, Modifier,  key);
        public bool Unregister() => UnregisterHotKey(handle, id);

        public bool Dispose() => Unregister();


                   
    }

    public enum WindowMessage : uint
    {
        WM_HotKey = 0x0312,
        WM_PASTE  =  0x0302
    }

}
