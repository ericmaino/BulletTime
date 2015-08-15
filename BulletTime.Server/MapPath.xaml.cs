using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using BulletTime.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BulletTime.Server
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPath : Page
    {
        public MapPath()
        {
            this.InitializeComponent();
        }

        private Point m_PreviousContactPoint = default(Point);
        private int m_FrameCount = 0;
        private IList<Tuple<int, int>> frames = new List<Tuple<int, int>>();

        public void OnCanvasPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var cameras = ((MapViewModel)this.DataContext).CameraCount;
            PointerPoint pt = e.GetCurrentPoint(InkCanvas);

            PointerDeviceType pointerDevType = e.Pointer.PointerDeviceType;

            if (pointerDevType == PointerDeviceType.Mouse && !pt.Properties.IsLeftButtonPressed)
            {
                return;
            }
            var currentContactPt = pt.Position;

            if (m_PreviousContactPoint == default(Point))
            {
                m_PreviousContactPoint = currentContactPt;
                return;
            }

            var x1 = m_PreviousContactPoint.X;
            var y1 = m_PreviousContactPoint.Y;
            var x2 = currentContactPt.X;
            var y2 = currentContactPt.Y;
            double distance = CalculateDistance(x1, y1, x2, y2);
            Color color = Color.FromArgb(120, 255, 0, 0);

            var frame = (int)((x2 * 30) / InkCanvas.ActualWidth) + 2;
            var camera = (int)((y2 * cameras) / InkCanvas.ActualHeight) + 1;
            frames.Add(new Tuple<int, int>(frame, camera));

            if (distance > 5.0)
            {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        //
                        // If the delta of the mouse is significant enough,
                        // we add a line geometry to the Canvas
                        Line line = new Line()
                        {
                            X1 = x1,
                            Y1 = y1,
                            X2 = x2,
                            Y2 = y2,
                            StrokeThickness = 5,
                            Stroke = new SolidColorBrush(color)
                        };

                        // Draw the line on the canvas by adding the Line object as
                        // a child of the Canvas object.
                        InkCanvas.Children.Add(line);
                    });
                m_PreviousContactPoint = currentContactPt;
            }
        }

        private double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            double distance = Math.Sqrt(Math.Pow(Math.Abs(x1 - x2), 2) + Math.Pow(Math.Abs(y1 - y2), 2));
            return distance;
        }

        private async void Play(object sender, RoutedEventArgs e)
        {
            var context = this.DataContext as MapViewModel;
            var folder = await KnownFolders.VideosLibrary.CreateFolderAsync("BulletTime", CreationCollisionOption.OpenIfExists);
            var images = new List<WriteableBitmap>();

            foreach (var frame in this.frames)
            {
                var ip = MapViewModel.CurrentCameras.Skip(frame.Item2 - 1).First().IPAddress;

                var cameraFolder = await folder.CreateFolderAsync(ip.ToString(), CreationCollisionOption.OpenIfExists);
                var file = await cameraFolder.OpenStreamForReadAsync($"{frame.Item1:D2}.jpg");
                var decoder = await BitmapDecoder.CreateAsync(file.AsRandomAccessStream());
                var f = await decoder.GetFrameAsync(0);
                var bmp = new WriteableBitmap((int)f.PixelWidth, (int)f.PixelHeight);
                await bmp.SetSourceAsync(file.AsRandomAccessStream());
                images.Add(bmp);
            }

            RenderViewModel.Instance.Images = images;
            this.Frame.Navigate(typeof(Render));
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
