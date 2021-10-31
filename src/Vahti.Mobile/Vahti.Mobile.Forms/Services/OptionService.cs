﻿using Xamarin.Essentials;

namespace Vahti.Mobile.Forms.Services
{
    /// <summary>
    /// Provides application option information
    /// </summary>
    public class OptionService : IOptionService
    {
        private const string ShowMinMaxValuesPreferenceName = "ShowMinMaxValues";

        public bool ShowMinMaxValues
        {
            get
            {
                return Preferences.Get(ShowMinMaxValuesPreferenceName, false);
            }
            set
            {
                Preferences.Set(ShowMinMaxValuesPreferenceName, value);
            }
        }
    }
}