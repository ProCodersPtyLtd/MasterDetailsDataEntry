using Microsoft.Extensions.DependencyInjection;
using Platz.ObjectBuilder;
using SqlForms.DevSpace.Controlers;
using SqlForms.DevSpace.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SqlForms.DevSpace
{
    public static class DevSpaceServiceCollectionExtensions
    {
        public static IServiceCollection AddDevSpace([NotNullAttribute] this IServiceCollection services)
        {
            services.AddSingleton<ISpaceController, SpaceController>();
            services.AddSingleton<IProjectLoader, FileProjectLoader>();
            services.AddSingleton<IFormBuilderController, FormBuilderController>();

            return services;
        }
    }
}
