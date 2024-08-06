using OpenTel.Book.Contracts;
using OpenTel.RabbitMQ;

namespace OpenTel.Api.Features;

internal class EventsPublisher
{
    private readonly RabbitMqPublisher _rabbitMqPublisher;
    private readonly bool _simulateFailure;
    private static readonly Random Random = new();

    public EventsPublisher(RabbitMqPublisher rabbitMqPublisher, bool simulateFailure)
    {
        _rabbitMqPublisher = rabbitMqPublisher;
        _simulateFailure = simulateFailure;
    }

    public void Publish(Book.Contracts.Book book)
    {
        if (_simulateFailure)
        {
            var chance = Random.Next(100);
            if (chance < 25)
            {
                throw new InvalidOperationException("Simulated failure. Can't publish the event.");
            }
        }

        _rabbitMqPublisher.Publish(new GetBookRequested(book, DateTimeOffset.UtcNow),
            "book.events");
    }
}