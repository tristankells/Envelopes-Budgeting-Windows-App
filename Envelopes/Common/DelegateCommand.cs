using System;
using System.Windows.Input;

namespace Envelopes.Common {
    public sealed class DelegateCommand : ICommand {
        private readonly Func<bool> canExecuteCallback;
        private readonly Action executeCallback;

        public DelegateCommand(Action executeCallback) {
            this.executeCallback = executeCallback ?? throw new ArgumentNullException(nameof(executeCallback));
        }

        public DelegateCommand(Action executeCallback, Func<bool> canExecuteCallback)
            : this(executeCallback) {
            this.canExecuteCallback = canExecuteCallback ?? throw new ArgumentNullException(nameof(canExecuteCallback));
        }

        bool ICommand.CanExecute(object parameter) => CanExecute();

        void ICommand.Execute(object parameter) {
            Execute();
        }

        event EventHandler ICommand.CanExecuteChanged {
            add {
                if (canExecuteCallback == null) {
                    return;
                }

                CommandManager.RequerySuggested += value;
            }
            remove {
                if (canExecuteCallback == null) {
                    return;
                }

                CommandManager.RequerySuggested -= value;
            }
        }
        public bool CanExecute() => canExecuteCallback == null || canExecuteCallback();

        public void Execute() {
            executeCallback();
        }
    }
}