//----------------------------------------------------------------------------------------------
// <copyright file="IEventPublisher.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------

namespace Hexa.Core.Domain
{
    public interface IEventPublisher
    {
        void Publish<T>(T @event)
        where T : Event;
    }
}