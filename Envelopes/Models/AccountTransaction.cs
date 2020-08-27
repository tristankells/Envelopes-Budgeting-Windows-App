using System;
using Envelopes.Common;

namespace Envelopes.Models {
    public class AccountTransaction : Model {
        public string AccountName {
            get => accountName;
            set => SetPropertyValue(ref accountName, value, nameof(AccountName));
        }

        #region Fields

        private int id;
        private int accountId;
        private DateTime date;
        private string payee;
        private int categoryId;
        private string memo;
        private decimal outflow;
        private decimal inflow;
        private string accountName;

        #endregion

        #region Properties

        public int Id {
            get => id;
            set => SetPropertyValue(ref id, value, nameof(Id));
        }

        public int AccountId {
            get => accountId;
            set => SetPropertyValue(ref accountId, value, nameof(AccountId));
        }

        public DateTime Date {
            get => date;
            set => SetPropertyValue(ref date, value, nameof(Date));
        }

        public string Payee {
            get => payee;
            set => SetPropertyValue(ref payee, value, nameof(Payee));
        }

        public string Memo {
            get => memo;
            set => SetPropertyValue(ref memo, value, nameof(Memo));
        }

        public decimal Outflow {
            get => outflow;
            set => SetPropertyValue(ref outflow, value, nameof(Outflow));
        }

        public decimal Inflow {
            get => inflow;
            set => SetPropertyValue(ref inflow, value, nameof(Inflow));
        }

        public decimal Total {
            get => inflow - outflow;
            set => SetPropertyValue(ref inflow, value, nameof(Total));
        }

        public int CategoryId {
            get => categoryId;
            set => SetPropertyValue(ref categoryId, value, nameof(CategoryId));
        }

        #endregion
    }
}