using System;
using System.Collections.Generic;

namespace Bank.Data
{
    public partial class BrainService
    {
        /// <summary>
        /// The name of the "bank".
        /// </summary>
        public string BankName = "Washington American Mutual";

        /// <summary>
        /// Text show on the footer of the page.
        /// Allows html
        /// </summary>
        public string Footer = @"
            <p>Member FDIC. Deposits insured up to applicable limits.</p>
            <p>
                Washington American Mutual is committed to providing secure, transparent, and customer-focused financial services.
                All products and services are subject to applicable state and federal regulations.
            </p>
            <p>&copy; 2026 Washington American Mutual. All rights reserved.</p>
        ";

        /// <summary>
        /// The text next to the login box. 
        /// </summary>
        public TitleTextPair Dog = new TitleTextPair
        (
            "Welcome to Smarter Banking",
            new List<string>
            {
                "Take control of your financial future with tools designed to help you grow.",
                "Secure, simple, and built around you—banking that fits your life.",
                "Your goals matter. We're here to help you reach them faster.",
                "Experience banking that works as hard as you do.",
                "Trusted by thousands. Built for your future.",
                "Manage your money with confidence, clarity, and control."
            }
        );

        /// <summary>
        /// Mobile banking card on-top of the <see cref="ThreeThings" /> list
        /// </summary>
        public string MobileBanking = "Bank anytime, anywhere with our secure and easy-to-use mobile app.";

        /// <summary>
        /// Below the login area of the main page.
        /// </summary>
        public TitleTextPair[] ThreeThings = new TitleTextPair[3]
        {
            new TitleTextPair(
            "Auto Loans Made Easy",
            "Drive your dream car sooner with flexible financing, competitive rates, and fast approvals."
            ),
            new TitleTextPair
            (
                "Financial Education",
                "Make smarter decisions with expert insights, tools, and resources designed to grow your wealth."
            ),
            new TitleTextPair
            (
                "Digital Banking",
                "Enjoy seamless, secure banking from anywhere with our powerful online and mobile platforms."
            ),
        };

        /// <summary>
        /// Random text that goes below <see cref="ThreeThings" />
        /// Allows HTML
        /// </summary>
        public TitleTextPair RandomText = new TitleTextPair(
            "Banking Built Around You",
            "At Washington American Mutual, we believe banking should be simple, secure, and empowering. " +
            "That’s why we provide modern financial solutions designed to help you save smarter, spend wisely, " +
            "and grow confidently. Whether you're planning for the future or managing your day-to-day finances, " +
            "we’re here to support every step of your journey."
        );

        /// <summary>
        /// Rates below <see cref="RandomText">
        /// </summary>
        public TitleTextPair[] Rates = new TitleTextPair[3]
        {
            new TitleTextPair("Fast Online Transfers", "Low Fees"),
            new TitleTextPair("Credit Card Rates", "Competitive APR"),
            new TitleTextPair("International Payments", "Best-in-Class Rates")
        };

        [Obsolete("Use class properties instead.", true)]
        public string Get(string key) =>
            "This is obsolete";
    }

    /// <summary>
    /// Used in <see cref="BrainService" /> for configuration.
    /// Contains a title and description.
    /// </summary>
    public class TitleTextPair
    {
        private readonly string title = string.Empty;
        private readonly string text = string.Empty;

        private readonly List<string> textOptions;

        public string Title { get { return title; } }
        public string Text
        {
            get
            {
                if (textOptions != null)
                    return textOptions[new Random().Next(textOptions.Count)];
                else
                    return text;
            }
        }

        public TitleTextPair() { }

        public TitleTextPair(string title, string text)
        {
            this.title = title;
            this.text = text;
        }

        public TitleTextPair(string title, List<string> text)
        {
            this.title = title;
            textOptions = text;
        }
    }
}