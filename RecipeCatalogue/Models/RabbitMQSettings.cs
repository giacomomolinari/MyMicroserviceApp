namespace RecipeCatalogue.Models;

// used to store the RabbitMQ settings in the appsettings.json file
public class RabbitMQSettings
{
    public string ConnectionString { get; set; } = null!;

    public string BrokerName { get; set; } = null!;

    public string ServiceName { get; set; } = null!;

}