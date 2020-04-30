using System;
using Envelopes.Common;

namespace Envelopes.Models
{
    public class AccountTransaction : Model
    {
        #region Fields

        private int id;
        private int accountId;
        private DateTime date;
        private int payeeId;
        private int categoryId;
        private string memo;
        private decimal outflow;
        private decimal inflow;

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

        public int PayeeId {
            get => payeeId;
            set => SetPropertyValue(ref payeeId, value, nameof(PayeeId));
        }

        public int CategoryId {
            get => categoryId;
            set => SetPropertyValue(ref categoryId, value, nameof(CategoryId));
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

        #endregion
    }
}