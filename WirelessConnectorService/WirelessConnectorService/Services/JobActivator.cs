namespace WirelessConnectorService.Services
{
    using System.IO;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Castle.Windsor.Installer;
    using Microsoft.Azure.WebJobs.Host;

    public sealed class JobActivator : IJobActivator
    {
        private readonly IWindsorContainer container;

        public JobActivator()
        {
            this.container = new WindsorContainer();
            this.InstallWindsorContainerInstallers();
        }

        public T CreateInstance<T>()
        {
            return this.container.Resolve<T>();
        }

        public void Release(object instance)
        {
            this.container.Release(instance);
        }

        private void InstallWindsorContainerInstallers()
        {
            this.container.Register(Castle.MicroKernel.Registration.Component.For<IWindsorContainer>().Instance(this.container).LifeStyle.Singleton);

            this.container.Install(FromAssembly.InDirectory(new AssemblyFilter(Directory.GetCurrentDirectory())));
        }
    }
}
