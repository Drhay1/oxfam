using System.Collections.Generic;
using System.Linq;
using Bank.Data;
using Microsoft.AspNetCore.Components;

namespace Bank.Pages.Inbox
{
    public partial class Inbox
    {
        [Inject] private BrainService Brain { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }

        private User CurrentUser => Brain.GetCurrentUser();

        private List<User.InboxMessage> Messages =>
            Brain.GetCurrentInboxMessages();

        private User.InboxMessage SelectedMessage =>
            Messages.FirstOrDefault(x => x.Id == SelectedMessageId) ?? Messages.FirstOrDefault();

        private string SelectedMessageId { get; set; }

        private int UnreadCount =>
            Messages.Count(x => x.IsUnread);

        private void SelectMessage(User.InboxMessage message)
        {
            SelectedMessageId = message.Id;
            Brain.MarkInboxMessageRead(message.Id);
        }

        private string Preview(User.InboxMessage message)
        {
            const int maxLength = 115;

            if (message.Body.Length <= maxLength)
                return message.Body;

            return $"{message.Body.Substring(0, maxLength)}...";
        }

        protected override void OnInitialized()
        {
            if (CurrentUser == null)
                Navigation.NavigateTo("/");
        }
    }
}
