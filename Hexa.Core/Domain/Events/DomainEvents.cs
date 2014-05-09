//----------------------------------------------------------------------------------------------
// <copyright file="DomainEvents.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Domain Event Publisher 
    /// </summary>
    public static class DomainEvents
    {
        /// <summary>
        /// The callback actions
        /// </summary>
        [ThreadStatic]
        private static List<Delegate> actions;

        [ThreadStatic]
        private static ConcurrentQueue<Action> events;

        /// <summary>
        /// The event publisher
        /// </summary>
        private static Func<IEventPublisher> eventPublisher = () => new ConsumeEventPublisher();

        /// <summary>
        /// The publish method
        /// </summary>
        private static MethodInfo publishMethod = typeof(IEventPublisher).GetMethod("Publish");

        /// <summary>
        /// Clears the callbacks.
        /// Used for unit testing.
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
        public static void Dispatch<T>(T @event)
        where T : class
        {
            if (DomainEvents.events == null)
            {
                DomainEvents.events = new ConcurrentQueue<Action>();
            }

            DomainEvents.events.Enqueue(() => eventPublisher().Publish<T>(@event));

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

        public static void Raise()
        {
            Action dispatch;
            while (DomainEvents.events.TryDequeue(out dispatch))
            {
                dispatch();
            }
        }
    }
}