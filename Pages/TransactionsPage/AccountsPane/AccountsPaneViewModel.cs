using System;
using System.ComponentModel;
using Envelopes.Common;
using Envelopes.Models;
using GongSolutions.Wpf.DragDrop;

namespace Envelopes.Pages.TransactionsPage.AccountsPane {
    public interface IAccountsPaneViewModel : IItemsViewModelBase<Account> {
        decimal AccountsTotalBalance { get; set; }
        DelegateCommand ShowAllTransactionsCommand { get; set; }
        public event EventHandler SelectedAccountChanged;
    }

    public class AccountsPaneViewModel : ItemsViewModelBase<Account>, IAccountsPaneViewModel, IDropTarget {
        public DelegateCommand ShowAllTransactionsCommand { get; set; }
        public event EventHandler SelectedAccountChanged;
        private decimal accountsTotalBalance;
        public decimal AccountsTotalBalance {
            get => accountsTotalBalance;
            set => SetPropertyValue(ref accountsTotalBalance, value, nameof(AccountsTotalBalance));
        }

        public AccountsPaneViewModel() {
            PropertyChanged += AccountsPaneViewModel_PropertyChanged;
        }

        public void DragOver(IDropInfo dropInfo) {
           // throw new System.NotImplementedException();
        }

        public void Drop(IDropInfo dropInfo) {
            throw new System.NotImplementedException();
        }

        private void AccountsPaneViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(SelectedItem):
                    SelectedAccountChanged?.Invoke(SelectedItem, e);
                    break;
            }
        }
    }
}