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
            services.AddScoped<ISpaceController, SpaceController>();
            services.AddScoped<IProjectLoader, FileProjectLoader>();
            services.AddScoped<IFormBuilderController, FormBuilderController>();

            return services;
        }
    }
}
