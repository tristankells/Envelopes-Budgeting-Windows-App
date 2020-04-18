using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Envelopes.Common;
using Envelopes.Models;

namespace Envelopes.TransactionsPage.AccountsPane {
    public interface IAccountsPaneViewModel : IViewModel {
        public ObservableCollection<Account> AccountsList { get; }
        public ICommand AddAccountCommand { get; set; }
        public ICommand DeleteAccountCommand { get; set; }
        public Account SelectedAccount { get; set; }
        public void AddAccount(Account account);
        public bool RemoveAccount(Account account);

        public event PropertyChangedEventHandler OnAccountNameUpdated;
    }

    public class AccountsPaneViewModel : NotifyPropertyChanged, IAccountsPaneViewModel {
        private ObservableCollection<Account> accountsList;
        public ICommand AddAccountCommand { get; set; }
        public ICommand DeleteAccountCommand { get; set; }

        public event PropertyChangedEventHandler OnAccountNameUpdated;

        private Account selectedAccount;
        public Account SelectedAccount {
            get => selectedAccount;
            set => SetPropertyValue(ref selectedAccount, value, nameof(SelectedAccount));
        }

        public AccountsPaneViewModel() {
            accountsList = new ObservableCollection<Account>();
        }

        public ObservableCollection<Account> AccountsList {
            get => accountsList;

            private set {
                if (accountsList != null) {
                    accountsList.CollectionChanged -= TransactionsListOnCollectionChanged;
                }

                accountsList = value;
                accountsList.CollectionChanged += TransactionsListOnCollectionChanged;

                OnPropertyChanged(nameof(AccountsList));
            }
        }

        public void AddAccount(Account account) {
            account.PropertyChanged += OnItemPropertyChanged;
            accountsList.Add(account);
        }

        public bool RemoveAccount(Account account) {
            if (account == null) return false;
            account.PropertyChanged -= OnItemPropertyChanged;
            return accountsList.Remove(account);
        }


        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(Account.Name):
                    OnAccountNameUpdated?.Invoke(sender, e);
                    break;

            }
        }

        private void TransactionsListOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

        }
    }
}