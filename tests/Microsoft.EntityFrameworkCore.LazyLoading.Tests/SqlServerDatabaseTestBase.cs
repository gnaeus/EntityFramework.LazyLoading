﻿using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.LazyLoading.Internal;
using Microsoft.EntityFrameworkCore.LazyLoading.Metadata.Internal;
using Microsoft.EntityFrameworkCore.LazyLoading.Query.Internal;
using Microsoft.EntityFrameworkCore.LazyLoading.Tests.Configuration;
using Microsoft.EntityFrameworkCore.LazyLoading.Tests.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Microsoft.EntityFrameworkCore.LazyLoading.Tests
{
    public abstract class SqlServerDatabaseTestBase
    {
        protected SchoolContext CreateDbContext()
        {
            var config = Config.GetInstance();

            var dbContextOptionsBuilder = new DbContextOptionsBuilder<SchoolContext>();

            dbContextOptionsBuilder
                .UseSqlServer(config.SqlServerDatabaseConfig.ConnectionString);

            // LazyLoading specific
            dbContextOptionsBuilder.ReplaceService<IEntityMaterializerSource, LazyLoadingEntityMaterializerSource<SchoolContext>>();
            dbContextOptionsBuilder.ReplaceService<EntityFrameworkCore.Internal.IConcurrencyDetector, ConcurrencyDetector>();
            dbContextOptionsBuilder.ReplaceService<ICompiledQueryCache, PerDbContextCompiledQueryCache>();

            var ctx = new SchoolContext(dbContextOptionsBuilder.Options);

            // LazyLoading specific
            // ReSharper disable PossibleNullReferenceException
            (ctx.GetService<IEntityMaterializerSource>() as LazyLoadingEntityMaterializerSource<SchoolContext>).SetDbContext(ctx);
            (ctx.GetService<ICompiledQueryCache>() as PerDbContextCompiledQueryCache).SetDbContext(ctx);
            // ReSharper restore PossibleNullReferenceException

            ctx.Database.EnsureCreated();
            ctx.Database.Migrate();

            return ctx;
        }
    }
}