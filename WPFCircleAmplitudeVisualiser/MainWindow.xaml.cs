using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPFCircleAmplitudeVisualiser
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public const int POINTSCOUNT = 360;
        private Point[] points = new Point[POINTSCOUNT];
        private WPFDraw panel;
        int _trbMulti = 4;
        int currentPoint = 0;
        public MainWindow()
        {
            InitializeComponent();
            setup();
        }
        private void setup()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);
            foreach(var device in devices)
            {
                cbDevice.Items.Add(device);
            }
            if (cbDevice.Items.Count > 0)
                cbDevice.SelectedIndex = 0;
            panel = new WPFDraw(mainCanvas);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Start();
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            Point middle = new Point(mainCanvas.ActualWidth / 2, mainCanvas.ActualHeight / 2);
            double volume = ((MMDevice)(cbDevice.SelectedItem)).AudioMeterInformation.MasterPeakValue * 100 * _trbMulti + 50 * _trbMulti;
            currentPoint %= POINTSCOUNT;
            points[currentPoint++] = PointOnCircle(volume, currentPoint * (360 / POINTSCOUNT), middle);
            Paint();
            dispatcherTimer.Start();
        }
        private void Paint()
        {
            panel.Clear();
            for (int i = 0; i < POINTSCOUNT; i++)
            {
                panel.DrawLine((int)points[i].X, (int)points[i].Y, (int)points[(i + 1) % 360].X, (int)points[(i + 1) % 360].Y, Brushes.Blue);
            }
        }
        private static Point PointOnCircle(double radius, double angleInDegrees, Point origin)
        {
            // Convert from degrees to radians via multiplication by PI/180        
            double x = (radius * Math.Cos(angleInDegrees * Math.PI / 180)) + origin.X;
            double y = (radius * Math.Sin(angleInDegrees * Math.PI / 180)) + origin.Y;

            return new Point(x, y);
        }
    }
}
