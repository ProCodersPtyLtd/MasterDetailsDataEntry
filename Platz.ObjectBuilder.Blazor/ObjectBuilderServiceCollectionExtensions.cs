using Microsoft.Extensions.DependencyInjection;
using Platz.ObjectBuilder.Blazor;
using Platz.ObjectBuilder.Blazor.Controllers;
using Platz.ObjectBuilder.Blazor.Controllers.Logic;
using Platz.ObjectBuilder.Blazor.Controllers.Schema;
using Platz.ObjectBuilder.Blazor.Controllers.Validation;
using Platz.ObjectBuilder.Blazor.Validation;
using Platz.ObjectBuilder.Engine;
using Platz.ObjectBuilder.Expressions;
using Platz.SqlForms;
using Plk.Blazor.DragDrop;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Platz.ObjectBuilder
{
    public static class ObjectBuilderServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatzObjectBuilder([NotNullAttribute] this IServiceCollection services)
        {
            services.AddScoped(typeof(DragDropService<>));
            services.AddTransient<IObjectResolver, SqlJsonObjectResolver>();
            services.AddTransient<IQueryBuilderRuleFactory, QueryBuilderRuleFactory>();
            services.AddTransient<IBuilderRuleFactory<ISchemaBuilderRule, DesignSchema>, BuilderRuleFactory<ISchemaBuilderRule, DesignSchema>>();
            services.AddTransient<ISqlExpressionEngine, SqlExpressionEngine>();
            services.AddTransient<IQueryBuilderEngine, QueryBuilderEngine>();
            services.AddTransient<IQueryController, QueryController>();
            services.AddTransient<ISchemaController, SchemaController>();
            services.AddTransient<SchemaTableDesignController, SchemaTableDesignController>();
            services.AddSingleton<IStoreSchemaStorage, FileStoreSchemaStorage>();
            services.AddSingleton<IDataMigrationManager, DataMigrationManager>();
            services.AddSingleton<IStoreDatabaseDriver, SqlJsonStoreDatabaseDriver>();
            services.AddSingleton<IMigrationAggregator, MigrationAggregator>();
            return services;
        }
    }
}
