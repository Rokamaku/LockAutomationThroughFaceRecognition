using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Para.LockAutomation.Service
{
    public class ParaFuncService
    {
        public HttpClient Client { get; set; }

        public ParaFuncService(HttpClient client, IConfiguration config)
        {
            if (config["ASPNETCORE_ENVIRONMENT"]?.ToString() == "Development")
            {
                client.BaseAddress = new Uri(config["ParaFuncUrl:Development"]);
            }
            else
            {
                client.BaseAddress = new Uri(config["ParaFuncUrl:Production"]);
            }
            
            Client = client;
        }

        public void GetTrainingStatus(int personGroupId)
        {
            Client.GetAsync($"/api/trainingStatus/{personGroupId}");
        }

    }
}
