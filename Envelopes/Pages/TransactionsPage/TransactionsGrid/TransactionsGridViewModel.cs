using System.Collections.ObjectModel;
using Envelopes.Common;
using Envelopes.Models;

namespace Envelopes.Pages.TransactionsPage.TransactionsGrid {
    public interface ITransactionsGridViewModel : IItemsViewModelBase<AccountTransaction> {
        public ObservableCollection<Category> Categories { get; }
        public ObservableCollection<Account> Accounts { get; }
    }

    public class TransactionsGridViewModel : ItemsViewModelBase<AccountTransaction>, ITransactionsGridViewModel {
        private ObservableCollection<Account> accounts;
        private ObservableCollection<Category> categories;

        public TransactionsGridViewModel() {
            Categories = new ObservableCollection<Category>();
            Accounts = new ObservableCollection<Account>();
        }

        public ObservableCollection<Category> Categories {
            get => categories;
            private set {
                categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public ObservableCollection<Account> Accounts {
            get => accounts;
            private set {
                accounts = value;
                OnPropertyChanged(nameof(Accounts));
            }
        }

        public new void AddItem(AccountTransaction item) {
            item.PropertyChanged += OnItemPropertyChanged;
            ItemList.Insert(0, item);
        }
    }
}