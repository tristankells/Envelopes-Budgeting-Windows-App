using Envelopes.Common;
using Envelopes.TransactionsPage.AccountsPane;

namespace Envelopes.TransactionsPage {
    public interface ITransactionsPageViewModel : IViewModel {
    }

    public class TransactionsPageViewModel : NotifyPropertyChanged, ITransactionsPageViewModel {
    }
}