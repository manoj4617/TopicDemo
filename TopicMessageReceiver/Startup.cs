namespace SubscriptionReceiver
{
    class Startup
    {
        static async Task Main()
        {
            MessageReceiver messageReceiver = new MessageReceiver();

            await messageReceiver.HandleMessages();
        }
    }
}