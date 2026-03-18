using System;
using System.Linq;
using System.Collections.Generic;
using System.Timers;

namespace Bank.Data
{
    public partial class BrainService
    {
        public bool LoggedIn { get; set; } = false;
        public string CurrentUser { get; set; }
        public DateTime LoggedInAt { get; set; }
        public string InactivityDisplay = "none";

        // Persistent storage for external accounts per user
        private Dictionary<string, List<User.ExternalAccount>> UserExternalAccounts
            = new Dictionary<string, List<User.ExternalAccount>>();

        public List<User.ExternalAccount> GetExternalAccounts(string username)
        {
            if (UserExternalAccounts.ContainsKey(username))
                return UserExternalAccounts[username];
            return new List<User.ExternalAccount>();
        }

        public void AddExternalAccount(string username, User.ExternalAccount account)
        {
            if (!UserExternalAccounts.ContainsKey(username))
                UserExternalAccounts[username] = new List<User.ExternalAccount>();

            UserExternalAccounts[username].Add(account);
        }

        private List<User> Users = new List<User>
        {
            new User {
                FirstName = "Linda",
                LastName = "CROSTIC",
                UnreadMessages = 5,
                Accounts = new List<User.Account>
                {
                    new User.Account
                    {
                        Name = "Premium Checking",
                        BaseBalance = 0f,
                        Last4Digits = 4278,
                        PastTransactions = new List<User.Account.Transaction>
                        {
                            new User.Account.Transaction("Oxfam Organization", "Deposit", 200000f)
                        }
                    },
                    new User.Account
                    {
                        Name = "Savings Account",
                        BaseBalance = 26594.00f,
                        Last4Digits = 1930,
                        PastTransactions = new List<User.Account.Transaction>()
                    },
                    new User.Account
                    {
                        Name = "Bronze Credit Card",
                        BaseBalance = 1645.71f,
                        Last4Digits = 4015,
                        PastTransactions = new List<User.Account.Transaction>()
                    }
                }
            }
        };

        public void InactivityDismissed()
        {
            LoggedIn = false;
            CurrentUser = null;
        }

        public void Logout()
        {
            LoggedIn = false;
            CurrentUser = null;
        }

        public bool DoesUserExist(string username) =>
            Users.FirstOrDefault(x => x.Username == username) != null;

        public void AddTransaction(User.Account account, User.Account.Transaction transaction) =>
            account.PastTransactions.Add(transaction);

        public void Login(string username)
        {
            CurrentUser = username;
            LoggedIn = true;
            LoggedInAt = DateTime.Now;
        }

        public User GetCurrentUser() =>
            Users.FirstOrDefault(x => x.Username == CurrentUser);

        public List<User.Account> GetCurrentAccounts() =>
            Users.FirstOrDefault(x => x.Username == CurrentUser)?.Accounts;
    }
}