using System;
using System.Collections.Generic;
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
        private string payeeName;
        private string accountName;
        private List<CategoryTransaction> categoryTransactions;

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

        public decimal Total {
            get => inflow - outflow;
            set => SetPropertyValue(ref inflow, value, nameof(Total));
        }

        public int CategoryId {
            get => categoryId;
            set => SetPropertyValue(ref categoryId, value, nameof(CategoryId));
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

        public List<CategoryTransaction> CategoryTransactions {
            get => categoryTransactions ??= new List<CategoryTransaction>();
            set => SetPropertyValue(ref categoryTransactions, value, nameof(CategoryTransactions));
        }
    }

    public class CategoryTransaction : Model {
        private int accountTransactionId;
        private AccountTransaction accountTransaction;

        private int categoryId;
        private Category category;

        private decimal outflow;
        private decimal inflow;


        public int CategoryId {
            get => categoryId;
            set => SetPropertyValue(ref categoryId, value, nameof(CategoryId));
        }

        public decimal Outflow {
            get => outflow;
            set => SetPropertyValue(ref outflow, value, nameof(Outflow));
        }

        public decimal Inflow {
            get => inflow;
            set => SetPropertyValue(ref inflow, value, nameof(Inflow));
        }
}
}