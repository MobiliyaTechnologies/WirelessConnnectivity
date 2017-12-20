namespace WirelessConnectorService.Services
{
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public sealed class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().InNamespace("WirelessConnectorService.Services")
                .LifestyleSingleton());

            container.Register(Classes.FromThisAssembly().InNamespace("WirelessConnectorService.Functions")
             .LifestyleSingleton());
        }
    }
}
