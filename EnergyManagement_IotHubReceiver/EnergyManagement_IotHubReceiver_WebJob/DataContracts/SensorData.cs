namespace EnergyManagement_IotHubReceiver_WebJob.DataContracts
{
    using System;

    public class SensorData
    {
        public string SensorName { get; set; }

        public int? Sensor_Id { get; set; }

        public double? Temperature { get; set; }

        public double? Brightness { get; set; }

        public double? Humidity { get; set; }

        public DateTime? Timestamp { get; set; }
    }
}
