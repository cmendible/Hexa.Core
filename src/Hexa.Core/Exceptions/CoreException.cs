//----------------------------------------------------------------------------------------------
// <copyright file="CoreException.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core
{
    using System;

    /// <summary>
    /// Core Exception
    /// </summary>
    public abstract class CoreException : Exception
    {
        /// <summary>
        /// Exception unique id used for logging purposes.
        /// </summary>
        private readonly Guid _UniqueId = GuidExtensions.NewCombGuid();

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException"/> class.
        /// </summary>
        protected CoreException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException"/> class.
        /// </summary>
        /// <param name="p_Message">The p_ message.</param>
        protected CoreException(string message)
        : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        protected CoreException(string message, Exception ex)
        : base(message, ex)
        {
        }

        /// <summary>
        /// Gets the unique id.
        /// </summary>
        /// <value>The unique id.</value>
        public Guid UniqueId
        {
            get
            {
                return _UniqueId;
            }
        }
    }
}