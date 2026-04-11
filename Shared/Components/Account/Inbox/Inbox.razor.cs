using System;
using Bank.Data;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace Bank.Shared.Components.Account.Inbox
{
    public partial class Inbox
    {
        [Inject] private BrainService Brain { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }

        private int NumUnread =>
            Brain.GetCurrentInboxMessages().Count(x => x.IsUnread);

        private void ViewInbox()
        {
            Navigation.NavigateTo("/inbox");
        }
    }
}
