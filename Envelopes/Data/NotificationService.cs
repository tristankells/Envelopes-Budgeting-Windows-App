using System;
using Envelopes.Models;

namespace Envelopes.Data {
    public interface INotificationService {
        public event EventHandler OnCategoryBudgetedChanged;
        public event EventHandler OnTransactionBalanceChanged;
        public event EventHandler OnActiveAccountChanged;
        public event EventHandler OnShowAllTransactionsExecuted;
        void NotifyActiveAccountChanged(Account account);
        void NotifyCategoryBudgetedChanged();
        void NotifyTransactionBalanceChanged();
        void NotifyShowAllTransactionsExecuted();
        void NotifyTransactionsImportCompleted();
    }

    public class NotificationService : INotificationService {
        public event EventHandler OnCategoryBudgetedChanged;
        public event EventHandler OnTransactionBalanceChanged;
        public event EventHandler OnActiveAccountChanged;
        public event EventHandler OnShowAllTransactionsExecuted;

        public void NotifyActiveAccountChanged(Account account) {
            OnActiveAccountChanged?.Invoke(account, new EventArgs());
        }

        public void NotifyCategoryBudgetedChanged() {
            OnCategoryBudgetedChanged?.Invoke(null, new EventArgs());
        }

        public void NotifyTransactionBalanceChanged() {
            OnTransactionBalanceChanged?.Invoke(null, new EventArgs());
        }

        public void NotifyShowAllTransactionsExecuted() {
            OnShowAllTransactionsExecuted?.Invoke(null, new EventArgs());
        }

        public void NotifyTransactionsImportCompleted() {
            OnShowAllTransactionsExecuted?.Invoke(null, new EventArgs());
        }
    }
}