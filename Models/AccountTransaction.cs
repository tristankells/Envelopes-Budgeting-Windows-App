using System;
using Envelopes.Common;

namespace Envelopes.Models {
    public class AccountTransaction : Model {
        #region Fields

        private int id;
        private int accountId;
        private DateTime date;
        private int payeeId;
        private int categoryId;
        private string memo;
        private decimal outflow;
        private decimal inflow;

        private Category category;

        private string payeeName;
        private string categoryName;
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

        public int PayeeId {
            get => payeeId;
            set => SetPropertyValue(ref payeeId, value, nameof(PayeeId));
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

        public int CategoryId {
            get => categoryId;
            set => SetPropertyValue(ref categoryId, value, nameof(CategoryId));
        }

        public Category Category {
            get => category;
            set => SetPropertyValue(ref category, value, nameof(Category));
        }

        public string CategoryName {
            get => categoryName;
            set => SetPropertyValue(ref categoryName, value, nameof(CategoryName));
        }

        #endregion

        public string PayeeName {
            get => payeeName;
            set => SetPropertyValue(ref payeeName, value, nameof(PayeeName));
        }

        public string AccountName {
            get => accountName;
            set => SetPropertyValue(ref accountName, value, nameof(AccountName));
        }
    }
}