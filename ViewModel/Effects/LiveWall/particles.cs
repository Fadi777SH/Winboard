using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using Winboard.ViewModel;
using System.Collections.Generic;
using System;
namespace Winboard
{
    class HoloParticles
    {

        public float X;
        public float Y;
        public float Size;
        public float Speed;
        public System.Drawing.Color color;
        private int _dirction;
        private DispatcherTimer _timer ;
        private UI _mainwindow;

        public FrameworkElement _target =new();
        public List<HoloParticles> _particles = new();
        private const int _particlescount = 20;
        private static System.Random _ran = new();

        private float[] _trilly;
        private int Trillength = 5;

        public HoloParticles(int Width , int hight)
        {
            X = _ran.Next(0, Width);
            Y = _ran.Next(0, hight);
            Size = (float)(_ran.NextDouble() * 3 + 1);
            Speed = (float)(_ran.NextDouble() * 0.5 + 0.2);
            _dirction = _ran.Next(0, 2) == 0 ? 1 : -1;
            color = System.Drawing.Color.FromArgb(_ran.Next(10, 100), 255, 255, 255);
            _trilly = new float[Trillength];
            for(int i =0; i< Trillength; i++)
            {
                _trilly[i] = Y - i * 2f;
            }

        } 
        public void UpDate(int Width , int Highet)
        {
            Y += Speed * _dirction;
            if (Y < 0) Y = Highet;
            if (Y > Highet) Y = 0;
            for(int i =Trillength-1;i>0; i--)
            {
                _trilly[i] = _trilly[i - 1];

                    
            }
            _trilly[0] = Y;
            

        }
        public void Draw(DrawingContext g)
        {
            for(int i = Trillength-1; i>=0; i--)
            {
                int alpha = (int)(color.A * (0.2 + 0.8*(1f - i / (float)Trillength)));
                var wpfColor = System.Windows.Media.Color.FromArgb((byte)alpha, color.R, color.G, color.B);
                var br = new System.Windows.Media.SolidColorBrush(wpfColor);

                g.DrawEllipse(br, null, new System.Windows.Point(X + Size / 2, Y + Size / 2), Size / 2, Size / 2);
               
            }
            
        }

        public void AddParticles(UI mainwindow)
        {
            _mainwindow = mainwindow;

            _target = mainwindow;
            for (int i=0; i < _particlescount; i++)
            {
                _particles.Add(new HoloParticles((int)_mainwindow.Width, (int)_mainwindow.Height));
            }

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(30);
            _timer.Tick += _anim_Tick;
            _timer.Start();

        }

        private void _anim_Tick(object? sender, System.EventArgs e)
        {
            foreach (var p in _particles)
            {
                p.UpDate((int)_mainwindow.ActualWidth, (int)_mainwindow.ActualHeight);

            }

            _target?.InvalidateVisual();
        }



    }
}
