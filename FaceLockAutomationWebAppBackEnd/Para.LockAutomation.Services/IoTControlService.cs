using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Para.LockAutomation.Service
{
    public class IoTControlService
    {
        private readonly ServiceClient _iotService;

        public IoTControlService(ServiceClient iotService)
        {
            _iotService = iotService;
        }

        //public async Task UnlockDoor()
        //{
           
        //}
    }

    public class LockControl
    {
        public bool IsOn { get; set; }
        public float Time { get; set; }
    }
}
