namespace WirelessConnectorService
{
    using WirelessConnectorService.Services;
    using Microsoft.Azure.WebJobs;

    public class WebJobHost
    {
        public static JobActivator JobActivator;

        public static void Main()
        {
            JobActivator = new JobActivator();
            var config = new JobHostConfiguration { JobActivator = JobActivator };
            config.UseTimers();

            var host = new JobHost(config);
            host.RunAndBlock();
        }
    }
}
