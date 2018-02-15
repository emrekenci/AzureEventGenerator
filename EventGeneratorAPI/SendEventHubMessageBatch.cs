﻿using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.EventHubs;
using System.Text;
using System;
using EventGeneratorAPI.Models;
using System.Linq;

namespace EventGeneratorAPI
{
    public static class SendEventHubMessageBatch
    {
        [FunctionName("Job_SendEventHubMessageBatch")]
        public static async Task<string> Run([ActivityTrigger] EventHubJobProperties ehJobProperties, TraceWriter log)
        {
            EventHubsConnectionStringBuilder connectionStringBuilder = new EventHubsConnectionStringBuilder(ehJobProperties.ConnectionString)
            {
                EntityPath = ehJobProperties.EventHub
            };

            string connectionString = connectionStringBuilder.ToString();
            EventHubClient eventHubClient = EventHubClient.CreateFromConnectionString(connectionString);

            try
            {
                var ehMessages = ehJobProperties.Messages.Select(m => new EventData(Encoding.UTF8.GetBytes(m)));

                await eventHubClient.SendAsync(ehMessages);
                await eventHubClient.CloseAsync();
            }
            catch (Exception exception)
            {
                log.Info($"Exception: {exception.Message}");
            }

            log.Info($"sent batch of {ehJobProperties.Messages.Count()} messages");
            return $"finished sending {ehJobProperties.Messages.Count()} to {ehJobProperties.EventHub}!";
        }
    }
}