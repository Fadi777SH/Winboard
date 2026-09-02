using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Forms;
using System;

namespace Winboard
{
    public class ClipboardMonitor
    {
        public event EventHandler ClipboardChanged;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        private const int WM_CLIPBOARDUPDATE = 0x031D;
        private HwndSource _source;
        private IntPtr _handle;

        public void Start(System.Windows.Window window)
        {
            _handle = new WindowInteropHelper(window).Handle;
            _source = HwndSource.FromHwnd(_handle);
            _source.AddHook(WndProc);
            AddClipboardFormatListener(_handle);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_CLIPBOARDUPDATE)
            {
                ClipboardChanged?.Invoke(this, EventArgs.Empty); // raise the event
            }
            return IntPtr.Zero;
        }

        public void Stop()
        {
            RemoveClipboardFormatListener(_handle);
            _source?.RemoveHook(WndProc);
        }
    }
}
