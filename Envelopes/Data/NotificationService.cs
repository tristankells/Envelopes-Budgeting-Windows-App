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
    }

    public class NotificationService : INotificationService {
        public event EventHandler OnCategoryBudgetedChanged;
        public event EventHandler OnTransactionBalanceChanged;
        public event EventHandler OnActiveAccountChanged;
        public event EventHandler OnShowAllTransactionsExecuted;

        public void NotifyActiveAccountChanged(Account account) {
            OnActiveAccountChanged?.Invoke(account, null);
        }

        public void NotifyCategoryBudgetedChanged() {
            OnCategoryBudgetedChanged?.Invoke(null, null);
        }

        public void NotifyTransactionBalanceChanged() {
            OnTransactionBalanceChanged?.Invoke(null, null);
        }

        public void NotifyShowAllTransactionsExecuted() {
            OnShowAllTransactionsExecuted?.Invoke(null, null);
        }
    }
}