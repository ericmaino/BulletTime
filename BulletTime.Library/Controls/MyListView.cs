using System.ComponentModel;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BulletTime.UI;

namespace BulletTime.Controls
{
    public class MyListView : ListView, IPropertyHost
    {
        public MyListView()
        {
            ItemHeight = this.NewProperty(x => x.ItemHeight);
            Items.VectorChanged += ItemsChanged;
            Loaded += MyListView_Loaded;
            SizeChanged += MyListView_SizeChanged;
        }

        public Property<double> ItemHeight { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(string propertyName)
        {
            var changed = PropertyChanged;

            if (changed != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private void MyListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ItemHeight.Value = ActualHeight / Items.Count;
        }

        private void MyListView_Loaded(object sender, RoutedEventArgs e)
        {
            ItemHeight.Value = ActualHeight / Items.Count;
        }

        private void ItemsChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            ItemHeight.Value = ActualHeight / Items.Count;
        }
    }
}