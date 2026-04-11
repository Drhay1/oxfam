using Bank.Data;
using Microsoft.AspNetCore.Components;

namespace Bank.Pages.Business
{
    public partial class Business
    {
        [Inject] private BrainService Brain { get; set; }

        private string BackLocation =>
            Brain.LoggedIn ? "/account" : "/";

        private string BackLabel =>
            Brain.LoggedIn ? "Account" : "Home";
    }
}
