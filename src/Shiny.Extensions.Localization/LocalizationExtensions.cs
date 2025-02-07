﻿using Microsoft.Extensions.DependencyInjection;
using Shiny.Extensions.Localization.Providers;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;


namespace Shiny.Extensions.Localization
{
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Configures localization with Microsoft.Extensions.DependencyInjection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="builderAction"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureLocalization(this IServiceCollection services, Action<LocalizationBuilder> builderAction)
        {
            var builder = new LocalizationBuilder();
            builderAction.Invoke(builder);
            var manager = builder.Build();

            services.AddSingleton(manager);
            services.AddSingleton(typeof(ILocalize<>), typeof(Localize<>));

            return services;
        }


        ///// <summary>
        ///// Add the SQL Server provider to the localization builder as a localization source
        ///// </summary>
        ///// <param name="builder">Your current builder</param>
        ///// <param name="connectionString">The SQL Server connection string</param>
        ///// <param name="appFilter">This will add an additional filter (WHERE AppIdentifier = 'YOURVALUE') to the query</param>
        ///// <returns></returns>
        //public static LocalizationBuilder AddSqlServer(this LocalizationBuilder builder, string connectionString, string? appFilter = null)
        //    => builder.Add(new AdoNetLocalizationProvider<SqlConnection>(connectionString, appFilter));


        /// <summary>
        /// Add a generic ADO.NET driver
        /// </summary>
        /// <typeparam name="TDbConnection"></typeparam>
        /// <param name="builder">Your current builder</param>
        /// <param name="connectionString">The database connection string</param>
        /// <param name="appFilter">This will add an additional filter (WHERE AppIdentifier = 'YOURVALUE') to the query</param>
        /// <returns></returns>
        public static LocalizationBuilder AddAdoNet<TDbConnection>(this LocalizationBuilder builder, string connectionString, string? appFilter = null) where TDbConnection : DbConnection, new()
            => builder.Add(new AdoNetLocalizationProvider<TDbConnection>(connectionString, appFilter));


        /// <summary>
        /// Gets localization source for a type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static ILocalizationSource GetLocalize<T>(this ILocalizationManager manager)
            => new Localize<T>(manager);


        /// <summary>
        /// This is a useful method if you want to serialize all sections for a specified culture to something like JSON
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> GetAllSectionsWithKeys(this ILocalizationManager manager, CultureInfo culture)
        {
            var dict = new Dictionary<string, IReadOnlyDictionary<string, string>>();
            foreach (var section in manager.Sections)
            {
                var values = section.GetValues(culture);
                dict.Add(section.Name, values);
            }
            return dict;
        }


        /// <summary>
        /// Quick shortcut for getting a sections key/values for a specified culture
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="sectionName"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public static IReadOnlyDictionary<string, string> GetSectionValues(this ILocalizationManager manager, string sectionName, CultureInfo culture)
            => manager.GetSection(sectionName)!.GetValues(culture);


        public static void Bind(this ILocalizationSource localize, object obj)
        {
            var props = obj
                .GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.SetMethod != null && x.GetMethod != null)
                .ToList();

            foreach (var prop in props)
            {
                var value = localize[prop.Name];
                if (value != null)
                    prop.SetValue(obj, value);
            }
        }


        public static string? GetEnum<T>(this ILocalizationManager localize, string section, T value)
            => localize.GetSection(section)?.GetEnum(value);


        public static string? GetEnum<T>(this ILocalizationSource localize, T value)
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(typeof(T).FullName + " is not an enum");

            var name = $"{typeof(T).Namespace}.{typeof(T).Name}.{value}";
            return localize[name];
        }
    }
}
