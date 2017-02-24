namespace Hexa.Core.Domain.Tests
{
    using Xunit;

    public class DomainEventsTests
    {
        [Fact]
        public void DomainEvents_Raise()
        {
            bool eventFired = false;
            DomainEvents.Register<object>((o) => { eventFired = true; });

            DomainEvents.Dispatch(new object());

            Assert.Equal(true, eventFired);
        }
    }
}