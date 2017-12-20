namespace WirelessConnectorService.DataContracts
{
    public class MeterInfo
    {
        public string PowerScout { get; set; }

        public string Breaker_details { get; set; }

        public int BuildingId { get; set; }

        public double UTCConversionTime { get; set; }
    }
}
