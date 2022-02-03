﻿using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;


namespace Shiny.Extensions.Localization.SqlServer
{
    public class SqlServerLocalizationProvider : ILocalizationProvider
    {
        readonly string connectionString;
        readonly string? appFilter;


        public SqlServerLocalizationProvider(string connectionString, string? appFilter)
        {
            this.connectionString = connectionString;
            this.appFilter = appFilter;
        }


        public ILocalizationSource[] Load()
        {
            var list = new List<ILocalizationSource>();
            using (var conn = new SqlConnection(this.connectionString))
            {
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = this.GetSql();
                    conn.Open();

                    using (var reader = command.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        var section = "";
                        List<string>? currentKeys = null;
                        Dictionary<string, string>? currentValues = null;

                        while (reader.Read())
                        {
                            var current = reader.GetString(0);
                            var key = reader.GetString(1);
                            var culture = reader.IsDBNull(2) ? null : reader.GetString(2);
                            var value = reader.GetString(3);

                            if (current != section)
                            {
                                current = section;
                                currentValues = new Dictionary<string, string>();
                                currentKeys = new List<string>();
                                var source = new SqlServerLocalizationSource(current, currentKeys, currentValues);
                                list.Add(source);
                            }
                            if (!currentKeys!.Contains(key))
                                currentKeys.Add(key);

                            currentValues!.Add($"{key}_{culture}", value);
                        }
                    }
                }
            }
            return list.ToArray();
        }


        string? builtSql;
        string GetSql()
        {
            this.builtSql ??= this.appFilter == null
                ? "SELECT Section, ResourceKey, CultureCode, Value FROM Localizations ORDER BY Section, ResourceKey, CultureCode"
                : $"SELECT Section, ResourceKey, CultureCode, Value FROM Localizations WHERE AppIdentifier = '{this.appFilter}' ORDER BY Section, ResourceKey, CultureCode"

            return this.builtSql;
        }
    }
}
