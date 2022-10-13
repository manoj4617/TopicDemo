using System; 
using System.Configuration; 
using System.Threading.Tasks; 
using Azure.Storage.Queues; 
using Azure.Storage.Queues.Models;

namespace SubscriptionReceiver
{
    public static class SendMessageToStorageQueue
    {
        static string  connectionString = ConfigurationManager.AppSettings["serviceBusConnectionString"];

        static string queueName = "topicarchives";

        public static void InsertMessage(string message)
        {

            QueueClient queueClient = new QueueClient(connectionString, queueName);

            queueClient.CreateIfNotExists();

            if (queueClient.Exists())
            {
                queueClient.SendMessage(message) ;
            }

            Console.WriteLine($"Inserted: {message}");
        }
    }
}
