using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BulletTime.Server
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Render : Page
    {
        private readonly DispatcherTimer timer;
        private int frameIndex;
        private RenderViewModel model;

        public Render()
        {
            InitializeComponent();
            Loaded += Render_Loaded;
            timer = new DispatcherTimer();
            frameIndex = 0;
        }

        private void Render_Loaded(object sender, RoutedEventArgs e)
        {
            model = DataContext as RenderViewModel;
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(33);
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            Animation.Source = model.Images[frameIndex++%model.Images.Count];
        }

        private void GoHome(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (MainPage));
        }
    }
}