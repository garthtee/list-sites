using Autofac;
using System;
using System.Linq;

namespace GarthToland.ListSites
{
    public static class InversionOfControl
    {
        public static IContainer BuildContainer(Settings settings)
        {
            var thisAssembly = typeof(InversionOfControl).Assembly;
            var builder = new ContainerBuilder();

            builder
                .RegisterAssemblyTypes(thisAssembly)
                .Where(HasInterfaceWithMatchingName)
                .AsImplementedInterfaces();

            builder.RegisterType<MessageService>().As<IMessageService>();

            if (settings != null)
                builder.RegisterInstance(settings).As<ISettings>();

            return builder.Build();
        }

        private static bool HasInterfaceWithMatchingName(Type type)
        {
            string interfaceName = string.Concat("I", type.Name);
            return type.GetInterface(interfaceName) != null;
        }
    }
}
