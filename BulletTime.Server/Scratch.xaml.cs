using BulletTime.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            this.InitializeComponent();
            this.Loaded += Scratch_Loaded;
        }

        private async void Scratch_Loaded(object sender, RoutedEventArgs e)
        {
            var loader = await ImageLoader.Create(new Models.RemoteCameraModel(IPAddress.Parse("192.168.1.131"), 0, null));
            var frames = await loader.GetFrameImages();
            Images.Source = frames;
        }
    }
}
