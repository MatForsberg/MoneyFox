﻿using MoneyFox.Application.Common.Interfaces;
using System;
using Windows.ApplicationModel.Store;
using Windows.System;

namespace MoneyFox.Uwp.Src
{
    public class MarketplaceOperations : IStoreOperations
    {
        private const string STORE_URL = "ms-windows-store:reviewapp?appid=";

        public async void RateApp() => await Launcher.LaunchUriAsync(new Uri($"{STORE_URL}{CurrentApp.AppId}"));
    }
}
