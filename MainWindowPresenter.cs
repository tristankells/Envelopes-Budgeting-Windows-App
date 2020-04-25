using System.Windows;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.TransactionsPage;

namespace Envelopes {
    public interface IMainWindowPresenter {
        public Window MainWindow { get; }
    }

    public class MainWindowPresenter : Presenter, IMainWindowPresenter {
        private readonly ITransactionsPagePresenter transactionsPagePresenter;
        private readonly MainWindow view;
        private readonly IMainWindowViewModel viewModel;
        private readonly IDataService dataService;

        public MainWindowPresenter(MainWindow view, 
            IMainWindowViewModel viewModel,
            ITransactionsPagePresenter transactionsPagePresenter, 
            IDataService dataService) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.transactionsPagePresenter = transactionsPagePresenter;
            this.dataService = dataService;

            BindEvents();
            BindCommands();
        }

        private void BindCommands() {
            viewModel.SaveBudgetCommand = new DelegateCommand(ExecuteSaveBudget, CanSaveBudget);
        }

        private bool CanSaveBudget() => true;

        private async void ExecuteSaveBudget() {
            await dataService.SaveBudget();
        }

        private void BindEvents() {
            view.Loaded += View_Loaded;
            view.Closing += View_Closing;
        }

        private async  void View_Closing(object sender, System.EventArgs e) {
             await dataService.SaveBudget();
        }

        private async void View_Loaded(object sender, RoutedEventArgs e) {
            await dataService.LoadApplicationData();
            viewModel.CurrentPage = transactionsPagePresenter.GetPageView();
        }

        public Window MainWindow => view;
    }
}