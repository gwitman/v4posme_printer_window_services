using PdfiumViewer;

namespace v4posme_printer_window_services.HelperCore;

public class PdfPrinter(string pdfPath)
{
    public string Print(string printerName ="Wondershare PDFelement")
    {
        try
        {
            // Cargar el documento PDF
            using var document                          = PdfDocument.Load(pdfPath);
            using var printDocument                     = document.CreatePrintDocument();
            // Configurar para imprimir todas las páginas
            printDocument.PrinterSettings.PrinterName               = printerName;
            printDocument.PrinterSettings.FromPage                  = 1;
            printDocument.PrinterSettings.ToPage                    = document.PageCount;
            printDocument.PrinterSettings.DefaultPageSettings.Color = true;
            printDocument.Print();
            return $"Documento se ha impreso correctamente {pdfPath}";
        }
        catch (Exception ex)
        {
            return ($"Error al imprimir: {ex.Message}");
        }
    }
}
