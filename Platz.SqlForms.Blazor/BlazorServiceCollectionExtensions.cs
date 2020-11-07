using Platz.SqlForms.Shared;
using Microsoft.Extensions.DependencyInjection;
using Platz.SqlForms.Blazor;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Platz.SqlForms
{
    public static class BlazorServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatzSqlFormsBlazor([NotNullAttribute] this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IComponentTypeResolver, ComponentTypeResolver>();
            serviceCollection.AddTransient<RepeaterDataComponentController, RepeaterDataComponentController>();
            return serviceCollection;
        }
    }
}
