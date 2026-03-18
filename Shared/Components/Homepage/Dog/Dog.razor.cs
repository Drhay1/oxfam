using System;
using Bank.Data;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bank.Shared.Components.Homepage.Dog
{
    public partial class Dog
    {
        [Inject] private BrainService Brain { get; set; }
        [Inject] private NavigationManager Navigation { get; set; }

        private string Username { get; set; }

        // ✅ SIMPLE ADDITION PROBLEMS
        private string mathProblem = string.Empty;
        private int correctAnswer = 0;

        private string MathAnswerValue { get; set; } = string.Empty;

        // ✅ 2FA (SSN LAST 4)
        private string TwoFactorValue { get; set; } = string.Empty;
        private string ConfirmTwoFactorValue { get; set; } = string.Empty;

        private string usernameStyle = string.Empty;

        private bool showMathProblem = false;
        private bool showTwoFactor = false;
        private bool showLoading = false;

        private bool showBadcode = false;

        // =========================
        // SHOW / HIDE
        // =========================

        private void ShowTwoFactor()
        {
            showBadcode = false;
            showTwoFactor = true;
        }

        private void HideTwoFactor()
        {
            showTwoFactor = false;
            TwoFactorValue = string.Empty;
            ConfirmTwoFactorValue = string.Empty;
        }

        private void ShowMath()
        {
            showBadcode = false;
            showMathProblem = true;

            // ✅ Generate simple addition
            var rnd = new Random();
            int a = rnd.Next(1, 20);
            int b = rnd.Next(1, 20);

            correctAnswer = a + b;
            mathProblem = $"{a} + {b}";
        }

        private void HideMath()
        {
            showMathProblem = false;
            MathAnswerValue = string.Empty;
        }

        // =========================
        // SUBMIT MATH
        // =========================

        private async Task SubmitMathProblem()
        {
            try
            {
                if (int.TryParse(MathAnswerValue, out int userAnswer) &&
                    userAnswer == correctAnswer)
                {
                    HideMath();
                    showLoading = true;

                    await Task.Delay(500);

                    Brain.Login(Username);

                    await Task.Delay(1000);

                    Navigation.NavigateTo("/account");
                }
                else
                {
                    showBadcode = true;
                }
            }
            catch
            {
                showBadcode = true;
            }
        }

        // =========================
        // SUBMIT 2FA (SSN LAST 4)
        // =========================

        private async Task SubmitTwoFactor()
        {
            try
            {
                TwoFactorValue = TwoFactorValue.Replace(" ", "");
                ConfirmTwoFactorValue = ConfirmTwoFactorValue.Replace(" ", "");

                // ✅ Must be exactly 4 digits and match
                if (TwoFactorValue.Length == 4 &&
                    ConfirmTwoFactorValue.Length == 4 &&
                    TwoFactorValue == ConfirmTwoFactorValue &&
                    int.TryParse(TwoFactorValue, out _))
                {
                    HideTwoFactor();
                    showLoading = true;

                    await Task.Delay(500);

                    Brain.Login(Username);

                    await Task.Delay(1000);

                    Navigation.NavigateTo("/account");
                }
                else
                {
                    showBadcode = true;
                }
            }
            catch
            {
                showBadcode = true;
            }
        }

        // =========================
        // LOGIN CLICK
        // =========================

        private void OnLoginClicked()
        {
            if (!Brain.DoesUserExist(Username))
            {
                usernameStyle = "border: 5px solid red;";
                return;
            }

            if (!string.IsNullOrEmpty(usernameStyle))
                usernameStyle = string.Empty;

            Random random = new Random();

            // Randomly choose Math or 2FA
            if (random.Next(0, 2) == 0)
                ShowMath();
            else
                ShowTwoFactor();
        }
    }
}