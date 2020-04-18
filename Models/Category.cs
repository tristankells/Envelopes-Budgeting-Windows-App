using Envelopes.Common;

namespace Envelopes.Models {
    public class Category : Model
    {
        #region Fields

        private int id;
        private string name;
        private decimal budgeted;

        #endregion

        #region Properties

        /// <summary>
        /// The unique identifier for the category
        /// </summary>
        public int Id {
            get => id;
            set => SetPropertyValue(ref id, value, nameof(Id));
        }

        /// <summary>
        /// The name of the category. Should be unique.
        /// </summary>
        public string Name {
            get => name;
            set => SetPropertyValue(ref name, value, nameof(Name));
        }

        /// <summary>
        /// The amount of money budgeted towards a category. The balance of a category is Budgeted - the Total Sum of all accounts against that category.
        /// </summary>
        public decimal Budgeted {
            get => budgeted;
            set => SetPropertyValue(ref budgeted, value, nameof(Budgeted));
        }

        #endregion
    }
}