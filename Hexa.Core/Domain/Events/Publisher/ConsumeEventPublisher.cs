//----------------------------------------------------------------------------------------------
// <copyright file="ConsumeEventPublisher.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    /// <summary>
    /// Consume Event Publisher
    /// </summary>
    public class ConsumeEventPublisher : IEventPublisher
    {
        /// <summary>
        /// Publishes the specified event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event">The event.</param>
        public void Publish<T>(T @event) where T : class
        {
            foreach (var consumer in ServiceLocator.GetAllInstances<IConsumeEvent<T>>())
            {
                consumer.Consume(@event);
            }
        }
    }
}
