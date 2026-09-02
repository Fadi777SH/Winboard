using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;


namespace Winboard
{
    public class Blureffect
    {


        [StructLayout(LayoutKind.Sequential)]
        public struct AccentPolicy
        {
            public int _AccentState;
            public int _AccentFlag;
            public int _GradientColor;
            public int _AnimationID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WindowCompositionAttributeData
        {
            public int _Attribute;
            public IntPtr _Data;
            public int SizeOfData;
        }

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(
            IntPtr hwnd,
            ref WindowCompositionAttributeData data
        );

        public void EnableBlur(Window window)
        {

            IntPtr Hand= new WindowInteropHelper(window).Handle;
            var accent = new AccentPolicy();
            accent._AccentState = 4;
            var accentstructuresize = Marshal.SizeOf(accent);
            var accentptr = Marshal.AllocHGlobal(accentstructuresize);
            Marshal.StructureToPtr(accent, accentptr, false);

            var accentdata = new WindowCompositionAttributeData();
            accentdata._Attribute = 19;
            accentdata.SizeOfData = accentstructuresize;
            accentdata._Data = accentptr;

            SetWindowCompositionAttribute(Hand,ref accentdata);
            Marshal.FreeHGlobal(accentptr);
        }
    }
    }

