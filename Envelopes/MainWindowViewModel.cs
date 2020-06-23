using Envelopes.Common;
using System.Windows;
using System.Windows.Input;


namespace Envelopes {
    public interface IMainWindowViewModel : IViewModelBase {
        public FrameworkElement CurrentPage { get; set; }
        public ICommand NavigateToTransactionsPageCommand { get; set; }
        public ICommand NavigateToBudgetPageCommand { get; set; }
        public ICommand SaveBudgetCommand { get; set; }
        public decimal RemainingBalanceToBudget { get; set; }
    }

    public class MainWindowViewModel : NotifyPropertyChanged, IMainWindowViewModel {
        #region Fields

        private FrameworkElement currentPage;
        private decimal remainingBalanceToBudget;

        #endregion

        public ICommand NavigateToTransactionsPageCommand { get; set; }
        public ICommand NavigateToBudgetPageCommand { get; set; }
        public ICommand SaveBudgetCommand { get; set; }

        public decimal RemainingBalanceToBudget {
            get => remainingBalanceToBudget;
            set => SetPropertyValue(ref remainingBalanceToBudget, value, nameof(RemainingBalanceToBudget));
        }

        public FrameworkElement CurrentPage {
            get => currentPage;
            set => SetPropertyValue(ref currentPage, value, nameof(CurrentPage));
        }
    }
}