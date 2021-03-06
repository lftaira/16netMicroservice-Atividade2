﻿using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Productor
{
    class Program
    {

        const string ConnectionString = "Endpoint=sb://sbtaira.servicebus.windows.net/;SharedAccessKeyName=ProductPolicy;SharedAccessKey=0OZlOB5kPuyHQ9mV/B9IAihsGPreJRi8NdBEcCGhMN8=";
        const string QueuePath = "productchanged";
        static IQueueClient _queueClient;
        static void Main(string[] args)
        {
            SendMessagesAsync().GetAwaiter().GetResult();
            Console.WriteLine("messages were sent");
            Console.ReadLine();

        }

        private static async Task SendMessagesAsync()
        {
            _queueClient = new QueueClient(ConnectionString, QueuePath);
            var messages = "Hi,Hello,Hey,How are you,Be Welcome"
                .Split(",")
                .Select(msg =>
                {
                    Console.WriteLine($"Will send message: {msg}");
                    return new Message(Encoding.UTF8.GetBytes(msg));
                })
                        .ToList();
            await _queueClient.SendAsync(messages);
            await _queueClient.CloseAsync();
        }
        public bool CheckCommunicationExceptions(Task task)
        {
            if (task.Exception == null || task.Exception.InnerExceptions.Count == 0) return true;

            task.Exception.InnerExceptions.ToList()
                .ForEach(innerException =>
            {
                Console.WriteLine($"Error in SendAsync task:{ innerException.Message}.Details:{ innerException.StackTrace}");
                if (innerException is ServiceBusCommunicationException)
                    Console.WriteLine("Connection Problem with Host");
            });
            return false;
        }

    }
}
