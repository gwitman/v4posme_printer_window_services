using Autofac;
using Autofac.Extras.Quartz;
using log4net;
using log4net.Config;
using Topshelf;
using v4posme_printer_window_services.HelperCore;
using v4posme_printer_window_services.Service;

var log4NetPath = Path.Combine(AppContext.BaseDirectory, "log4net.config");
XmlConfigurator.Configure(new FileInfo(log4NetPath)); // <- ruta absoluta segura

var containerBuilder = new ContainerBuilder();

// Servicios y módulos
containerBuilder.RegisterType<MainService>();
containerBuilder.RegisterModule(new QuartzAutofacFactoryModule());
containerBuilder.RegisterModule(new QuartzAutofacJobsModule(typeof(Program).Assembly));

// Logger por clase (usando tipo base como fallback)
containerBuilder.Register(c =>
{
    var type = c.Resolve<IComponentContext>().ComponentRegistry.Registrations
        .FirstOrDefault(r => r.Activator.LimitType.Name.Contains("MainJob"))?.Activator.LimitType ?? typeof(Program);

    return LogManager.GetLogger(type);
}).As<ILog>().InstancePerDependency();

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

containerBuilder.RegisterInstance(configuration).As<IConfiguration>().SingleInstance();

// Registra PrintSettings como singleton
containerBuilder.Register(c =>
{
    var config      = c.Resolve<IConfiguration>();
    var settings    = new PrintSettings();
    config.GetSection("PrintSettings").Bind(settings);
    return settings;
}).As<PrintSettings>().SingleInstance();

var container = containerBuilder.Build();

// TOPSHELF HOST
HostFactory.Run(hostConfigurator =>
{
    hostConfigurator.SetServiceName("posMePrinterService");
    hostConfigurator.SetDisplayName("Servicio de Impresion de posMe");
    hostConfigurator.SetDescription("Busca archivos en el directorio y lo imprime.");

    hostConfigurator.RunAsLocalSystem();
    hostConfigurator.UseLog4Net(); // Solo para logs internos de Topshelf

    hostConfigurator.Service<MainService>(serviceConfigurator =>
    {
        serviceConfigurator.ConstructUsing(hostSettings => container.Resolve<MainService>());
        serviceConfigurator.WhenStarted(service => service.OnStart());
        serviceConfigurator.WhenStopped(service => service.OnStop());
    });
});