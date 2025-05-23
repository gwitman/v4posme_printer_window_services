using log4net;
using Quartz;
using v4posme_printer_window_services.HelperCore;

namespace v4posme_printer_window_services.Service;

public class MainService(PrintSettings settings, IScheduler scheduler, ILog logger)
{
    private IScheduler Scheduler { get; }   = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
    private PrintSettings Settings { get; } = settings ?? throw new ArgumentNullException(nameof(settings));
    private ILog Logger { get; }            = logger ?? throw new ArgumentNullException(nameof(logger));

    public void OnStart()
    {
        try
        {
            if (Settings.PrintIntervalSeconds <= 0)
                throw new InvalidOperationException("El intervalo de impresión debe ser mayor a cero.");

            var job = JobBuilder
                .Create<MainJob>()
                .WithIdentity(nameof(MainJob), SchedulerConstants.DefaultGroup)
                .Build();

            var trigger = TriggerBuilder
                .Create()
                .WithIdentity($"{nameof(MainJob)}Trigger", SchedulerConstants.DefaultGroup)
                .ForJob(job)
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(Settings.PrintIntervalSeconds).RepeatForever())
                .Build();

            Scheduler.Start();
            Scheduler.ScheduleJob(job, trigger);
        }
        catch (Exception ex)
        {
            logger.Error(ex);
        }
    }

    public void OnPaused() => Scheduler.PauseAll();
    public void OnContinue() => Scheduler.ResumeAll();
    public void OnStop() => Scheduler.Shutdown();
}