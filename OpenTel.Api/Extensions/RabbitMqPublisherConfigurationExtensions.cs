using OpenTel.Api.Features;
using OpenTel.RabbitMQ;

namespace OpenTel.Api.Extensions;

public static class RabbitMqPublisherConfigurationExtensions
{
    public static WebApplicationBuilder AddRabbitMq(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(new RabbitMqPublisher(builder.Configuration.GetConnectionString("RabbitMq")!));
        builder.Services.AddSingleton<EventsPublisher>(sp =>
            new EventsPublisher(sp.GetRequiredService<RabbitMqPublisher>(),
                builder.Configuration.GetValue<bool>("Feature:PublishEventFailure")));

        return builder;
    }
}