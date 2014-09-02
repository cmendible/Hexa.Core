//----------------------------------------------------------------------------------------------
// <copyright file="DomainEvents.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    /// Domain Event Publisher
    /// </summary>
    public static class DomainEvents
    {
        /// <summary>
        /// The domain events key
        /// </summary>
        private const string DomainEventsKey = "Hexa.Core.DomainEvents.Events";

        /// <summary>
        /// The callback actions
        /// </summary>
        [ThreadStatic]
        private static List<Delegate> actions;

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>
        /// The events.
        /// </value>
        private static ConcurrentQueue<Action> Events
        {
            get
            {
                if (ContextState.Get<ConcurrentQueue<Action>>(DomainEvents.DomainEventsKey) == null)
                {
                    ContextState.Store(DomainEvents.DomainEventsKey, new ConcurrentQueue<Action>());
                }

                return ContextState.Get<ConcurrentQueue<Action>>(DomainEvents.DomainEventsKey);
            }
        }

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
            DomainEvents.Events.Enqueue(() =>
                {
                    foreach (var consumer in IoC.GetAllInstances<IConsumeEvent<T>>())
                    {
                        consumer.Consume(@event);
                    }

                    if (DomainEvents.actions != null)
                    {
                        foreach (Delegate action in DomainEvents.actions)
                        {
                            Action<T> typedAction = action as Action<T>;
                            if (typedAction != null)
                            {
                                typedAction(@event);
                            }
                        }
                    }
                });
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
        /// Raises the queued events.
        /// </summary>
        public static void Raise()
        {
            Action dispatch;
            while (DomainEvents.Events.TryDequeue(out dispatch))
            {
                dispatch();
            }
        }
    }
}