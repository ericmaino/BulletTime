using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BulletTime.UI;
using BulletTime.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UniversalScratch
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CameraClientViewModel model;

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            model = ((CameraClientViewModel) DataContext);
            model.Initialize();
            model.SelectedResolution.Changed += SelectedResolution_Changed;
        }

        private void SelectedResolution_Changed(IProperty obj)
        {
            Capture.Source = model.Media;
        }
    }
}