using System.Drawing.Printing;
using log4net;
using PdfiumViewer;
using PdfDocument = PdfiumViewer.PdfDocument;

namespace v4posme_printer_window_services.HelperCore;

public class PdfPrinter(string pdfPath)
{
    public string Print(PrintSettings settings, ILog log)
    {
        try
        {
            using var document = PdfDocument.Load(pdfPath);
            using var printDocument = document.CreatePrintDocument(PdfPrintSettings.DefaultPrinterSettings);
            printDocument.PrintController = new StandardPrintController();
            // Configurar para imprimir todas las páginas
            printDocument.PrinterSettings.PrinterName = settings.PrinterName;
            printDocument.PrinterSettings.FromPage = 1;
            printDocument.PrinterSettings.ToPage = document.PageCount;
            log.Info(printDocument.DefaultPageSettings.PaperSize);
            foreach (PaperSize paperSize in printDocument.PrinterSettings.PaperSizes)
            {
                log.Info(paperSize);
            }
            if (settings.PrinterName.Contains(settings.TipoPrinter, StringComparison.InvariantCultureIgnoreCase))
            {
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("ThermalReceipt80 Custom", settings.WidthPage, 12898);
                printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            }
            
            printDocument.DefaultPageSettings.Color = true;
            printDocument.Print();
            return $"Documento se ha impreso correctamente {pdfPath}";
        }
        catch (Exception ex)
        {
            return ($"Error al imprimir: {ex.Source} {ex.Message}");
        }
    }
}