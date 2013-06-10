#region Header

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion Header

namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    public static class DomainEvents
    {
        #region Fields

        // so that each thread has its own callbacks
        [ThreadStatic]
        private static List<Delegate> actions;

        #endregion Fields

        #region Methods

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
                foreach (var action in actions)
                    if (action is Action<T>)
                    {
                        ((Action<T>)action)(args);
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

        #endregion Methods
    }
}