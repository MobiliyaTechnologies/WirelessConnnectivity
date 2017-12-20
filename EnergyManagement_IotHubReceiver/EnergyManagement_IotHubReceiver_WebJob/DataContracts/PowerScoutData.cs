using System;

namespace EnergyManagement_IotHubReceiver_WebJob.DataContracts
{
    public class PowerScoutData
    {
        public double AMPS_SYSTEM_AVG { get; set; }

        public string Breaker_details { get; set; }

        public string Building { get; set; }

        public int ClassOccupanyRemaining { get; set; }

        public int ClassOccupiedValue { get; set; }

        public int TotalClassCapacity { get; set; }

        public double Daily_electric_cost { get; set; }

        public double Daily_KWH_System { get; set; }

        public int IsClassOccupied { get; set; }

        public double Monthly_electric_cost { get; set; }

        public double Monthly_KWH_System { get; set; }

        public string PowerScout { get; set; }

        public double Pressure { get; set; }

        public double Relative_humidity { get; set; }

        public double Temperature { get; set; }

        public DateTime Timestamp { get; set; }

        public double Visibility { get; set; }

        public double KwSystem { get; set; }

        public string PiServerName { get; set; }

        public double UTCConversionTime { get; set; }
    }
}
