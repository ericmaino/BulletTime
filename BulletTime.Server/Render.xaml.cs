using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BulletTime.Server
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Render : Page
    {
        private readonly DispatcherTimer timer;
        private RenderViewModel model;
        private int frameIndex;

        public Render()
        {
            this.InitializeComponent();
            this.Loaded += Render_Loaded;
            timer = new DispatcherTimer();
            frameIndex = 0;
        }

        private void Render_Loaded(object sender, RoutedEventArgs e)
        {
            model = this.DataContext as RenderViewModel;
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(33);
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            this.Animation.Source = model.Images[frameIndex++ % model.Images.Count];
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }

    public class RenderViewModel
    {
        public static RenderViewModel Instance { get; set; }

        static RenderViewModel()
        {
            Instance = new RenderViewModel();
        }

        public RenderViewModel()
        {
            if (Instance != null)
            {
                this.Images = Instance.Images;
            }
            else
            {
                this.Images = new List<WriteableBitmap>();
            }
        }

        public IList<WriteableBitmap> Images { get; set; }
    }
}
