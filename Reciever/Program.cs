using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Reciever
{
    class Program
    {
        const string QueueConnectionString = "Endpoint=sb://sbtaira.servicebus.windows.net/;SharedAccessKeyName=ProductPolicy;SharedAccessKey=0OZlOB5kPuyHQ9mV/B9IAihsGPreJRi8NdBEcCGhMN8=";
        const string QueuePath = "productschanged";
        static IQueueClient _queueClient;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private static async Task ReceiveMessagesAsync()
        {
            _queueClient = new QueueClient(QueueConnectionString, QueuePath);
            _queueClient.RegisterMessageHandler(MessageHandler,
                new MessageHandlerOptions(ExceptionHandler) { AutoComplete = false });
            Console.ReadLine();
            await _queueClient.CloseAsync();

        }

        private static Task ExceptionHandler(ExceptionReceivedEventArgs exceptionArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionArgs.Exception}.");
            var context = exceptionArgs.ExceptionReceivedContext;
            Console.WriteLine($"Endpoint:{context.Endpoint}, Path:{context.EntityPath}, Action:{context.Action}");
            return Task.CompletedTask;
        }

        private static async Task MessageHandler(Message message, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Received message:{ Encoding.UTF8.GetString(message.Body)}");
            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }
    }
}
