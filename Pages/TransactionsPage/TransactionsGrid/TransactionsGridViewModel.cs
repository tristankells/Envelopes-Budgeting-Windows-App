using System.Collections.ObjectModel;
using Envelopes.Common;
using Envelopes.Models;

namespace Envelopes.Pages.TransactionsPage.TransactionsGrid {

    public interface ITransactionsGridViewModel : IItemsViewModelBase<AccountTransaction> {
        public ObservableCollection<Category> Categories { get; }
        public ObservableCollection<Account> Accounts { get; }
        
    }

    public class TransactionsGridViewModel : ItemsViewModelBase<AccountTransaction>, ITransactionsGridViewModel {
        private ObservableCollection<Category> categories;
        private ObservableCollection<Account> accounts;

        

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
                OnPropertyChanged(nameof(Categories));
            }
        }
    }
}