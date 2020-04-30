using System.Windows;
using Envelopes.CategoriesPage.CategoriesGrid;
using Envelopes.Common;
using Envelopes.Pages.BudgetPage.CategoriesGrid;
using Envelopes.TransactionsPage;
using Envelopes.TransactionsPage.AccountsPane;

namespace Envelopes.BudgetPage {
    public interface IBudgetPagePresenter
    {
        public BudgetPageView GetPageView();
    }

    public class BudgetPagePresenter : Presenter, IBudgetPagePresenter
    {
        #region Fields

        private BudgetPageView view;
        private ITransactionsPageViewModelBase viewModelBase;
        private ICategoriesGridPresenter categoriesGridPresenter;

        #endregion

        public BudgetPagePresenter(BudgetPageView view, ITransactionsPageViewModelBase viewModelBase, ICategoriesGridPresenter categoriesGridPresenter) : base(view, viewModelBase) {
            this.view = view;
            this.viewModelBase = viewModelBase;
            this.categoriesGridPresenter = categoriesGridPresenter;

            BindEvents();
        }

        private void BindEvents()
        {
            view.Loaded += View_Loaded;
        }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            view.CategoriesGridControl.Content = categoriesGridPresenter.GetView();
        }

        public BudgetPageView GetPageView() => view;
    }
}