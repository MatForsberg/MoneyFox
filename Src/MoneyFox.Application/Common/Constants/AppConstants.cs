﻿using System.Diagnostics.CodeAnalysis;

namespace MoneyFox.Application.Common.Constants
{
    /// <summary>
    /// String Constants for usage in the app
    /// </summary>
    [SuppressMessage("Major Code Smell", "S3996:URI properties should not be strings", Justification = "We use the string to display as well.")]
    public static class AppConstants
    {
        /// <summary>
        /// URL to the Apply Solutions website
        /// </summary>
        public static string WebsiteUrl => "http://www.apply-solutions.ch";

        /// <summary>
        /// Mail address for support
        /// </summary>
        public static string SupportMail => "mobile.support@apply-solutions.ch";


        /// <summary>
        /// URL to the GitHub repository
        /// </summary>
        public static string GitHubRepositoryUrl => "https://github.com/MoneyFox/MoneyFox";

        /// <summary>
        /// URL to the GitHub repository
        /// </summary>
        public static string TranslationProjectUrl => "https://crowdin.com/project/money-fox";

        /// <summary>
        /// URL to the Twitter AccountViewModel of the icon.
        /// </summary>
        public static string IconDesignerTwitterUrl => "https://twitter.com/vandert9";

        /// <summary>
        /// URL to the Twitter AccountViewModel of the icon.
        /// </summary>
        public static string GithubContributionUrl => "https://github.com/MoneyFox/MoneyFox/graphs/contributors";

        /// <summary>
        /// ID string for add income shortcuts
        /// </summary>
        public static string AddIncomeTileId => "AddIncomeTile";

        /// <summary>
        /// ID string for add transfer shortcuts
        /// </summary>
        public static string AddExpenseTileId => "AddSpendingTile";

        /// <summary>
        /// ID string for add transfer shortcuts
        /// </summary>
        public static string AddTransferTileId => "AddTransferTile";

        public static string LogFileName => "moneyfox.log";

        public static string MSAL_APPLICATION_ID => "00a3e4cd-b4b0-4730-be62-5fcf90a94a1d";

    }
}
