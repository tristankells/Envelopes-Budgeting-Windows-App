﻿using Envelopes.Models;

namespace Tests.Envelopes {
    public static class TestDataSetup {
        public static Account CreateAccount() =>
            new Account {
                Id = 1,
                Name = "Kiwibank"
            };

        public static Category CreateCategory() =>
            new Category {
                Id = 1,
                Name = "Savings",
                Budgeted = 10.0M
            };

        public static AccountTransaction CreateAccountTransaction() =>
            new AccountTransaction {
                AccountId = 1,
                CategoryId = 1,
                Inflow = 100.0M,
                Outflow = 0.0M,
                Payee = "Aderant"
            };
    }
}