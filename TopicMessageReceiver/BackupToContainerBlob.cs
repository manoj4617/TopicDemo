using System.Configuration;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using System.Text;
using SharedLib.Models;

namespace SubscriptionReceiver
{
    public static class BackupToContainerBlob
    {
        static string connectionString = ConfigurationManager.AppSettings.Get("storageConnectionString");
        static string subscriptionName = ConfigurationManager.AppSettings.Get("subscriptionName");
        static string backupContainerName = ConfigurationManager.AppSettings.Get("backUpContainerName");

        public static async void BackupMessage(ServiceBusReceivedMessage message)
        {
            StringBuilder path = new StringBuilder(string.Empty);

            var blobServiceClient = new BlobServiceClient(connectionString);
            string subContainerName = subscriptionName;
          

            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(backupContainerName);

            blobContainerClient.CreateIfNotExistsAsync().Wait();

            path.Append(@subContainerName)
                .Append("\\" + message.EnqueuedTime.Year.ToString())
                .Append("\\" + message.EnqueuedTime.Month.ToString() + "\\");


            string fileName = message.EnqueuedTime.Day+"_"+message.MessageId + ".json";

            Directory.CreateDirectory(path.ToString());
            string mainFilePath = Path.Combine(path.ToString(), fileName);
            string data = message.Body.ToString();  


            await File.WriteAllTextAsync(mainFilePath, data);

            BlobClient blobClient = blobContainerClient.GetBlobClient(mainFilePath);

            try
            {
                await blobClient.UploadAsync(mainFilePath, true);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }
}
