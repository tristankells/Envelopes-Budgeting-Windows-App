using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Envelopes.Common;
using Envelopes.Models;

namespace Envelopes.Pages.BudgetPage.CategoriesGrid {
    public interface ICategoriesGridViewModel : IItemsViewModelBase<Category> {
        decimal TotalAvailable { get; }
        public DelegateCommand CoverOverBudgetCommand { get; set; }
    }


    public class CategoriesGridViewModel : ItemsViewModelBase<Category>, ICategoriesGridViewModel {
        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
            OnPropertyChanged(nameof(TotalAvailable));
        }

        protected override void OnItemCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            OnPropertyChanged(nameof(TotalAvailable));
        }

        public decimal TotalAvailable {
            get { return ItemList.Select(c => c.Available).Sum(); }
        }

        public DelegateCommand CoverOverBudgetCommand { get; set; }
    }
}