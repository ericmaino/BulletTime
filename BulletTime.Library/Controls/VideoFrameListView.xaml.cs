using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BulletTime.Controls
{
    public sealed partial class VideoFrameListView : UserControl
    {
        private double _width;

        public VideoFrameListView()
        {
            this.InitializeComponent();
            ThisView.Items.VectorChanged += ItemsChanged;
            ThisView.Loaded += MyListView_Loaded;
            ThisView.SizeChanged += MyListView_SizeChanged;
        }

        public double ItemWidth
        {
            get { return _width; }
            set
            {
                _width = value;
                this.InvokePropertyChanged("ItemWidth");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(object), typeof(VideoFrameListView), new PropertyMetadata(null, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            VideoFrameListView view = dependencyObject as VideoFrameListView;

            if (view != null)
            {
                view.ItemsSource = args.NewValue;
            }
        }

        public object ItemsSource
        {
            get { return ThisView.ItemsSource; }
            set { ThisView.ItemsSource = value; }
        }

        public void InvokePropertyChanged(string propertyName)
        {
            var action = PropertyChanged;

            if (action != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void MyListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ItemWidth = ThisView.ActualWidth / ThisView.Items.Count;
        }

        private void MyListView_Loaded(object sender, RoutedEventArgs e)
        {
            ItemWidth = ThisView.ActualWidth / ThisView.Items.Count;
        }

        private void ItemsChanged(IObservableVector<object> sender, IVectorChangedEventArgs @event)
        {
            ItemWidth = ThisView.ActualWidth / ThisView.Items.Count;
        }
    }
}
