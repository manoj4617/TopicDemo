using Azure.Messaging.ServiceBus;
using SharedLib.Models;
using System.Configuration;
using System.Text.Json;

namespace SubscriptionReceiver
{
    public class MessageReceiver
    {
        public  int SpecflowMessagesCount { get; set; }
        public  int NonspecflowMessages { get; set; }
        public MessageReceiver()
        {
            SpecflowMessagesCount = 0;
            NonspecflowMessages = 0;
        }

        string connectionString = ConfigurationManager.AppSettings.Get("serviceBusConnectionString");

        string topicName = ConfigurationManager.AppSettings.Get("topicName");

        string subscriptionName = ConfigurationManager.AppSettings.Get("subscriptionName");

        ServiceBusClient client;

        ServiceBusProcessor processor;

        async Task MessageHandler(ProcessMessageEventArgs args)
        {
            Console.WriteLine(args.Message);
            var messgeBody = JsonSerializer.Deserialize<Device>(args.Message.Body.ToString());
            if (messgeBody is not null /*&& IsMessageOlder(args.Message)*/)
            {
                if (!IsSpecflowMessage(messgeBody))
                {
                    BackupToContainerBlob.BackupMessage(args.Message);
                    NonspecflowMessages++;
                    Console.WriteLine($"Message sent to container blob");
                    await args.CompleteMessageAsync(args.Message);
                }
                else
                {
                    SpecflowMessagesCount++;
                    Console.WriteLine($"DELETED--specflow_message_id_{args.Message.MessageId} enqueued_on_{args.Message.EnqueuedTime}");
                    await args.CompleteMessageAsync(args.Message);
                }
            }
            else
            {
                await args.DeferMessageAsync(args.Message);
            }
        }

         Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message.ToString());
            return Task.CompletedTask;
        }

        public async Task HandleMessages()
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
                Console.WriteLine($"Total Specflow Messages--{SpecflowMessagesCount}");
                Console.WriteLine($"Total Non-Specflow Messages--{NonspecflowMessages}");
                Console.WriteLine("Stopped receiving messages");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }

        private bool IsSpecflowMessage(Device deviceMessage)
        {
            if (deviceMessage.SerialNumber.StartsWith("Specflow", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsMessageOlder(ServiceBusReceivedMessage deviceMessage)
        {
            return deviceMessage.EnqueuedTime.Year <= 2021 && deviceMessage.EnqueuedTime.Month < 12 ?
                true : false;
        }
    }
}
