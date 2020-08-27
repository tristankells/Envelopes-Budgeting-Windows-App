using Envelopes.Common;
using Envelopes.Models;

namespace Envelopes.Pages.BudgetPage.CategoriesGrid {
    public interface ICategoriesGridViewModel : IItemsViewModelBase<Category> {
    }

    public class CategoriesGridViewModel : ItemsViewModelBase<Category>, ICategoriesGridViewModel {
    }
}