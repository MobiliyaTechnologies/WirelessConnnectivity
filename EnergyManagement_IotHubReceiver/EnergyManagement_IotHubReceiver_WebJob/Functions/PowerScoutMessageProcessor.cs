namespace EnergyManagement_IotHubReceiver_WebJob.Functions
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using EnergyManagement_IotHubReceiver_WebJob.DataContracts;
    using EnergyManagement_IotHubReceiver_WebJob.Services;
    using EnergyManagement_IotHubReceiver_WebJob.Utilities;
    using Microsoft.Azure.WebJobs;

    public sealed class PowerScoutMessageProcessor
    {
        private readonly PowerScoutService powerScoutService;

        private readonly Dictionary<string, int> buildings;
        private readonly List<string> powerScouts;

        public PowerScoutMessageProcessor(PowerScoutService powerScoutService)
        {
            this.powerScoutService = powerScoutService;

            this.buildings = this.powerScoutService.GetBuildings();
            this.powerScouts = this.powerScoutService.GetPowerScout();
        }

        public void ProcessData([ServiceBusTrigger("PowerScout_HalfHours")] PowerScoutData powerScoutData, TextWriter logger)
        {
            using (SqlConnection sqlConnection = new SqlConnection(ApplicationConfiguration.SqlConnectionString))
            {
                if (!this.buildings.ContainsKey(powerScoutData.Building))
                {
                    var buildingId = this.powerScoutService.AddBuilding(powerScoutData.Building);
                    this.buildings.Add(powerScoutData.Building, buildingId);
                }

                if (!this.powerScouts.Any(p => p.Equals(powerScoutData.PowerScout)))
                {
                    var meterInfo = new MeterInfo { Breaker_details = powerScoutData.Breaker_details, PowerScout = powerScoutData.PowerScout, BuildingId = this.buildings[powerScoutData.Building], UTCConversionTime = powerScoutData.UTCConversionTime };
                    this.powerScoutService.AddPowerScout(meterInfo);
                }

                this.powerScoutService.AddPowerScoutData(powerScoutData);
            }
        }
    }
}
