using System.Reflection;
using log4net;
using Quartz;
using v4posme_printer_window_services.HelperCore;

namespace v4posme_printer_window_services.Service;

public class MainService(GlobalSettings settings, IScheduler scheduler, ILog logger)
{
    private IScheduler Scheduler { get; }   = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
    private GlobalSettings Settings { get; } = settings ?? throw new ArgumentNullException(nameof(settings));

    public void OnStart()
    {
        try
        {
            Scheduler.Start();
            ConfigurarJobAndTrigger();
        }
        catch (Exception ex)
        {
            logger.Error(ex);
        }
    }

    private void ConfigurarJobAndTrigger()
    {
        if (Settings.Jobs.Count <= 0) return;
        foreach (var (jobName, config) in Settings.Jobs)
        {
            if (!config.Enabled)
            {
                continue;
            }
            ConfigureJob(jobName, config.Intervalo);
        }
    }
    
    private void ConfigureJob(string jobName, int intervalo)
    {
        if (intervalo <= 0)
        {
            logger.Warn($"Intervalo inválido para {jobName}. Usando valor por defecto de 2 segundos.");
            intervalo = 2;
        }

        // Obtener el tipo del job por convención de nombres
        var jobType = Type.GetType($"v4posme_printer_window_services.Service.{jobName}") ??
                      Assembly.GetExecutingAssembly().GetTypes()
                          .FirstOrDefault(t => t.Name.Equals(jobName, StringComparison.OrdinalIgnoreCase));

        if (jobType == null)
        {
            logger.Error($"No se encontró la clase para el job {jobName}");
            return;
        }

        var jobBuilder = JobBuilder
            .Create(jobType)
            .WithIdentity(jobName, SchedulerConstants.DefaultGroup)
            .Build();

        var trigger = TriggerBuilder
            .Create()
            .WithIdentity($"{jobName}Trigger", SchedulerConstants.DefaultGroup)
            .ForJob(jobBuilder)
            .StartNow()
            .WithSimpleSchedule(x => x.WithIntervalInSeconds(Settings.PrintIntervalSeconds).RepeatForever())
            .Build();

        Scheduler.ScheduleJob(jobBuilder, trigger);

        logger.Info($"Job {jobName} configurado con intervalo de {intervalo} segundos");
    }

    public void OnPaused() => Scheduler.PauseAll();
    public void OnContinue() => Scheduler.ResumeAll();
    public void OnStop() => Scheduler.Shutdown();
}