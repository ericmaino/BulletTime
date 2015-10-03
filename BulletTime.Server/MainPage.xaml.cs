using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BulletTime.Models;
using BulletTime.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BulletTime.Server
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ((ServerViewModel) DataContext).Register.Execute(null);
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MapViewModel.CurrentCameras = ((ServerViewModel) DataContext).Cameras.Source as IEnumerable<RemoteCameraModel>;
            Frame.Navigate(typeof (MapPath));
        }
    }
}