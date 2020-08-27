using System.Windows;

namespace Envelopes.Common {
    public interface IView {
        object DataContext { set; }

        public event RoutedEventHandler Loaded;
        public event RoutedEventHandler Unloaded;
    }
}