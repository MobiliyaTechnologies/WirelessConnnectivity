using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnergyManagement_IotHubReceiver_WebJob.Utilities;

namespace EnergyManagement_IotHubReceiver_WebJob.DataContracts
{
    public class NotificatioConfiguration
    {
        public string NotificationAuthorizationKey { get; set; }

        public string NotificationSender { get; set; }

        public string NotificationReceiver { get; set; }
        
    }
}
