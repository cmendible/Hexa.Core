using System;
using Hexa.Core.Pooling;
using NUnit.Framework;
using Hexa.Core.Tests.Domain;


namespace Hexa.Core.Tests.Extensions
{
    [TestFixture]
    public class ObjectExtensionsTests
    {
        [Test]
        public void MakeTransient()
        {
            var human = new Human();
            var transientHuman = human.MakeTransient();
        }
    }
}
