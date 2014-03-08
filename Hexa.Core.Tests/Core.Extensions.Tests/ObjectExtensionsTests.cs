//----------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensionsTests.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Extensions.Tests
{
    using System;
    using Domain;
    using NUnit.Framework;
    using Hexa.Core.Tests.Domain;

    [TestFixture]
    public class ObjectExtensionsTests
    {
        [Test]
        public void DeepClone()
        {
            var entityA = new EntityA();
            EntityA clone = entityA.DeepClone();
        }
    }
}