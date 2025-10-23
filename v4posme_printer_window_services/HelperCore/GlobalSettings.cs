namespace v4posme_printer_window_services.HelperCore;

public class GlobalSettings
{
    public string FolderPath { get; set; } = string.Empty;
    public int PrintIntervalSeconds { get; set; }
    public string PrinterName { get; set; } = string.Empty;
    public string PrefijoName { get; set; } = string.Empty;
    public int WidthPage { get; set; } = 283;
    public int Scale { get; set; } = 85;
    public int Copies { get; set; } = 1;
    public string TipoPrinter { get; set; } = string.Empty;
    public int PrintIntervalMinutes { get; set; }
    public string NombreProgramaHuella { get; set; } = "ProgramaDeHuella.exe";
    public string RutaProgramaHuella { get; set; } = @"C:\Program Files\Huella\";
    public Dictionary<string, JobConfig> Jobs { get; set; } = new();
}

public class JobConfig
{
    public bool Enabled { get; set; }
    public int Intervalo { get; set; }
}