using System;
using System.Linq;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Bank.Pages.Transfer
{
    using Data;

    public partial class Transfer
    {
        [Inject] private NavigationManager Navigation { get; set; }
        [Inject] private BrainService Brain { get; set; }

        private float Amount { get; set; }
        private string TransferFrom { get; set; }
        private string TransferTo { get; set; }

        private bool showModal = false;
        private bool showLoadingModal = false;
        private bool showRestrictionModal = false;

        private User.ExternalAccount NewExternalAccount = new User.ExternalAccount();
        private User.ExternalAccount SelectedExternalAccount;

        private List<User.Account> AllAccounts =>
            Brain.GetCurrentAccounts()
            .Concat(GetExternalAccountsAsAccounts())
            .ToList();

        private List<User.ExternalAccount> ExternalAccounts =>
            Brain.GetExternalAccounts(Brain.CurrentUser);

        private List<User.Account> GetExternalAccountsAsAccounts() =>
            ExternalAccounts.Select(x => new User.Account
            {
                Name = x.Name,
                Last4Digits = int.Parse(x.Last4Digits)
            }).ToList();

        private void ShowExternalAccountForm()
        {
            showModal = true;
        }

        private void CloseModal()
        {
            showModal = false;
            NewExternalAccount = new User.ExternalAccount();
        }

        private void AddExternalAccount()
        {
            if (!string.IsNullOrWhiteSpace(NewExternalAccount.AccountHolder) &&
                !string.IsNullOrWhiteSpace(NewExternalAccount.AccountNumber) &&
                !string.IsNullOrWhiteSpace(NewExternalAccount.AccountType))
            {
                Brain.AddExternalAccount(Brain.CurrentUser, NewExternalAccount);

                TransferTo = NewExternalAccount.Last4Digits;
                SelectedExternalAccount = NewExternalAccount;

                showModal = false;
                NewExternalAccount = new User.ExternalAccount();
            }
        }

        private async Task OnSubmit()
        {
            if (TransferFrom == null)
                TransferFrom = Brain.GetCurrentAccounts().First().Last4Digits.ToString();

            if (TransferTo == null)
                TransferTo = Brain.GetCurrentAccounts().First().Last4Digits.ToString();

            var external = ExternalAccounts.FirstOrDefault(x => x.Last4Digits == TransferTo);

            // Show loading
            showLoadingModal = true;
            StateHasChanged();
            await Task.Delay(4000);
            showLoadingModal = false;

            // External restriction modal
            if (external != null)
            {
                SelectedExternalAccount = external;
                showRestrictionModal = true;
                return;
            }

            // Normal transfer
            var fromAccount = Brain.GetCurrentAccounts()
                .FirstOrDefault(x => x.Last4Digits.ToString() == TransferFrom);
            var toAccount = Brain.GetCurrentAccounts()
                .FirstOrDefault(x => x.Last4Digits.ToString() == TransferTo);

            if (fromAccount == null || toAccount == null) return;
            if (fromAccount == toAccount) return;

            Brain.AddTransaction(fromAccount,
                new User.Account.Transaction($"Transfer to ****{TransferTo}", "Balance Transfer", -Amount, 0));
            Brain.AddTransaction(toAccount,
                new User.Account.Transaction($"Transfer from ****{TransferFrom}", "Balance Transfer", Amount, 0));

            await Task.Delay(500);

            Navigation.NavigateTo($"/transfer/success/{Guid.NewGuid()}");
        }

        private void CloseRestrictionModal()
        {
            showRestrictionModal = false;
        }
    }
}