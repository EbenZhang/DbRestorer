using Autofac;
using DBRestorer.Domain;
using DBRestorer.Model;

namespace DBRestorer
{
    public class ViewModelLocator
    {
        private static IContainer _container;

        public MainWindowVm MainWindowVm
        {
            get { return _container.Resolve<MainWindowVm>(); }
        }

        public static void BootStrap()
        {
            var builder = new ContainerBuilder();

            // Usually you're only interested in exposing the type
            // via its interface:
            builder.RegisterType<SqlServerUtil>().As<ISqlServerUtil>();
            builder.RegisterType<MainWindowVm>().AsSelf();
            builder.RegisterType<SqlInstancesVM>().AsSelf();
            _container = builder.Build();
        }
    }
}