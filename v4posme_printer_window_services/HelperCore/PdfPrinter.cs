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
            log.Info($"Iniciando impresión del archivo: {pdfPath}");

            //inicializamos el documento
            using var pdfViewer = new PdfViewerControl();
            pdfViewer.Load(pdfPath);
            log.Info($"Archivo {pdfPath} cargado correctamente en el visor PDF");

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
            log.Info($"Archivo {pdfPath} dimensiones PDF: {pdfWidthPts}pts x {pdfHeightPts}pts ({pdfWidth} x {pdfHeight} centésimas de pulgada)");

            //ajustamos el ancho al configurado
            printDoc.DefaultPageSettings.PaperSize = new PaperSize("Ticket", settings.WidthPage, settings.HeightPage);
            log.Info($"Archivo {pdfPath} tamaño de papel configurado: {settings.WidthPage} x {settings.HeightPage}");

            //configuramos la impresora
            printDoc.PrinterSettings.Copies         = (short)settings.Copies;
            printDoc.PrinterSettings.PrinterName    = settings.PrinterName;
            log.Info($"Archivo {pdfPath} impresora: {settings.PrinterName}, copias: {settings.Copies}");

            //realizamos la impresion
            printDoc.Print();

            var mensaje = $"Archivo {pdfPath} impreso correctamente en {settings.PrinterName}";
            log.Info(mensaje);
            return mensaje;
        }
        catch (Exception ex)
        {
            log.Error($"Archivo {pdfPath} error al imprimir: {ex.Message}", ex);
            return $"Error al imprimir: {ex.Message}";
        }
    }
}