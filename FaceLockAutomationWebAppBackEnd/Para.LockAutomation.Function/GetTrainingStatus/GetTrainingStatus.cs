using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunctions.Autofac;
using Para.LockAutomation.Service;
using System.Net.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Para.LockAutomation.Models;
using System.Threading;

namespace Para.LockAutomation.Function.GetTrainingStatus
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class GetTrainingStatus
    {
        [FunctionName("Negotiate")]
        public static SignalRConnectionInfo GetSignalRInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, methods: "GET", Route ="getSignalRInfo")] HttpRequest req,
            [SignalRConnectionInfo(HubName = "TrainingStatus")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }

        [FunctionName("GetTrainingStatus")]
        public static async Task Run(
            [HttpTrigger(authLevel: AuthorizationLevel.Anonymous, methods: "GET", Route = "trainingStatus/{personGroupId:int?}")]HttpRequestMessage req,
            int? personGroupId,
            [Inject]TrainingService trainingService,
            [SignalR(HubName = "TrainingStatus")]IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");
            if (!personGroupId.HasValue)
            {
                return;
            }

            TrainingStatus newStatus = TrainingStatus.NotTrain;

            newStatus = await trainingService.GetTrainingStatus(personGroupId.Value);
            while (newStatus == TrainingStatus.NotTrain || newStatus == TrainingStatus.Running)
            {                               
                Thread.Sleep(2000);
                newStatus = await trainingService.GetTrainingStatus(personGroupId.Value);
            }

            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "fetchNewStatus",
                    Arguments = new dynamic[] 
                    {
                        new {
                            personGroupId = personGroupId.Value,
                            status = (int)newStatus
                        }
                    }
                });
        }
    }
}
