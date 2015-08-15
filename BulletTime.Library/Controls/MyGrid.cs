using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BulletTime.Controls
{
    public class MyGrid : Grid
    {
        public MyGrid()
        {
            Debug.Write("MyGrid Created");
            SizeChanged += MyGrid_SizeChanged;
            Loaded += MyGrid_Loaded;
        }

        private void MyGrid_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void MyGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }
    }
}