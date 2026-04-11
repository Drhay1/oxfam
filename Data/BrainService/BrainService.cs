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
        public bool AccountFrozen { get; set; } = true;
        public bool ShowSuspiciousActivityPopup { get; set; } = false;
        public string SuspiciousActivitySubject { get; set; } = "Suspicious transfer activity noticed";
        public string SuspiciousActivityMessage { get; set; } =
            "Suspicious activity was noticed on your account. A $190,000 transfer from your Premium Checking account to an external account that is not in the account owner's name was 90% complete. To protect your funds, the transaction was placed on hold and the account was frozen for review.";

        // Persistent storage for external accounts per user
        private Dictionary<string, List<User.ExternalAccount>> UserExternalAccounts
            = new Dictionary<string, List<User.ExternalAccount>>();

        private Dictionary<string, HashSet<string>> UserReadInboxMessages
            = new Dictionary<string, HashSet<string>>();

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

            account.AddedAt = DateTime.Now;
            UserExternalAccounts[username].Add(account);
        }

        private List<User> Users = new List<User>
        {
            new User {
                FirstName = "Linda",
                LastName = "CROSTIC",
                Email = "linda.crostic@example.com",
                PhoneNumber = "(555) 014-4278",
                MailingAddress = "1200 Evergreen Terrace",
                City = "Seattle",
                State = "WA",
                ZipCode = "98101",
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
                            new User.Account.Transaction("Oxfam Organization", "Deposit", 200000f),
                            new User.Account.Transaction("External transfer to non-owner account", "Hold", -190000f, 0)
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
            ShowSuspiciousActivityPopup = false;
        }

        public void Logout()
        {
            LoggedIn = false;
            CurrentUser = null;
            ShowSuspiciousActivityPopup = false;
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
            ShowSuspiciousActivityPopup = true;
        }

        public void DismissSuspiciousActivityPopup()
        {
            ShowSuspiciousActivityPopup = false;
        }

        public User GetCurrentUser() =>
            Users.FirstOrDefault(x => x.Username == CurrentUser);

        public List<User.Account> GetCurrentAccounts() =>
            Users.FirstOrDefault(x => x.Username == CurrentUser)?.Accounts;

        public void UpdateCurrentUserProfile(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string mailingAddress,
            string city,
            string state,
            string zipCode)
        {
            var user = GetCurrentUser();
            if (user == null)
                return;

            var previousUsername = user.Username;

            user.FirstName = firstName?.Trim();
            user.LastName = lastName?.Trim();
            user.Email = email?.Trim();
            user.PhoneNumber = phoneNumber?.Trim();
            user.MailingAddress = mailingAddress?.Trim();
            user.City = city?.Trim();
            user.State = state?.Trim();
            user.ZipCode = zipCode?.Trim();

            var newUsername = user.Username;
            if (previousUsername == newUsername)
                return;

            CurrentUser = newUsername;
            MoveUserKey(UserExternalAccounts, previousUsername, newUsername);
            MoveUserKey(UserReadInboxMessages, previousUsername, newUsername);
        }

        public List<User.InboxMessage> GetCurrentInboxMessages()
        {
            var user = GetCurrentUser();
            if (user == null)
                return new List<User.InboxMessage>();

            var messages = new List<User.InboxMessage>
            {
                new User.InboxMessage(
                    SuspiciousActivitySubject,
                    SuspiciousActivityMessage,
                    "Security",
                    DateTime.Now),
                new User.InboxMessage(
                    "Monthly statement is ready",
                    $"Hello {user.FirstName}, your latest monthly statement is now available. Please review your posted transactions, current balances, and any recent account notices when you have a moment.",
                    "Statement",
                    DateTime.Now.AddDays(-1)),
                new User.InboxMessage(
                    "Security reminder",
                    "Washington American Mutual will never ask for your password in an email or text message. Keep your login details private and review account activity regularly for anything unfamiliar.",
                    "Security",
                    DateTime.Now.AddDays(-2)),
                new User.InboxMessage(
                    "Transfer service update",
                    "Scheduled and one-time transfers are available from your online banking dashboard. Internal transfers post immediately in your account view, while external transfer activity may require extra review.",
                    "Account Activity",
                    DateTime.Now.AddDays(-4)),
                new User.InboxMessage(
                    "New mobile banking features",
                    "Mobile banking now supports faster account review, simplified transfer entry, and quick access to recent activity. Sign in from a trusted device to explore the updated experience.",
                    "Bank Notice",
                    DateTime.Now.AddDays(-7))
            };

            if (LoggedInAt != default)
            {
                messages.Add(new User.InboxMessage(
                    "Online banking sign-in",
                    $"A sign-in was recorded for {user.FirstName} {user.LastName}. If this was you, no action is needed. If you do not recognize this activity, contact support and review your recent transactions.",
                    "Security",
                    LoggedInAt));
            }

            foreach (var account in user.Accounts)
            {
                foreach (var transaction in account.PastTransactions)
                {
                    var action = transaction.Amount >= 0 ? "posted to" : "posted from";

                    messages.Add(new User.InboxMessage(
                        $"{transaction.Type} activity",
                        $"{transaction.Description} for {transaction.FormattedAmount} {action} {account.Name} (****{account.Last4Digits}). This notice was created from your recent transaction activity.",
                        "Transaction",
                        transaction.Date));
                }
            }

            foreach (var account in GetExternalAccounts(user.Username))
            {
                messages.Add(new User.InboxMessage(
                    "External account added",
                    $"{account.AccountType} account ending in {account.Last4Digits} at {account.BankName} was added for transfers. Review the account details before sending funds to a newly added destination.",
                    "Account Activity",
                    account.AddedAt));
            }

            ApplyReadStatus(user.Username, messages);

            return messages
                .OrderByDescending(x => x.Date)
                .ToList();
        }

        public void MarkInboxMessageRead(string messageId)
        {
            var user = GetCurrentUser();
            if (user == null || string.IsNullOrEmpty(messageId))
                return;

            if (!UserReadInboxMessages.ContainsKey(user.Username))
                UserReadInboxMessages[user.Username] = new HashSet<string>();

            UserReadInboxMessages[user.Username].Add(messageId);
        }

        private void ApplyReadStatus(string username, List<User.InboxMessage> messages)
        {
            if (!UserReadInboxMessages.ContainsKey(username))
                return;

            var readMessages = UserReadInboxMessages[username];

            foreach (var message in messages)
                message.IsUnread = !readMessages.Contains(message.Id);
        }

        private void MoveUserKey<T>(Dictionary<string, T> dictionary, string oldKey, string newKey)
        {
            if (!dictionary.ContainsKey(oldKey) || dictionary.ContainsKey(newKey))
                return;

            dictionary[newKey] = dictionary[oldKey];
            dictionary.Remove(oldKey);
        }
    }
}
