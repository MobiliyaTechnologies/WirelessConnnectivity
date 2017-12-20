namespace EnergyManagement_IotHubReceiver_WebJob.Utilities
{
    using System.Configuration;

    public static class ApplicationConfiguration
    {
        public static readonly string SqlConnectionString = GetConfigData("sql:ConnectionString");
        public static readonly string Client_Id = GetConfigData("ClientId");
        public static readonly string Client_Secret = GetConfigData("ClientSecret");
        public static readonly string Client_Code = GetConfigData("ClientCode");
        public static readonly string AuthApiSubAddess = GetConfigData("AuthApiSubAddress");
        public static readonly string BaseAddress = GetConfigData("BaseAddress");

        private static string GetConfigData(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
