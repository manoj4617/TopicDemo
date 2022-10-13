using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SubscriptionReceiver
{
    public static class BackupToContainerBlob
    {
        static string connectionString = ConfigurationManager.AppSettings.Get("storageConnectionString");
        public static async void BackupMessage(ServiceBusReceivedMessage message)
        {
            var blobServiceClient = new BlobServiceClient(connectionString);
            string containerName = message.EnqueuedTime.Year.ToString();

            BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

            blobContainerClient.CreateIfNotExistsAsync().Wait();

            string subDirMonth = message.EnqueuedTime.Month.ToString();
            string subDirDay = message.EnqueuedTime.Day.ToString();

            string fileName = message.MessageId + ".json";

            string filePath = subDirMonth + '/' + subDirDay + '/' + fileName;
            string data = message.Body.ToString();


            await File.WriteAllTextAsync(filePath, data);

            BlobClient blobClient = blobContainerClient.GetBlobClient(filePath);

            await blobClient.UploadAsync(filePath, true);

        }
    }
}
