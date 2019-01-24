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
        public const int BASERADIUS = 25;
        private PointData[] points = new PointData[POINTSCOUNT];
        private WPFDraw panel;
        int _trbMulti = 10;
        int currentPoint = 0;

        public struct PointData
        {
            public double value;
            public double currentValue;
            public int degree;
        }
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
            double volume = ((MMDevice)(cbDevice.SelectedItem)).AudioMeterInformation.MasterPeakValue * 100 * _trbMulti + BASERADIUS * _trbMulti;
            currentPoint %= POINTSCOUNT;
            points[0].value = volume;
            points[0].currentValue = volume;
            points[0].degree = 0;
            currentPoint++;
            Paint();
            for(int i = POINTSCOUNT - 1; i > 0; i--)
            {
                points[i] = points[i - 1];
                points[i].degree++;
                points[i].currentValue -= ((points[i].value - BASERADIUS * _trbMulti) / POINTSCOUNT);
            }
            dispatcherTimer.Start();
        }
        private void Paint()
        {
            panel.Clear();
            Point middle = new Point(mainCanvas.ActualWidth / 2, mainCanvas.ActualHeight / 2);
            Point circlePoint = PointOnCircle(BASERADIUS * _trbMulti, 359, middle);
            Point previousPoint = PointOnCircle(points[0].value, points[0].degree, middle);
            panel.DrawLine((int)previousPoint.X, (int)previousPoint.Y, (int)circlePoint.X, (int)circlePoint.Y, Brushes.Red);
            for (int i = 0; i < POINTSCOUNT; i++)
            {
                circlePoint = PointOnCircle(points[i].currentValue, points[i].degree, middle);
                panel.DrawLine((int)circlePoint.X, (int)circlePoint.Y, (int)previousPoint.X, (int)previousPoint.Y, Brushes.Blue);
                previousPoint = circlePoint;
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
