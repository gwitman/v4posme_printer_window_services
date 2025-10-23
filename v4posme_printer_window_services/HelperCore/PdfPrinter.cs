using System.Drawing.Printing;
using log4net;
using Patagames.Pdf.Net;
using Patagames.Pdf.Net.Controls.WinForms;

namespace v4posme_printer_window_services.HelperCore;

public class PdfPrinter(string pdfPath)
{
    public string Print(GlobalSettings settings, ILog log)
    {
        try
        {
            PdfCommon.Initialize();
            using var document = PdfDocument.Load(pdfPath);
            using var printDoc = new PdfPrintDocument(document);
            printDoc.PrintController                = new StandardPrintController();
            printDoc.PrinterSettings.PrinterName    = settings.PrinterName;
            log.Info($"Document Paper Size: {printDoc.DefaultPageSettings.PaperSize}");
            foreach (PaperSize paperSize in printDoc.PrinterSettings.PaperSizes)
            {
                log.Info($"Printer Paper size: {paperSize}");
            }

            printDoc.PrinterSettings.Copies     = (short)settings.Copies;
            printDoc.PrinterSettings.FromPage   = 1;
            printDoc.PrinterSettings.ToPage     = document.Pages.Count;
            printDoc.DefaultPageSettings.Color  = true;

            if (settings.TipoPrinter.Contains("pos", StringComparison.InvariantCultureIgnoreCase))
            {
                if (settings.Scale > 0)
                {
                    printDoc.PrintSizeMode  = PrintSizeMode.CustomScale;
                    printDoc.Scale          = settings.Scale;
                }
                else
                {
                    printDoc.PrintSizeMode = PrintSizeMode.ActualSize;
                }

                printDoc.AutoCenter         = false;
                printDoc.OriginAtMargins    = true;
                printDoc.DefaultPageSettings.Margins                = new Margins(0, 0, 0, 0);
                printDoc.DefaultPageSettings.PrinterResolution.Kind = PrinterResolutionKind.High;
                printDoc.DefaultPageSettings.PaperSize              = new PaperSize("Custom", settings.WidthPage, printDoc.DefaultPageSettings.PaperSize.Height);
                printDoc.DefaultPageSettings.Landscape              = false;
            }

            printDoc.Print();
            return $"Documento se ha impreso correctamente {pdfPath}";
        }
        catch (Exception ex)
        {
            return ($"Error al imprimir: {ex.Source} {ex.Message}");
        }
    }
}