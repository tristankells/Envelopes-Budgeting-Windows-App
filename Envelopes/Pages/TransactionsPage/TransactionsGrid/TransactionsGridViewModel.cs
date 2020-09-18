using System.Collections.ObjectModel;
using Envelopes.Common;
using Envelopes.Models;

namespace Envelopes.Pages.TransactionsPage.TransactionsGrid {
    public interface ITransactionsGridViewModel : IItemsViewModelBase<AccountTransaction> {
        public ObservableCollection<AccountTransaction> AccountTransactions { get; }
        public ObservableCollection<Category> Categories { get; }
        public ObservableCollection<Account> Accounts { get; }
        IAsyncCommand ImportTransactionsCommand { get; set; }
    }

    public class TransactionsGridViewModel : ItemsViewModelBase<AccountTransaction>, ITransactionsGridViewModel {
        private ObservableCollection<Account> accounts;
        private ObservableCollection<Category> categories;

        public IAsyncCommand ImportTransactionsCommand { get; set; }

        public ObservableCollection<AccountTransaction> AccountTransactions => ItemList;

        public ObservableCollection<Category> Categories {
            get => categories ??= new ObservableCollection<Category>();
            private set {
                categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public ObservableCollection<Account> Accounts {
            get => accounts ??= new ObservableCollection<Account>();
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