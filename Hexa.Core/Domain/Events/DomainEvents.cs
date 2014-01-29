//----------------------------------------------------------------------------------------------
// <copyright file="DomainEvents.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class DomainEvents
    {
        // so that each thread has its own callbacks
        [ThreadStatic]
        private static List<Delegate> actions;

        /// <summary>
        /// The publish method
        /// </summary>
        private static MethodInfo publishMethod = typeof(DomainEvents).GetMethods()
            .Where(m => m.Name == "Raise")
            .Select(m => new
            {
                Method = m,
                Params = m.GetParameters(),
                Args = m.GetGenericArguments()
            })
            .Where(x => x.Params.Length == 1
                        && x.Args.Length == 1
                        && x.Params[0].ParameterType == x.Args[0])
            .Select(x => x.Method)
            .First();

        /// <summary>
        /// Clears the callbacks.
        /// </summary>
        public static void ClearCallbacks()
        {
            DomainEvents.actions = null;
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="@event">The args.</param>
        public static void Raise<T>(T @event)
        where T : class
        {
            foreach (var handler in ServiceLocator.GetAllInstances<IDomainEventHandler<T>>())
            {
                handler.Handle(@event);
            }

            if (DomainEvents.actions != null)
            {
                foreach (Action action in DomainEvents.actions)
                {
                    Action<T> typedAction = action as Action<T>;
                    if (typedAction != null)
                    {
                        typedAction(@event);
                    }
                }
            }
        }

        /// <summary>
        /// Registers a callback for the given domain event.
        /// Used for unit testing.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callback">The callback.</param>
        public static void Register<T>(Action<T> callback)
        where T : class
        {
            if (DomainEvents.actions == null)
            {
                DomainEvents.actions = new List<Delegate>();
            }

            DomainEvents.actions.Add(callback);
        }

        /// <summary>
        /// Publishes the specified @event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event">The @event.</param>
        public static void Publish<T>(T @event)
        where T : class
        {
            DomainEvents.Publish(new { @event });
        }

        /// <summary>
        /// Publishes the specified events.
        /// </summary>
        /// <param name="events">The events.</param>
        public static void Publish(ICollection events)
        {
            foreach (object @event in events)
            {
                MethodInfo method = DomainEvents.publishMethod.MakeGenericMethod(new Type[] { @event.GetType() });
                method.Invoke(null, new object[] { @event });
            }
        }
    }
}