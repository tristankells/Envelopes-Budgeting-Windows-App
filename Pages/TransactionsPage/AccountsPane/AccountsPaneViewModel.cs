using System.Collections.ObjectModel;
using System.Windows.Input;
using Envelopes.Common;
using Envelopes.Models;

namespace Envelopes.TransactionsPage.AccountsPane {
    public interface IAccountsPaneViewModel : IItemsViewModelBase<Account> {

    }

    public class AccountsPaneViewModel : ItemsViewModelBase<Account>, IAccountsPaneViewModel {

    }
}