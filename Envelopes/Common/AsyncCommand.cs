using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Envelopes.Common {
    public interface IAsyncCommand : ICommand {
        Task ExecuteAsync();
        bool CanExecute();
    }

    public class AsyncCommand : IAsyncCommand {
        private readonly Func<bool> canExecute;
        private readonly Func<Task> execute;

        private bool isExecuting;

        public AsyncCommand(
            Func<Task> execute,
            Func<bool> canExecute = null) {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute() => !isExecuting && (canExecute?.Invoke() ?? true);

        public async Task ExecuteAsync() {
            if (CanExecute()) {
                try {
                    isExecuting = true;
                    await execute();
                }
                finally {
                    isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged() {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region Explicit implementations

        bool ICommand.CanExecute(object parameter) => CanExecute();

        void ICommand.Execute(object parameter) {
#pragma warning disable 4014
            ExecuteAsync();
#pragma warning restore 4014
        }

        #endregion
    }
}