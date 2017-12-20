namespace EnergyManagement_IotHubReceiver_WebJob.Services
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public sealed class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().InNamespace("EnergyManagement_IotHubReceiver_WebJob.Services")
                .LifestyleSingleton());

            container.Register(Classes.FromThisAssembly().InNamespace("EnergyManagement_IotHubReceiver_WebJob.Functions")
             .LifestyleSingleton());
        }
    }
}
