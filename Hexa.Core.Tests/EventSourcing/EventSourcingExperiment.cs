//----------------------------------------------------------------------------------------------
// <copyright file="Experiment.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------

#if false

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hexa.Core.Domain;
using Raven.Client.Embedded;
using System.Threading.Tasks;
using System.Reflection;
using Hexa.Core.Tests.Domain.Events;
using Microsoft.Practices.Unity;
using MassTransit;
using MassTransit.Services.Routing.Configuration;
using NUnit.Framework;
using Hexa.Core.Logging;

namespace Hexa.Core.Tests.Domain
{
    public class MassTransitPublisher : IEventPublisher
    {
        IServiceBus bus;

        public MassTransitPublisher(IServiceBus bus)
        {
            this.bus = bus;
        }

        public void Publish<T>(T @event) where T : Event
        {
            // DomainEvents.Raise<T>(@event);
            this.bus.Publish<T>(@event);
        }
    }

    public class AnotherEventMessageHandler :
        Consumes<EntityCCreated>.All,
        Consumes<EntityCUpdated>.All,
        Consumes<AddedBToEntityC>.All

        // IHandleMessages<EntityCCreated>, IHandleMessages<EntityCUpdated>, IHandleMessages<AddedBToEntityC>
    {
        public void Consume(EntityCCreated message)
        {
            Console.WriteLine("cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc");
        }

        public void Consume(EntityCUpdated message)
        {
            Console.WriteLine("==========================================================================");
        }

        public void Consume(AddedBToEntityC message)
        {
            Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");
        }
    }

    [TestFixture]
    public class EventSourcingExperiment
    {
        [Test]
        [Ignore("This is just an experiment")]
        public void Try_EventSourcing()
        {
            LoggerManager.GetLogger = (type) => { return new ConsoleLogger(type); };

            UnityContainer unityContainer = new UnityContainer();
            ServiceLocator.Initialize(
                (x, y) => unityContainer.RegisterType(x, y),
                (x, y) => unityContainer.RegisterInstance(x, y),
                (x) => { return unityContainer.Resolve(x); },
                (x) => { return unityContainer.ResolveAll(x); });

            var ravenStore = new EmbeddableDocumentStore
            {
                DataDirectory = "Data"
            };
            ravenStore.Conventions.FindIdentityProperty = prop => prop.Name == "Id";
            ravenStore.Initialize();

            AutoMapper.Mapper.CreateMap<EntityCUpdated, EntityC>()
            .ForMember(d => d.Id, o => o.Ignore());

            ServiceLocator.RegisterType(typeof(EventSourcedRepository<>), typeof(EventSourcedRepository<>));

            DomainEvents.Register<EntityCCreated>(e =>
            {
                EventSourcedRepository<EntityC> entityCRepo = ServiceLocator.GetInstance<EventSourcedRepository<EntityC>>();
                var newC = entityCRepo.GetById(e.Id);

                using (var session = ravenStore.OpenSession())
                {
                    session.Store(newC);
                    session.SaveChanges();
                }

                Console.WriteLine("Entity C Created");
            });

            DomainEvents.Register<AddedBToEntityC>(e =>
            {
                EventSourcedRepository<EntityC> entityCRepo = ServiceLocator.GetInstance<EventSourcedRepository<EntityC>>();
                var newC = entityCRepo.GetById(e.Id);

                using (var session = ravenStore.OpenSession())
                {
                    var old = session.Load<EntityC>(e.Id);
                    if (old != null)
                    {
                        session.Store(old);
                        session.SaveChanges();
                    }
                }

                Console.WriteLine("A new B was related to C... Sending alert to Japan");
            });

            DomainEvents.Register<EntityCUpdated>(e =>
            {
                using (var session = ravenStore.OpenSession())
                {
                    var old = session.Load<EntityC>(e.Id);
                    if (old != null)
                    {
                        AutoMapper.Mapper.Map<EntityCUpdated, EntityC>(e, old);
                        session.Store(old);
                        session.SaveChanges();
                    }
                }

                Console.WriteLine("Hey somebody updated C!");
            });

            DomainEvents.Register<EntityCUpdated>(e =>
            {
                Console.WriteLine("Send email to Javier");
            });

            IServiceBus clientBus = ServiceBusFactory.New(sbc =>
            {
                // sbc.UseMsmq(cfg =>
                // {
                //    cfg.VerifyMsmqConfiguration();
                //    //cfg.UseMulticastSubscriptionClient();
                // });
                sbc.UseRabbitMq();
                sbc.SetCreateTransactionalQueues(true);

                // sbc.UseSubscriptionService("msmq:// localhost/mt_subscriptions");
                // sbc.ReceiveFrom("msmq:// localhost/test_queue");
                sbc.ReceiveFrom("rabbitmq://localhost/client_queue");
                sbc.Subscribe(subs =>
                {
                    subs.Handler<EntityCUpdated>((msg, @event) => Console.WriteLine(@event.Name));
                    subs.Consumer<AnotherEventMessageHandler>();
                });
                sbc.EnableMessageTracing();
                sbc.UseJsonSerializer();
                sbc.SetDefaultRetryLimit(10);

                // sbc.EnableRemoteIntrospection();
            });

            IServiceBus bus = ServiceBusFactory.New(sbc =>
            {
                // sbc.UseMsmq(cfg =>
                // {
                //    cfg.VerifyMsmqConfiguration();
                //    //cfg.UseMulticastSubscriptionClient();
                // });
                sbc.UseRabbitMq();
                sbc.SetCreateTransactionalQueues(true);

                // sbc.UseSubscriptionService("msmq:// localhost/mt_subscriptions");
                // sbc.ReceiveFrom("msmq:// localhost/test_queue");
                sbc.ReceiveFrom("rabbitmq://localhost/test_queue");

                // sbc.Subscribe(subs =>
                // {
                //    subs.Handler<EntityCUpdated>((msg, @event) => Console.WriteLine(@event.Name));
                //    subs.Consumer<AnotherEventMessageHandler>();
                // });
                sbc.EnableMessageTracing();
                sbc.UseJsonSerializer();
                sbc.SetDefaultRetryLimit(10);

                // sbc.EnableRemoteIntrospection();
            });

            // Bus.Instance.Probe();
            // Bus.Instance.WriteIntrospectionToConsole();

            // NServiceBus
            // var bus = NServiceBus.Configure.With(typeof(NServiceBusPublisher).Assembly)
            //    .DefiningMessagesAs(t => t.Namespace != null && t.Namespace.StartsWith("Hexa.Core.Tests.Domain.Events"))
            //    .Log4Net()
            //    .DefaultBuilder()
            //    .JsonSerializer()
            //    .MsmqTransport()
            //    .IsTransactional(true)
            //    .MsmqSubscriptionStorage()
            //    .UnicastBus()
            //    .ImpersonateSender(false)
            //    .LoadMessageHandlers()
            //    .UseRavenTimeoutPersister()
            //    .CreateBus()
            //    .Start(() => Configure.Instance.ForInstallationOn<Windows>().Install());

            IEventStore store = new MemoryEventStore(new MassTransitPublisher(bus));
            ServiceLocator.RegisterInstance<IEventStore>(store);

            // Business
            EntityC entity = new EntityC(Guid.NewGuid(), "CFM");
            for (int i = 0; i <= 10; i++)
            {
                entity.AddB(Guid.NewGuid());
            }

            EventSourcedRepository<EntityC> repo = ServiceLocator.GetInstance<EventSourcedRepository<EntityC>>();
            repo.Save(entity, -1);

            // EntityC entityFromStore = repo.GetById(entity.Id);
            // entityFromStore.Update("cmendibl3");

            // repo.Save(entityFromStore, entityFromStore.Version);

            // entityFromStore = repo.GetById(entity.Id);

            ////FROM DE UI
            // for (int i = 0; i <= 100; i++)
            // {
            //    using (var session = ravenStore.OpenSession())
            //    {
            //        var readModel = session.Query<EntityC>().FirstOrDefault();
            //        if (readModel != null)
            //            Console.WriteLine("This is the name from read model:" + readModel.Name);
            //    }
            // };

            Console.WriteLine("Done!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }
}
#endif