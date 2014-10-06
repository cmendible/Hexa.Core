//----------------------------------------------------------------------------------------------
// <copyright file="ValidateEventListener.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System.Linq;
    using NHibernate.Cfg;
    using NHibernate.Event;

    using Validation;

    public sealed class ValidateEventListener : IPreInsertEventListener, IPreUpdateEventListener, IInitializable
    {
        private bool isInitialized;

        public void Initialize(Configuration cfg)
        {
            if (!this.isInitialized && (cfg != null))
            {
                this.isInitialized = true;
            }
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            Validate(@event.Entity);
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            Validate(@event.Entity);
            return false;
        }

        private static void Validate(object entity)
        {
            IValidatable validatable = entity as IValidatable;
            if (validatable != null)
            {
                validatable.AssertValidation();
            }
        }

        public static void AppendTo(Configuration configuration)
        {
            configuration.SetListeners(
                ListenerType.PreInsert,
                configuration.EventListeners.PreInsertEventListeners.Concat(new IPreInsertEventListener[] { new ValidateEventListener() }).ToArray());

            configuration.SetListeners(
                ListenerType.PreUpdate,
                configuration.EventListeners.PreUpdateEventListeners.Concat(new IPreUpdateEventListener[] { new ValidateEventListener() }).ToArray());
        }
    }
}