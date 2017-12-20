namespace EnergyManagement_IotHubReceiver_WebJob
{
    using EnergyManagement_IotHubReceiver_WebJob.Services;
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
