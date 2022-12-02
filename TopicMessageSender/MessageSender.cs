using Azure.Messaging.ServiceBus;
using SharedLib;
using SharedLib.Models;
using System.Configuration;
using System.Text;
using System.Text.Json;

namespace TopicMessageSender
{
    public class MessageSender
    {
        static string connectionString = ConfigurationManager.AppSettings.Get("serviceBusConnectionString");

        static string topicName = ConfigurationManager.AppSettings.Get("topicName");

        static ServiceBusClient client;

        static ServiceBusSender sender;
        static async Task Main()
        {
            client = new ServiceBusClient(connectionString);

            sender = client.CreateSender(topicName);

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            for (int i = 1; i <= 30; ++i)
            {
                var person = new Device()
                {
                    SerialNumber = $"specflow" + new Random().Next(10000),
                    DeviceReferenceId = new Guid().ToString(),
                    Name = $"device-{i}"
                };
                string messageBody = JsonSerializer.Serialize(person);

                if (!messageBatch.TryAddMessage(new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody))))
                {
                    throw new Exception($"The message {i} is too large to fit in the batch.");
                }
            }
            try
            {
                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of 4 messages has been published to the topic.");
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}