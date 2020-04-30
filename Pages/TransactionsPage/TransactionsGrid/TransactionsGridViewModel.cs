using System.Collections.ObjectModel;
using System.Windows.Input;
using Envelopes.Common;
using Envelopes.Models;

namespace Envelopes.TransactionsPage.TransactionsGrid {

    public interface ITransactionsGridViewModel : IItemsViewModelBase<AccountTransaction>
    {
        
    }

    public class TransactionsGridViewModel : ItemsViewModelBase<AccountTransaction>, ITransactionsGridViewModel {
        
    }
}