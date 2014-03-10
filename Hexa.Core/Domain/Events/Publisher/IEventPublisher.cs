//----------------------------------------------------------------------------------------------
// <copyright file="IEventPublisher.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    /// <summary>
    /// Event Publisher contract
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes the specified event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event">The event.</param>
        void Publish<T>(T @event) where T : class;
    }
}