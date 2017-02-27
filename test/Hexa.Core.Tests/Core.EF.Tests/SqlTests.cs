//----------------------------------------------------------------------------------------------
// <copyright file="SqlTest.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Orm.Tests.EF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Claims;
    using Core.Domain;
    using Hexa.Core.Tests.Data;
    using Hexa.Core.Tests.Domain;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    public class SqlTest : IClassFixture<SqlTest>
    {
        IServiceProvider serviceProvider;

        public SqlTest()
        {
            var services = new ServiceCollection();

            services.AddSingleton(new Mock<ILogger<BaseRepository<EntityA, Guid>>>().Object);
            services.AddSingleton(new Mock<ILogger<BaseRepository<EntityB, Guid>>>().Object);
            services.AddTransient<IEntityARepository, EntityAEFRepository>();
            services.AddTransient<IEntityBRepository, EntityBEFRepository>();
            services.AddScoped<DomainContext>(((s) =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<DomainContext>();
                    optionsBuilder.UseInMemoryDatabase();

                    var context = new DomainContext(optionsBuilder.Options, new Assembly[] { typeof(DomainContext).GetTypeInfo().Assembly });
                    context.Database.EnsureCreated();
                    return context;
                }));

            serviceProvider = services.BuildServiceProvider();

            IoC.Initialize(
                (x) => { return serviceProvider.GetService(x); },
                (x) => { return serviceProvider.GetServices(x); });

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "cmendible")
            }));

            // ClaimsPrincipal.Current = null;
        }

        [Fact]
        public void Add_EntityA()
        {
            EntityA entityA = this.AddEntityA();

            Assert.NotNull(entityA);
            Assert.NotNull(entityA.Version);
            Assert.False(entityA.Id == Guid.Empty);
            Assert.Equal("Martin", entityA.Name);
        }

        /**
        * These testcase will show, that an assertion failure "...collection xyz
        * was not processed by flush" will be thrown, if you use following
        * entity-constellation and an PostUpdateListener:
        *
        * You have entities that:
        *
        * <pre>
        * 1.) two entities are having a m:n relation AND
        * 2.) we have defined an PostUpdateListener that iterates through all properties of the entity and
        *     so also through the m:n relation (=Collection)
        * </pre>
        *
        */
        [Fact]
        public void Collection_Was_Not_Processed_By_Flush()
        {
            EntityA a = this.Create_EntityA_EntityB_And_Many_To_Many_Relation();

            // now update a simple property of EntityA, due to this the
            // MyPostUpdateListener will be called, which iterates through all
            // properties of EntityA (and also the Collection of the m:n relation)
            //--> org.hibernate.AssertionFailure: collection
            // was not processed by flush()
            var repo = this.serviceProvider.GetService<IEntityARepository>();
            a = repo.GetFiltered(u => u.Id == a.Id).Single();
            a.Name = "AA";
            repo.Modify(a);

            var context = this.serviceProvider.GetService<DomainContext>();
            context.SaveChanges();
        }

        [Fact]
        public void Delete_EntityA()
        {
            EntityA entityA = this.AddEntityA();

            IEntityARepository repo = this.serviceProvider.GetService<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFiltered(u => u.Id == entityA.Id);
            Assert.True(results.Count() > 0);

            EntityA entityA2Delete = results.First();

            repo.Remove(entityA2Delete);

            var context = this.serviceProvider.GetService<DomainContext>();
            context.SaveChanges();
        }

        [Fact]
        public void Query_EntityA()
        {
            EntityA entityA = this.AddEntityA();

            var repo = this.serviceProvider.GetService<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFiltered(u => u.Id == entityA.Id);
            Assert.True(results.Count() > 0);
        }

        [Fact]
        public void Update_EntityA()
        {
            EntityA entityA = this.AddEntityA();

            var repo = this.serviceProvider.GetService<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFiltered(u => u.Id == entityA.Id);
            Assert.True(results.Count() > 0);

            EntityA entityA2Update = results.First();
            entityA2Update.Name = "Maria";
            repo.Modify(entityA2Update);

            var context = this.serviceProvider.GetService<DomainContext>();
            context.SaveChanges();

            repo = this.serviceProvider.GetService<IEntityARepository>();
            entityA = repo.GetFiltered(u => u.Id == entityA.Id).Single();

            Assert.Equal("Maria", entityA.Name);
            Assert.True(entityA.UpdatedAt > entityA.CreatedAt);
        }

        private EntityA AddEntityA()
        {
            var entityA = new EntityA();
            entityA.Name = "Martin";

            var repo = this.serviceProvider.GetService<IEntityARepository>();
            repo.Add(entityA);

            var context = this.serviceProvider.GetService<DomainContext>();
            context.SaveChanges();

            return entityA;
        }

        private EntityA Create_EntityA_EntityB_And_Many_To_Many_Relation()
        {
            var a = new EntityA();
            a.Name = "A";

            var b = new EntityB();
            b.Name = "B";

            a.AddB(b);

            var repoA = this.serviceProvider.GetService<IEntityARepository>();
            var repoB = this.serviceProvider.GetService<IEntityBRepository>();

            repoB.Add(b);
            repoA.Add(a);

            var context = this.serviceProvider.GetService<DomainContext>();
            context.SaveChanges();

            return a;
        }
    }
}