using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await ApplicationViewModel.Current.InitializeMapViewModel();
            Frame.Navigate(typeof (MapPath));
        }

        private async void ShowMapWithFrames(object sender, RoutedEventArgs e)
        {
            await ApplicationViewModel.Current.InitializeMapViewModelWithFrames();
            Frame.Navigate(typeof (MapPath));
        }
    }
}