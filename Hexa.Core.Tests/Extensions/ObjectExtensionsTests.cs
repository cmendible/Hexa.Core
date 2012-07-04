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
        public void MakeTransient()
        {
            var human = new Human();
            Human transientHuman = human.MakeTransient();
        }

        #endregion Methods
    }
}
