using log4net;
using Quartz;
using v4posme_printer_window_services.HelperCore;

namespace v4posme_printer_window_services.Service;

public class MainJob(PrintSettings settings, ILog log) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        try
        {
            if (!Directory.Exists(settings.FolderPath))
            {
                log.Warn($"La carpeta configurada no existe: {settings.FolderPath}");
                return Task.CompletedTask;
            }

            var archivos = Directory.GetFiles(settings.FolderPath, $"{settings.PrefijoName}*.pdf");

            foreach (var archivo in archivos)
            {
                try
                {
                    log.Info($"Procesando archivo: {archivo}");

                    var printer     = new PdfPrinter(archivo);
                    var printResult = printer.Print(settings.PrinterName);

                    log.Info($"Resultado impresión: {printResult}");

                    var nuevoNombre = Path.Combine(settings.FolderPath, "impreso_" + Path.GetFileName(archivo));
                    File.Move(archivo, nuevoNombre);
                }
                catch (Exception exArchivo)
                {
                    log.Error($"Error al procesar el archivo {archivo}", exArchivo);
                }
            }
        }
        catch (Exception ex)
        {
            log.Error("Error durante la ejecución del trabajo de impresión", ex);
        }

        return Task.CompletedTask;
    }
}