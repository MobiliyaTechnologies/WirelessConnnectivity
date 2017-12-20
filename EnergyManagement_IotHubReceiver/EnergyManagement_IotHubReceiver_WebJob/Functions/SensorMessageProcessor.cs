namespace EnergyManagement_IotHubReceiver_WebJob.Functions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using EnergyManagement_IotHubReceiver_WebJob.DataContracts;
    using EnergyManagement_IotHubReceiver_WebJob.Services;
    using EnergyManagement_IotHubReceiver_WebJob.Utilities;
    using Microsoft.Azure.WebJobs;
    using Microsoft.ServiceBus.Messaging;

    public sealed class SensorMessageProcessor
    {
        private readonly QueueClient queueClient;
        private readonly SensorService sensorService;
        private Dictionary<string, int> sensors;

        public SensorMessageProcessor(SensorService sensorService)
        {
            this.sensorService = sensorService;
        }

        public async Task ProcessSensorMessage([TimerTrigger("* * * * * *")] TimerInfo timerInfo, TextWriter log)
        {
            this.sensors = this.sensorService.GetSensors();
            while (true)
            {
                var sensorDatas = new List<SensorData>();

                var sensors = this.sensorService.GetUpdatedSensorInfo();
                foreach (var sensorData in sensors)
                {
                    if (!this.sensors.ContainsKey(sensorData.SensorName))
                    {
                        sensorData.Sensor_Id = this.sensorService.AddSensor(sensorData.SensorName);
                        this.sensors.Add(sensorData.SensorName, sensorData.Sensor_Id.Value);
                    }
                    else
                    {
                        sensorData.Sensor_Id = this.sensors[sensorData.SensorName];
                    }

                    sensorDatas.Add(sensorData);
                }

                this.sensorService.AddSensorData(sensorDatas);
            }
        }
    }
}
