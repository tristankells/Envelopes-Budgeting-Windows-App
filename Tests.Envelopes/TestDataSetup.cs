using System.Collections.Generic;
using Envelopes.Models;

namespace Tests.Envelopes {
    public static class TestDataSetup {
        public static Account CreateAccount() {
            return new Account {
                Id = 1,
                Name = "Kiwibank"
            };
        }

        public static Category CreateCategoryOne() {
            return new Category() {
                Id = 1,
                Name = "Savings",
                Budgeted = 10.0M
            };
        }

        public static Category CreateCategoryTwo() {
            return new Category() {
                Id = 2,
                Name = "Eating Out",
                Budgeted = 20.0M
            };
        }

        public static AccountTransaction CreateAccountTransaction_WithSingleCategoryTransaction() {
            return new AccountTransaction() {
                AccountId = 1,
                Id = 1,
                CategoryTransactions = new List<CategoryTransaction> {
                    new CategoryTransaction() {
                        CategoryId = 1,
                        Inflow = 10M
                    }
                }
            };
        }

        public static AccountTransaction CreateAccountTransaction_WithMultipleCategoryTransactions() {
            return new AccountTransaction() {
                AccountId = 1,
                Id = 1,
                CategoryTransactions = new List<CategoryTransaction> {
                    new CategoryTransaction() {
                        CategoryId = 1,
                        Inflow = 10M
                    },
                    new CategoryTransaction() {
                        CategoryId = 2,
                        Inflow = 20M
                    }
                }
            };
        }
    }
}