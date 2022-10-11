using System.Configuration;

namespace SharedLib
{
    public static class ServiceBusSetup
    {
        public static string ConnectionString = ConfigurationManager.AppSettings.Get("serviceBusConnectionString");
        public static string TopicName = ConfigurationManager.AppSettings.Get("topicName");
    }
}