//----------------------------------------------------------------------------------------------
// <copyright file="DomainEvents.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public static class DomainEvents
    {
        // so that each thread has its own callbacks
        [ThreadStatic]
        private static List<Delegate> actions;

        // Clears callbacks passed to Register on the current thread
        public static void ClearCallbacks()
        {
            actions = null;
        }

        // Raises the given domain event
        public static void Raise<T>(T args)
        where T : IDomainEvent
        {
            foreach (var handler in ServiceLocator.GetAllInstances<IDomainEventHandler<T>>())
            {
                handler.Handle(args);
            }

            if (actions != null)
            {
                foreach (var action in actions)
                {
                    if (action.GetType().FullName.Contains(args.GetType().FullName))
                    {
                        action.DynamicInvoke(args);
                    }
                }
            }
        }

        // Registers a callback for the given domain event
        public static void Register<T>(Action<T> callback)
        where T : IDomainEvent
        {
            if (actions == null)
            {
                actions = new List<Delegate>();
            }

            actions.Add(callback);
        }
    }
}