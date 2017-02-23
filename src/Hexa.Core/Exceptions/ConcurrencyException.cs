//----------------------------------------------------------------------------------------------
// <copyright file="ConcurrencyException.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core
{
    using System;

    /// <summary>
    /// Concurrency Exception.
    /// </summary>
    public class ConcurrencyException : CoreException
    {
        /// <summary>
        /// Exception unique id used for logging purposes.
        /// </summary>
        private readonly Guid _UniqueId = GuidExtensions.NewCombGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        public ConcurrencyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConcurrencyException(string message)
        : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public ConcurrencyException(string message, Exception ex)
        : base(message, ex)
        {
        }
    }
}