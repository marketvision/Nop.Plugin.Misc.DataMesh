using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Core.Infrastructure;
using Nop.Core.Configuration;
using Autofac;

namespace Nop.Plugin.Misc.DataMesh.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //builder.RegisterType<DataLakeConsumer>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<StreamingOrderConsumer>().AsSelf().InstancePerLifetimeScope();
        }

        public int Order => 1123;
    }
}