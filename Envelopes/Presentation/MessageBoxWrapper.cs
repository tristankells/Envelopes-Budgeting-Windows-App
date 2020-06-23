using System.Windows;

namespace Envelopes.Presentation {
    public interface IMessageBoxWrapper {
        MessageBoxResult Show(string s, string deleteAccount, MessageBoxButton yesNoCancel, MessageBoxImage warning);
    }

    public class MessageBoxWrapper : IMessageBoxWrapper {
        public MessageBoxResult Show(string s, string deleteAccount, MessageBoxButton yesNoCancel, MessageBoxImage warning) {
            return MessageBox.Show(s, deleteAccount, yesNoCancel, warning);
        }
    }
}