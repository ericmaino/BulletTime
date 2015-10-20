using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using BulletTime.RemoteControl;

namespace BulletTime.UI
{
    public class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is CameraClientState)
            {
                switch ((CameraClientState) value)
                {
                    case CameraClientState.Idle:
                        return new SolidColorBrush(Colors.Gray);
                    case CameraClientState.Preparing:
                        return new SolidColorBrush(Colors.Yellow);
                    case CameraClientState.Recording:
                        return new SolidColorBrush(Colors.Red);
                    case CameraClientState.Uploading:
                        return new SolidColorBrush(Colors.CadetBlue);
                }
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}