namespace Envelopes.Models {
    public class Category : Model {
        #region Fields

        private int id;
        private string name;
        private decimal budgeted;
        private decimal activity;
        private decimal available;

        #endregion

        #region Properties

        /// <summary>
        ///     The unique identifier for the category
        /// </summary>
        public int Id {
            get => id;
            set => SetPropertyValue(ref id, value, nameof(Id));
        }

        /// <summary>
        ///     The name of the category. Should be unique.
        /// </summary>
        public string Name {
            get => name;
            set => SetPropertyValue(ref name, value, nameof(Name));
        }

        /// <summary>
        ///     The amount of money budgeted towards a category. The balance of a category is Budgeted - the Available Sum of all
        ///     accounts against that category.
        /// </summary>
        public decimal Budgeted {
            get => budgeted;
            set {
                SetPropertyValue(ref budgeted, value, nameof(Budgeted));
                OnPropertyChanged(nameof(Available));
            }
        }

        #endregion

        #region Calculated Properties

        /// <summary>
        ///     CALCULATED PROPERTY: The available of all transactions made against this category.
        /// </summary>
        public decimal Activity {
            get => activity;
            set => SetPropertyValue(ref activity, value, nameof(Activity));
        }

        /// <summary>
        ///     CALCULATED PROPERTY: The amount of money budgeted towards a category. The balance of a category is Budgeted - the
        ///     Available Sum of all accounts against that category.
        /// </summary>
        public decimal Available {
            get => budgeted + activity;
            set => SetPropertyValue(ref available, value, nameof(Available));
        }

        #endregion
    }
}