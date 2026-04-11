using Bank.Data;
using Microsoft.AspNetCore.Components;

namespace Bank.Pages.Support
{
    public partial class Support
    {
        [Inject] private BrainService Brain { get; set; }

        private string BackLocation =>
            Brain.LoggedIn ? "/account" : "/";

        private string BackLabel =>
            Brain.LoggedIn ? "Account" : "Home";

        private string WhatsAppLink =>
            $"https://wa.me/{DigitsOnly(Brain.SupportPhoneNumber)}";

        private string EmailLink =>
            $"mailto:{Brain.SupportEmail}";

        private string DigitsOnly(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var digits = string.Empty;
            foreach (var character in value)
            {
                if (char.IsDigit(character))
                    digits += character;
            }

            return digits;
        }
    }
}
