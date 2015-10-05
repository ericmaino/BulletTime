using System.Net;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BulletTime.Models;
using BulletTime.Rendering;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BulletTime
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scratch : Page
    {
        public Scratch()
        {
            InitializeComponent();
            Loaded += Scratch_Loaded;
        }

        private async void Scratch_Loaded(object sender, RoutedEventArgs e)
        {
            var loader = await ImageLoader.Create(new RemoteCameraModel(IPAddress.Parse("192.168.1.131"), 0, null));
            var frames = await loader.GetFrameImages();
            Images.Source = frames;
        }
    }
}