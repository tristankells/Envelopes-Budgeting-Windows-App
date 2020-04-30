using Envelopes.Common;
using Envelopes.TransactionsPage.AccountsPane;

namespace Envelopes.TransactionsPage {
    public interface ITransactionsPageViewModelBase : IViewModelBase {
    }

    public class TransactionsPageViewModelBase : NotifyPropertyChanged, ITransactionsPageViewModelBase {
    }
}