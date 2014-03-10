//----------------------------------------------------------------------------------------------
// <copyright file="EmptyEventPublisher.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using Hexa.Core.Logging;

    /// <summary>
    /// Default Event Publisher
    /// </summary>
    public class EmptyEventPublisher : IEventPublisher
    {
        /// <summary>
        /// The logger
        /// </summary>
        private ILogger logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyEventPublisher"/> class.
        /// </summary>
        public EmptyEventPublisher()
        {
            this.logger = LoggerManager.GetLogger(this.GetType());
        }

        /// <summary>
        /// Publishes the specified event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event">The event.</param>
        public void Publish<T>(T @event) where T : class
        {
            logger.DebugFormat("Publishing event of type: {0}", typeof(T).FullName);
        }
    }
}
