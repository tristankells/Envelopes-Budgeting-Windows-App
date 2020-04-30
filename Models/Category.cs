using Envelopes.Common;

namespace Envelopes.Models {
    public class Category : Model
    {
        #region Fields

        private int id;
        private string name;
        private decimal budgeted;
        private decimal activity;
        private decimal total;

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

        #region Calculated Properties

        /// <summary>
        /// CALCULATED PROPERTY: The total of all transactions made against this category.
        /// </summary>
        public decimal Activity
        {
            get => activity;
            set => SetPropertyValue(ref activity, value, nameof(Activity));
        }

        /// <summary>
        /// CALCULATED PROPERTY: The amount of money budgeted towards a category. The balance of a category is Budgeted - the Total Sum of all accounts against that category.
        /// </summary>
        public decimal Total
        {
            get => total;
            set => SetPropertyValue(ref total, value, nameof(Total));
        }

        #endregion



    }
}