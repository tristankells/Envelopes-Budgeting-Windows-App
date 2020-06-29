using System;
using System.Windows.Input;

namespace Envelopes.Common {
    /// <summary>
    /// Defines an <see cref="T:System.Windows.Input.ICommand" /> that executes callbacks (delegates) for both execute and can execute functionality.
    /// </summary>
    public sealed class DelegateCommand : ICommand {
        private readonly Func<bool> canExecuteCallback;
        private readonly Action executeCallback;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Aderant.PresentationFramework.Input.DelegateCommand" /> class.
        /// </summary>
        /// <param name="executeCallback">The <see cref="T:System.Action" /> to call when the command is executed.</param>
        public DelegateCommand(Action executeCallback) {
            this.executeCallback = executeCallback ?? throw new ArgumentNullException(nameof(executeCallback));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Aderant.PresentationFramework.Input.DelegateCommand" /> class.
        /// </summary>
        /// <param name="executeCallback">The <see cref="T:System.Action" /> to call when the command is executed.</param>
        /// <param name="canExecuteCallback">The function to call when the command needs to evaluate whether it can execute.</param>
        public DelegateCommand(Action executeCallback, Func<bool> canExecuteCallback)
            : this(executeCallback) {
            this.canExecuteCallback = canExecuteCallback ?? throw new ArgumentNullException(nameof(canExecuteCallback));
        }

        /// <summary>Determines whether this command can execute.</summary>
        /// <returns>
        /// 	<c>true</c> if this command can execute; otherwise, <c>false</c>.
        /// </returns>
        public bool CanExecute() {
            return canExecuteCallback == null || this.canExecuteCallback();
        }

        bool ICommand.CanExecute(object parameter) {
            return CanExecute();
        }

        /// <summary>Executes the command.</summary>
        public void Execute() {
            executeCallback();
        }

        void ICommand.Execute(object parameter) {
            Execute();
        }

        event EventHandler ICommand.CanExecuteChanged {
            add {
                if (canExecuteCallback == null)
                    return;
                CommandManager.RequerySuggested += value;
            }
            remove {
                if (canExecuteCallback == null)
                    return;
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}