namespace Hexa.Core.Domain
{
    public interface IEventPublisher
    {
        void Publish<T>(T @event) where T : Event;
    }
}
