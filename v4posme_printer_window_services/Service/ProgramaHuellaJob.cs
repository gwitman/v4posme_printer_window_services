using System.Diagnostics;
using log4net;
using Quartz;
using v4posme_printer_window_services.HelperCore;

namespace v4posme_printer_window_services.Service;

public class ProgramaHuellaJob : IJob
{
    private readonly GlobalSettings settings;
    private readonly ILog log;

    public ProgramaHuellaJob(GlobalSettings settings, ILog log)
    {
        this.settings = settings;
        this.log = log;
    }

    public Task Execute(IJobExecutionContext context)
    {
        try
        {
            log.Info("Verificando programa de huella...");
            
            var procesoPath = Path.Combine(settings.RutaProgramaHuella, settings.NombreProgramaHuella);
            
            if (!File.Exists(procesoPath))
            {
                log.Error($"No se encontró el programa en la ruta: {procesoPath}");
                return Task.CompletedTask;
            }

            // Verificar si ya está en ejecución
            var procesos = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(settings.NombreProgramaHuella));
            
            if (procesos.Length == 0)
            {
                log.Info($"Iniciando programa: {settings.NombreProgramaHuella}");
                
                var startInfo = new ProcessStartInfo
                {
                    FileName            = procesoPath,
                    WorkingDirectory    = settings.RutaProgramaHuella,
                    UseShellExecute     = true
                };

                Process.Start(startInfo);
            }
            else
            {
                log.Info($"El programa {settings.NombreProgramaHuella} ya está en ejecución");
            }
        }
        catch (Exception ex)
        {
            log.Error($"Error al manejar el programa de huella", ex);
        }

        return Task.CompletedTask;
    }
}