//----------------------------------------------------------------------------------------------
// <copyright file="IConsumeEvent.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    /// <summary>
    /// Event consumer contract
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConsumeEvent<T>
        where T : class
    {
        /// <summary>
        /// Consumes the specified event.
        /// </summary>
        /// <param name="event">The event.</param>
        void Consume(T @event);
    }
}