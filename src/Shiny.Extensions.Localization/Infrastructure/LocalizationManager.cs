﻿using System;
using System.Collections.Generic;
using System.Globalization;


namespace Shiny.Extensions.Localization.Infrastructure
{
    public class LocalizationManager : ILocalizationManager
    {
        readonly Dictionary<string, ILocalizationSource> localizeSet;


        public LocalizationManager(Dictionary<string, ILocalizationSource> localizeSet)
            => this.localizeSet = localizeSet;


        public string? this[string key] => this.GetString(key);
        public IReadOnlyCollection<ILocalizationSource> Sections => this.localizeSet.Values;


        public ILocalizationSource? GetSection(string sectionName)
        {
            if (!this.localizeSet.ContainsKey(sectionName))
                return null;

            return this.localizeSet[sectionName];
        }


        public string? GetString(string key, CultureInfo? culture = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var pair = key.Split(':');
            if (pair.Length != 2)
                throw new ArgumentException("Invalid Key");

            return this.GetSection(pair[0])?.GetString(pair[1], culture);
        }
    }
}
