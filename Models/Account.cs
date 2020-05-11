using Envelopes.Common;
using Envelopes.TransactionsPage.AccountsPane;

namespace Envelopes.Models {
    public class Account : Model
    {
        #region Fields

        private int id;
        private string name;
        private decimal total;

        #endregion

        #region Properties

        public int Id {
            get => id;
            set => SetPropertyValue(ref id, value, nameof(Id));
        }

        public string Name {
            get => name;
            set => SetPropertyValue(ref name, value, nameof(Name));
        }

        public decimal Total {
            get => total;
            set => SetPropertyValue(ref total, value, nameof(Total));
        }

        #endregion
    }
}