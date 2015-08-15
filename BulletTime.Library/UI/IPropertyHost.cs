using System.ComponentModel;

namespace BulletTime.UI
{
    public interface IPropertyHost : INotifyPropertyChanged
    {
        void InvokePropertyChanged(string propertyName);
    }
}