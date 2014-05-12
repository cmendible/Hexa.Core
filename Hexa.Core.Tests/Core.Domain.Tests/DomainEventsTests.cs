namespace Hexa.Core.Domain.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class DomainEventsTests
    {
        [Test]
        public void DomainEvents_Raise()
        {
            bool eventFired = false;
            DomainEvents.Register<object>((o) => { eventFired = true; });

            DomainEvents.Dispatch(new object());
            DomainEvents.Raise();

            Assert.AreEqual(true, eventFired);
        }
    }
}
