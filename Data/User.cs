using System.Collections.Generic;
using System;

namespace Bank.Data
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string MailingAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int UnreadMessages { get; set; } = 5;

        public string Username => $"{FirstName.ToLower()[0]}{LastName.ToLower()}";

        public List<Account> Accounts { get; set; } = new List<Account>();

        public class InboxMessage
        {
            public string Id => $"{Category}-{Subject}-{Body}";
            public string Subject { get; set; }
            public string Body { get; set; }
            public string Category { get; set; }
            public DateTime Date { get; set; }
            public bool IsUnread { get; set; }

            public string FormattedDate => Date.ToString("MMM dd, yyyy");

            public InboxMessage(
                string subject,
                string body,
                string category,
                DateTime date,
                bool isUnread = true)
            {
                Subject = subject;
                Body = body;
                Category = category;
                Date = date;
                IsUnread = isUnread;
            }
        }

        // Shared ExternalAccount class
        public class ExternalAccount
        {
            public string AccountHolder { get; set; }
            public string AccountNumber { get; set; }
            public string AccountType { get; set; }
            public string BankName { get; set; }
            public string RoutingNumber { get; set; }
            public DateTime AddedAt { get; set; } = DateTime.Now;

            public string Last4Digits =>
                !string.IsNullOrEmpty(AccountNumber) && AccountNumber.Length >= 4
                ? AccountNumber.Substring(AccountNumber.Length - 4)
                : "0000";

            public string Name => $"{AccountType}";
        }

        public class Account
        {
            public string Name { get; set; }

            public float CurrentBalance
            {
                get
                {
                    var total = BaseBalance;
                    foreach (Transaction t in PastTransactions)
                        total += t.Amount;
                    return total;
                }
            }

            public float BaseBalance { get; set; }
            public int Last4Digits { get; set; }
            public List<Transaction> PastTransactions { get; set; } = new List<Transaction>();

            public string FormattedBalance => string.Format("${0:n}", CurrentBalance);

            public class Transaction
            {
                public string Description { get; set; }
                public string Type { get; set; }
                public float Amount { get; set; }

                public int DaysBack { get; set; }

                public DateTime Date => DateTime.Now.AddDays(-DaysBack);

                public string FormattedAmount => string.Format("${0:n}", Amount);

                public Transaction(string description, string type, float amount, int daysBack = -1)
                {
                    Description = description;
                    Type = type;
                    Amount = amount;
                    DaysBack = daysBack == -1 ? new Random().Next(0, 30) : daysBack;
                }
            }
        }
    }
}
