using Bank.Data;
using Microsoft.AspNetCore.Components;

namespace Bank.Pages.Investing
{
    public partial class Investing
    {
        [Inject] private BrainService Brain { get; set; }

        private string BackLocation =>
            Brain.LoggedIn ? "/account" : "/";

        private string BackLabel =>
            Brain.LoggedIn ? "Account" : "Home";
    }
}
