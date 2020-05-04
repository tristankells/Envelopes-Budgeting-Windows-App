using System.ComponentModel;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.Models;
using GongSolutions.Wpf.DragDrop;

namespace Envelopes.Pages.TransactionsPage.AccountsPane {
    public interface IAccountsPaneViewModel : IItemsViewModelBase<Account> {
    }

    public class AccountsPaneViewModel : ItemsViewModelBase<Account>, IAccountsPaneViewModel, IDropTarget {
        private readonly IDataService dataService;

        public AccountsPaneViewModel(IDataService dataService) {
            PropertyChanged += AccountsPaneViewModel_PropertyChanged;
            this.dataService = dataService;
        }

        public void DragOver(IDropInfo dropInfo) {
           // throw new System.NotImplementedException();
        }

        public void Drop(IDropInfo dropInfo) {
            throw new System.NotImplementedException();
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