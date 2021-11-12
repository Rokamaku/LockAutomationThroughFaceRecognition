
// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}


using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Para.LockAutomation.Service;
using Para.LockAutomation.Utils;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.ApplicationInsights;
using Para.LockAutomation.Models;
using AzureFunctions.Autofac;
using Para.LockAutomation.Function;

namespace Para.LockAutomation.Function.IdentifyPerson
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class IdentifyPerson
    {
        [FunctionName("IdentifyPerson")]
        public static async Task Run(
            [EventGridTrigger]EventGridEvent eventGridEvent,
            [Blob("{data.url}", FileAccess.Read, Connection = "InputStorage")]Stream inputBlob,
            [Inject]FaceLogService faceLogService,
            [Inject]ServiceClient iotClient,
            [Inject]TelemetryClient telementry,
            [Inject]IConfiguration config,
            ILogger log)
        {
            try
            {
                if (eventGridEvent.EventType != "Microsoft.Storage.BlobCreated")
                {
                    return;
                }
                var createdEvent = ((JObject)eventGridEvent.Data).ToObject<StorageBlobCreatedEventData>();
                AzureFile file = new AzureFile
                {
                    Uri = createdEvent.Url,
                    FileName = createdEvent.Url.Split('/').Last(),
                    FilePath = createdEvent.Url.Split('/').Last()
                };

                var result = await faceLogService.CreateFaceLogAfterUploadForDefaultGroup(file, inputBlob);
                if (result.Persons.Any(ps => ps != ParaConsts.UnknownPersonId))
                {
                    CloudToDeviceMethod c2dMethod = new CloudToDeviceMethod("Unlock");
                    c2dMethod.SetPayloadJson("{'isOn': 'True','time': '5.0'}");
                    await iotClient.InvokeDeviceMethodAsync(config["IotHub:DeviceID:para-rpi"], c2dMethod);
                }
            }
            catch (IotHubException ex)
            {
                telementry.TrackException(ex);
                log.LogInformation(ex.Code.ToString());
                throw ex;
            }
        }
    }
}
