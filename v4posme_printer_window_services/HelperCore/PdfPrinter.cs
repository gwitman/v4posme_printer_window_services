using log4net;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Windows.Forms.PdfViewer;
using Syncfusion.Windows.PdfViewer;
using System.Drawing.Printing;

namespace v4posme_printer_window_services.HelperCore;

public class PdfPrinter(string pdfPath)
{
    public string Print(GlobalSettings settings, ILog log)
    {
        try
        {
            //inicializamos el documento
            using var pdfViewer = new PdfViewerControl();
            pdfViewer.Load(pdfPath);

            var printDoc                                    = pdfViewer.PrintDocument;
            pdfViewer.PrinterSettings.ShowPrintStatusDialog = false;
            pdfViewer.PrinterSettings.PageSize              = PdfViewerPrintSize.ActualSize;
            printDoc.DefaultPageSettings.Margins            = new Margins(0, 0, 0, 0);
            printDoc.OriginAtMargins                        = false;

            //tomamos la primera pagina para buscar alto y ancho
            var doc               = pdfViewer.LoadedDocument;
            var page              = doc.Pages[0];
            var pdfWidthPts       = page.Size.Width;
            var pdfHeightPts      = page.Size.Height;
            var pdfWidth          = (int)Math.Round(pdfWidthPts * 100 / 72);
            var pdfHeight         = (int)Math.Round(pdfHeightPts * 100 / 72);

            //ajustamos el ancho al configurado
            printDoc.DefaultPageSettings.PaperSize = new PaperSize("Ticket", settings.WidthPage, pdfHeight);

            //configuramos la impresora
            printDoc.PrinterSettings.Copies         = (short)settings.Copies;
            printDoc.PrinterSettings.PrinterName    = settings.PrinterName;

            //realizamos la impresion
            printDoc.Print();

            return $"Documento impreso correctamente: {pdfPath}";
        }
        catch (Exception ex)
        {
            return $"Error al imprimir: {ex.Message}";
        }
    }
}