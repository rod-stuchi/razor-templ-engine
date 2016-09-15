using RodStu.RazorTemplateEngine.Engine;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TuesPechkin;

namespace RodStu.RazorTemplateEngine.Pdf
{
    public class PdfCreator
    {
        public static byte[] MakePdf(string content, PdfConfiguration pdfConfig)
        {
            if (pdfConfig == null)
            {
                pdfConfig = new PdfConfiguration();
            }

            var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
            string pageArgs = pdfConfig.PageNumbersFooter ? @"--footer-right ""[page] / [toPage]""" : "";

            htmlToPdf.CustomWkHtmlArgs = $@"
                --print-media-type 
                --footer-font-size 8 
                --image-quality 94 
                --encoding ""utf-8""
                --dpi {(int)pdfConfig.Quality} 
                --image-dpi {(int)pdfConfig.Quality} 
                --title ""{pdfConfig.Title}""
                --zoom {pdfConfig.Zoom}"
                .Replace("\r\n", "")
                .ReplaceRx(@"\s+", " ");

            htmlToPdf.Margins = new NReco.PdfGenerator.PageMargins
            {
                Top = pdfConfig.MarginTop,
                Left = pdfConfig.MarginLeft,
                Bottom = pdfConfig.MarginBottom,
                Right = pdfConfig.MarginRight
            };

            htmlToPdf.CustomWkHtmlPageArgs = pageArgs;

            var pdfBytes = htmlToPdf.GeneratePdf(content);

            return pdfBytes;

        }
    }
}
