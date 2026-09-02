using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections.Generic;

namespace Winboard
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Media.Animation;

    public static class ShiftWindowOntoScreenHelper
    {
        
        public static void ShiftWindowOntoScreen(Window window)
        {

            if (window.Top < SystemParameters.VirtualScreenTop)
            {

                while(window.Top < SystemParameters.VirtualScreenTop + 20)
                window.Top += 1;

            }

            if (window.Left < SystemParameters.VirtualScreenLeft )
            {

                while (window.Left <SystemParameters.VirtualScreenLeft+20)
                window.Left += 1;

            }
           

            if (window.Left + window.Width > SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth)
            {
                while(window.Left + window.Width > SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth-20)
                    window.Left -= 1;

            }

            if (window.Top + window.Height > SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight)
            {
                while (window.Top + window.Height > SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight-20)
                    window.Top -= 1;
            }

            {
                var taskBarLocation = GetTaskBarLocationPerScreen();


                foreach (var taskBar in taskBarLocation)
                {
                     Rectangle windowRect = new Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height);

                    int avoidInfiniteLoopCounter = 25;
                    while (windowRect.IntersectsWith(taskBar))
                    {
                        avoidInfiniteLoopCounter--;
                        if (avoidInfiniteLoopCounter == 0)
                        {
                            break;
                        }


                        var intersection = Rectangle.Intersect(taskBar, windowRect);

                        if (intersection.Width < window.Width

                            || taskBar.Contains(windowRect))
                        {
                            if (taskBar.Left == 0)
                            {
                                window.Left = window.Left + intersection.Width;
                            }
                            else
                            {
                                window.Left = window.Left - intersection.Width;
                            }
                        }

                        if (intersection.Height < window.Height
 
                            || taskBar.Contains(windowRect))
                        {
                            if (taskBar.Top == 0)
                            {
                                window.Top = window.Top + intersection.Height;
                            }
                            else
                            {
                                window.Top = window.Top - intersection.Height;
                            }
                        }

                        windowRect = new Rectangle((int)window.Left, (int)window.Top, (int)window.Width, (int)window.Height);
                    }
                }
            }
        }

        private static List<Rectangle> GetTaskBarLocationPerScreen()
        {
            
            List<Rectangle> dockedRects = new List<Rectangle>();
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.Bounds.Equals(screen.WorkingArea) == true)
                {
                    // No taskbar on this screen.
                    continue;
                }

                Rectangle rect = new Rectangle();

                var leftDockedWidth = Math.Abs((Math.Abs(screen.Bounds.Left) - Math.Abs(screen.WorkingArea.Left)));
                var topDockedHeight = Math.Abs((Math.Abs(screen.Bounds.Top) - Math.Abs(screen.WorkingArea.Top)));
                var rightDockedWidth = ((screen.Bounds.Width - leftDockedWidth) - screen.WorkingArea.Width);
                var bottomDockedHeight = ((screen.Bounds.Height - topDockedHeight) - screen.WorkingArea.Height);
                if ((leftDockedWidth > 0))
                {
                    rect.X = screen.Bounds.Left;
                    rect.Y = screen.Bounds.Top;
                    rect.Width = leftDockedWidth;
                    rect.Height = screen.Bounds.Height;
                }
                else if ((rightDockedWidth > 0))
                {
                    rect.X = screen.WorkingArea.Right;
                    rect.Y = screen.Bounds.Top;
                    rect.Width = rightDockedWidth;
                    rect.Height = screen.Bounds.Height;
                }
                else if ((topDockedHeight > 0))
                {
                    rect.X = screen.WorkingArea.Left;
                    rect.Y = screen.Bounds.Top;
                    rect.Width = screen.WorkingArea.Width;
                    rect.Height = topDockedHeight;
                }
                else if ((bottomDockedHeight > 0))
                {
                    rect.X = screen.WorkingArea.Left;
                    rect.Y = screen.WorkingArea.Bottom;
                    rect.Width = screen.WorkingArea.Width;
                    rect.Height = bottomDockedHeight;
                }
                else
                {
                    // Nothing found!
                }

                dockedRects.Add(rect);
            }

            if (dockedRects.Count == 0)
            {
                // Taskbar is set to "Auto-Hide".
            }

            return dockedRects;
        }

    }



}
