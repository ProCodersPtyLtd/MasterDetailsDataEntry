using MasterDetailsDataEntry.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MasterDetailsDataEntry.Blazor
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMasterDetailsDataEntryBlazor([NotNullAttribute] this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IComponentTypeResolver, ComponentTypeResolver>();
            return serviceCollection;
        }
    }
}
