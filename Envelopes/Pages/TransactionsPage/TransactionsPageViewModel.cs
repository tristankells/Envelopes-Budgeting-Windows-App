using Envelopes.Common;

namespace Envelopes.Pages.TransactionsPage {
    public interface ITransactionsPageViewModel : IViewModelBase {
    }

    public class TransactionsPageViewModel : NotifyPropertyChanged, ITransactionsPageViewModel {
    }
}