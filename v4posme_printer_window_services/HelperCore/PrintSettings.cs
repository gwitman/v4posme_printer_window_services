namespace v4posme_printer_window_services.HelperCore;

public class PrintSettings
{
    public string FolderPath { get; set; } = string.Empty;
    public int PrintIntervalSeconds { get; set; }
    public string PrinterName { get; set; } = string.Empty;
    public string PrefijoName { get; set; } = string.Empty;
}