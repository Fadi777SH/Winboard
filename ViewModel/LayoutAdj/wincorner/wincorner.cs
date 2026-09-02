using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinboardDesgin
{
    public static class Wincorner
    {
        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrvalue, int attrsize);
        [DllImport("dwmapi.dll")]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS pmargins);

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopWidth;
            public int cyBottomHeight;
        } 

        public enum CornerStyle
        {
            defult = 0,
            DoNotRound =1,
            Round = 2,
            RoundSmall =3

        }


        public static  void ChangeCornerStyle(System.Windows.Window win , CornerStyle style)
        {
            IntPtr Hand = new WindowInteropHelper(win).Handle;
            int pref = (int)style;
            DwmSetWindowAttribute(Hand, 33, ref pref , sizeof(int));
            MARGINS marg = new MARGINS { cxLeftWidth = 1, cxRightWidth = 1, cyBottomHeight = 1, cyTopWidth = 1 };
            DwmExtendFrameIntoClientArea(Hand, ref marg);
        }


    }
}
