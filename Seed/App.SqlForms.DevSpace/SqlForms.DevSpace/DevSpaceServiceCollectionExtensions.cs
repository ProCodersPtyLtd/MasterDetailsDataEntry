using Microsoft.Extensions.DependencyInjection;
using Platz.ObjectBuilder;
using Platz.ObjectBuilder.Engine;
using SqlForms.DevSpace.Controlers;
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
            services.AddScoped<IDbContextRegistry, DbContextRegistry>();
            services.AddScoped<ISpaceController, SpaceController>();
            services.AddScoped<IProjectLoader, FileProjectLoader>();
            services.AddScoped<IFormBuilderController, FormBuilderController>();
            services.AddScoped<IQueryController, QueryController>();

            return services;
        }
    }
}
