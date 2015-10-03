using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using BulletTime.Rendering;
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
            InitializeComponent();
            Loaded += MapPath_Loaded;
        }

        private PointMapper Map { get; set; }
        private Point PreviousPoint { get; set; }

        private void MapPath_Loaded(object sender, RoutedEventArgs e)
        {
            var model = ((MapViewModel) DataContext);
            Map = new PointMapper(InkCanvas.ActualHeight, InkCanvas.ActualWidth, model.CameraCount, 30);
        }

        public async void OnCanvasPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var pt = e.GetCurrentPoint(InkCanvas);

            var pointerDevType = e.Pointer.PointerDeviceType;

            if (pointerDevType == PointerDeviceType.Mouse && !pt.Properties.IsLeftButtonPressed)
            {
                return;
            }

            var color = Color.FromArgb(120, 255, 0, 0);

            Map.AddPoint(pt.Position);

            if (PreviousPoint != default(Point))
            {
                if (PreviousPoint.GetDistance(pt.Position) > 5.0)
                {
                    var prev = PreviousPoint;
                    var curr = pt.Position;

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            //
                            // If the delta of the mouse is significant enough,
                            // we add a line geometry to the Canvas
                            var line = new Line
                            {
                                X1 = prev.X,
                                Y1 = prev.Y,
                                X2 = curr.X,
                                Y2 = curr.Y,
                                StrokeThickness = 5,
                                Stroke = new SolidColorBrush(color)
                            };

                            // Draw the line on the canvas by adding the Line object as
                            // a child of the Canvas object.
                            InkCanvas.Children.Add(line);
                        });

                    PreviousPoint = pt.Position;
                }
            }
            else
            {
                PreviousPoint = pt.Position;
            }
        }

        private async void Play(object sender, RoutedEventArgs e)
        {
            var images = new List<WriteableBitmap>();
            var loader = await ImageLoader.Create(MapViewModel.CurrentCameras);

            foreach (var frame in Map.MappedFrames)
            {
                var image = await loader.GetImage(frame);

                for (var i = 0; i < frame.ViewTime/33; i++)
                {
                    images.Add(image);
                }
            }

            RenderViewModel.Instance.Images = images;
            Frame.Navigate(typeof (Render));
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (MainPage));
        }
    }
}