using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfiumViewer;

namespace PDF_Merger.Services
{
    /// <summary>
    /// Class for initializing and configuring Pdfium for PDF operations.
    /// I want to use Pdfium for PDF operations, so this class makes sure that pdfium is properly initialized and configured.
    /// </summary>
    public static class PdfiumBootstrapper
    {
        public static void Initialize()
        {
            // Minimal 1-page blank PDF
            byte[] blankPdfBytes = Encoding.ASCII.GetBytes(
                "%PDF-1.1\n" +
                "1 0 obj<<>>endobj\n" +
                "2 0 obj<<>>endobj\n" +
                "3 0 obj<</Type/Pages/Count 1/Kids[4 0 R]>>endobj\n" +
                "4 0 obj<</Type/Page/Parent 3 0 R/MediaBox[0 0 300 300]/Contents 2 0 R>>endobj\n" +
                "5 0 obj<</Type/Catalog/Pages 3 0 R>>endobj\n" +
                "xref\n0 6\n0000000000 65535 f \n" +
                "0000000010 00000 n \n0000000052 00000 n \n0000000091 00000 n \n" +
                "0000000142 00000 n \n0000000211 00000 n \ntrailer\n" +
                "<</Size 6/Root 5 0 R>>\nstartxref\n270\n%%EOF"
            );

            using var memStream = new MemoryStream(blankPdfBytes);
            using var doc = PdfDocument.Load(memStream);
            using var bmp = doc.Render(0, 10, 10, 72, 72, false); // triggers native pdfium.dll load
        }
    }
}
