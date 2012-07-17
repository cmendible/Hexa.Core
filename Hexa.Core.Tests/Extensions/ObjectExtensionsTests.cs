namespace Hexa.Core.Tests.Extensions
{
    using System;

    using Domain;

    using NUnit.Framework;

    [TestFixture]
    public class ObjectExtensionsTests
    {
        #region Methods

        [Test]
        public void DeepClone()
        {
            var human = new Human();
            Human clone = human.DeepClone();
        }

        #endregion Methods
    }
}
