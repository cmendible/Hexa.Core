using System;
using Hexa.Core.Tests.Domain;
using NUnit.Framework;


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
