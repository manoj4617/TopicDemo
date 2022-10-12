using System; 
using System.Configuration; 
using System.Threading.Tasks; 
using Azure.Storage.Queues; 
using Azure.Storage.Queues.Models;

namespace SubscriptionReceiver
{
    public static class SendMessageToStorageQueue
    {
        static string  connectionString = ConfigurationManager.AppSettings["storageConnectionString"];

        static string queueName = "topicarchives";

        public static void InsertMessage(string message)
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

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
