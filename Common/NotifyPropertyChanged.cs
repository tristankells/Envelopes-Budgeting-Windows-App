using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Envelopes.Common {
    /// <summary>
    /// View model base class with support for INotifyPropertyChanged and nothing else
    /// </summary>
    public class NotifyPropertyChanged : INotifyPropertyChanged {
        /// <summary>Raised when a property is changed</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="E:Aderant.PresentationFramework.NotifyPropertyChanged.PropertyChanged" /> event for property with the given <paramref name="propertyName" />.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if (propertyChanged == null) {
                return;
            }

            propertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Utility method to handle property change notification easily. Consume this like this:
        /// 
        ///   public string MyProperty{
        ///       get { return myProperty; }
        ///       set { SetField(ref myProperty, value); }
        ///   }
        /// </summary>
        protected bool SetPropertyValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(field, value)) {
                return false;
            }

            field = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
    }
}