using System.ComponentModel;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.Models;

namespace Envelopes.Pages.TransactionsPage.AccountsPane {
    public interface IAccountsPaneViewModel : IItemsViewModelBase<Account> {
    }

    public class AccountsPaneViewModel : ItemsViewModelBase<Account>, IAccountsPaneViewModel {
        private readonly IDataService dataService;

        public AccountsPaneViewModel(IDataService dataService) {
            PropertyChanged += AccountsPaneViewModel_PropertyChanged;
            this.dataService = dataService;
        }

        private void AccountsPaneViewModel_PropertyChanged(object sender,
          PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(SelectedItem):
                    dataService.SetActiveAccount(SelectedItem);
                    break;
            }
        }
    }
}