using Autofac;
using DBRestorer.Ctrl.Model;

namespace DBRestorer.Ctrl.Domain
{
    public class ViewModelLocator
    {
        private static IContainer _container;

        public MainWindowVm MainWindowVm => _container.Resolve<MainWindowVm>();

        public static void BootStrap()
        {
            var builder = new ContainerBuilder();

            // Usually you're only interested in exposing the type
            // via its interface:
            builder.RegisterType<SqlServerUtil>().As<ISqlServerUtil>();
            builder.RegisterInstance(new UserPreferencePersist()).As<IUserPreferencePersist>();
            builder.RegisterType<MainWindowVm>().AsSelf();
            builder.RegisterType<SqlInstancesVm>().AsSelf();
            _container = builder.Build();
        }
    }
}