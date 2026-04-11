using Bank.Data;
using Microsoft.AspNetCore.Components;

namespace Bank.Pages.Profile
{
    public partial class Profile
    {
        [Inject] private BrainService Brain { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }

        private User CurrentUser => Brain.GetCurrentUser();

        private string FirstName { get; set; }
        private string LastName { get; set; }
        private string Email { get; set; }
        private string PhoneNumber { get; set; }
        private string MailingAddress { get; set; }
        private string City { get; set; }
        private string State { get; set; }
        private string ZipCode { get; set; }
        private bool showSaved;

        protected override void OnInitialized()
        {
            if (CurrentUser == null)
            {
                Navigation.NavigateTo("/");
                return;
            }

            FirstName = CurrentUser.FirstName;
            LastName = CurrentUser.LastName;
            Email = CurrentUser.Email;
            PhoneNumber = CurrentUser.PhoneNumber;
            MailingAddress = CurrentUser.MailingAddress;
            City = CurrentUser.City;
            State = CurrentUser.State;
            ZipCode = CurrentUser.ZipCode;
        }

        private void SaveProfile()
        {
            Brain.UpdateCurrentUserProfile(
                FirstName,
                LastName,
                Email,
                PhoneNumber,
                MailingAddress,
                City,
                State,
                ZipCode);

            showSaved = true;
        }
    }
}
