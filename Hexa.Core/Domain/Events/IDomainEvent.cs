//----------------------------------------------------------------------------------------------
// <copyright file="IDomainEvent.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    /// <summary>
    ///
    /// </summary>
    public interface IDomainEvent
    {
    }

    [Serializable]
    public class Event : Message, IDomainEvent
    {
    }

    [Serializable]
    public class Message
    {
        public int Version
        {
            get;
            set;
        }
    }
}