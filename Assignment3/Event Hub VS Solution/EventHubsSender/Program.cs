using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using EventModel;
using Newtonsoft.Json;

namespace Sender
{
    class Program
    {
        // Lamis Account
        // private const string eventHubName = "bikedata";
        // private const string connectionString = "Endpoint=sb://bikeeventshub1.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=/XclRW1Mwhmu7CS4B6rQg+ojmL25Mpp8Kpljhhh3vLc=";
        // Yomna Account
        private const string eventHubName = "flight_data";
        private const string connectionString = "Endpoint=sb://elg5166nsya.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Z+6hxqjt6waLg/chaPMHqbwHoy5yhjFbeAHfcuxfUkY=";
        private const string cachedEventSource = @"D:\DEBI\uOttawa\Cloud\ASGN3\bike_data.json";
        private static int numOfBatchesEvents = 50;
        private static int numBatchSize = 20;

        // The Event Hubs client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when events are being published or read regularly.
        static EventHubProducerClient producerClient;
        static BikeEvents bikeEvents = new BikeEvents();

        static async Task Main(string[] args)
        {
            // Number of batches
            if(args.Length > 0)
            {
                numOfBatchesEvents = Convert.ToInt32(args[0]);
            }

            // Batch size
            if(args.Length > 1)
            {
                numBatchSize = Convert.ToInt32(args[1]);
            }

            // Create a producer client that you can use to send events to an event hub
            producerClient = new EventHubProducerClient(connectionString, eventHubName);            

            // Read the Json Data
            bikeEvents.Initialize(cachedEventSource);

            try
            {
                for (int i = 1; i <= numOfBatchesEvents; i++)
                {
                    // Create a batch of events 
                    using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
                    var batch = bikeEvents.GetBatch(numBatchSize);

                    foreach (var bikeData in batch)
                    {
                        bikeData.ProcessTime = DateTime.Now;

                        if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bikeData)))))
                        {
                            // if it is too large for the batch
                            throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                        }

                    }

                    // Add artificial 2 seconds delay to the events
                    System.Threading.Thread.Sleep(2 * 1000);

                    // Use the producer client to send the batch of events to the event hub
                    await producerClient.SendAsync(eventBatch);
                    Console.WriteLine("A batch of {0} events has been published.", eventBatch.Count);
                }
            }
            finally
            {
                await producerClient.DisposeAsync();
            }            

            Console.WriteLine("Event publishing complete");

        }
    }
}
