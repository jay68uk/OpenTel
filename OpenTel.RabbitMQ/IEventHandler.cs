namespace OpenTel.RabbitMQ;

public interface IEventHandler<T>
{
    public Task HandleAsync(T @event);
}