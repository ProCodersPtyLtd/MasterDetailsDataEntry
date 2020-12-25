using Platz.SqlForms.Shared;
using Microsoft.Extensions.DependencyInjection;
using Platz.SqlForms.Blazor;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Platz.SqlForms
{
    public static class SqlFormsServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatzSqlForms([NotNullAttribute] this IServiceCollection services)
        {
            services.AddPlatzSqlFormsBlazor();
            services.AddSingleton<IDataEntryProvider, DataEntryProvider>();
            services.AddSingleton<IDataValidationProvider, DataValidationProvider>();
            services.AddSingleton<IDynamicEditFormDataProvider, DynamicEditFormDataProvider>();
            return services;
        }
    }
}
