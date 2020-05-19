using Envelopes.Common;
using Envelopes.Models;

namespace Envelopes.Pages.BudgetPage.CategoriesGrid
{

    public interface ICategoriesGridViewModelBase : IItemsViewModelBase<Category>
    {

    }
    public class CategoriesGridViewModel: ItemsViewModelBase<Category>, ICategoriesGridViewModelBase
    {

    }
}
