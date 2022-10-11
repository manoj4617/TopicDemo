using System;
using System.Configuration;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using SharedLib.Models;

namespace SubscriptionReceiver
{
    class MessageReceiver
    {
        static string connectionString = ConfigurationManager.AppSettings.Get("serviceBusConnectionString");

        static string topicName = ConfigurationManager.AppSettings.Get("topicName");

        static string subscriptionName = "sub-1";

        static ServiceBusClient client;

        static ServiceBusProcessor processor;

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string jsonString = Encoding.UTF8.GetString(args.Message.Body);
            Person person = JsonSerializer.Deserialize<Person>(jsonString);
            Console.WriteLine($"Person: {person.FirstName} {person.LastName} {person.Age}");

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        static async Task Main()
        {

            client = new ServiceBusClient(connectionString);

            processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions());

            try
            {
                processor.ProcessMessageAsync += MessageHandler;

                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync();

                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {

                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}