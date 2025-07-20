using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using System.Runtime.Versioning;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Syncfusion.DocIORenderer;

namespace PDF_Merger
{
    internal class ConvertToPDF
    {
        [SupportedOSPlatform("windows6.1")]
        public static string ConvertImageToPdf(string imagePath)
        {
            PdfDocument document = new();
            PdfPage page = document.Pages.Add();
            // Use fully qualified name for System.Drawing.Image
            System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);

            // Get page size in points (1 point = 1/72 inch)
            Syncfusion.Drawing.SizeF pageSize = page.GetClientSize(); // Updated to use Syncfusion.Drawing.SizeF

            // Convert image size from pixels to points (assuming 96 DPI)
            float imageWidthPoints = image.Width * 72f / image.HorizontalResolution;
            float imageHeightPoints = image.Height * 72f / image.VerticalResolution;

            float scale = 1.0f;

            // Check if scaling is needed
            if (imageWidthPoints > pageSize.Width || imageHeightPoints > pageSize.Height)
            {
                float widthScale = pageSize.Width / imageWidthPoints;
                float heightScale = pageSize.Height / imageHeightPoints;
                scale = Math.Min(widthScale, heightScale);
            }

            float drawWidth = imageWidthPoints * scale;
            float drawHeight = imageHeightPoints * scale;

            // Center the image on the page
            float x = (pageSize.Width - drawWidth) / 2;
            float y = (pageSize.Height - drawHeight) / 2;

            PdfGraphics graphics = page.Graphics;

            MemoryStream ms = new();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            ms.Position = 0;

            PdfBitmap pdfImage = new(ms);
            graphics.DrawImage(pdfImage, x, y, drawWidth, drawHeight);

            // Save the document
            string tempOutputDir = Path.Combine(Path.GetTempPath(), "Actom PDF Merger");
            Directory.CreateDirectory(tempOutputDir);

            string imageName = Path.GetFileNameWithoutExtension(imagePath);
            string tempOutfile = Path.Combine(tempOutputDir, imageName + ".pdf");

            document.Save(tempOutfile);

            // Dispose objects
            image.Dispose();
            document.Close(true);

            return tempOutfile;
        }

        internal static string ConvertWordToPdf(string filePath)
        {
            // Open the Word document
            using WordDocument wordDocument = new(filePath, FormatType.Automatic);
            // Initialize the DocIORenderer for Word-to-PDF conversion
            using DocIORenderer renderer = new();
            // Convert the Word document to PDF
            PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument);

            // Save the document
            string tempOutputDir = Path.Combine(Path.GetTempPath(), "Actom PDF Merger");
            Directory.CreateDirectory(tempOutputDir);

            string docName = Path.GetFileNameWithoutExtension(filePath);
            string tempOutfile = Path.Combine(tempOutputDir, docName + ".pdf");

            // Save the converted PDF to file
            pdfDocument.Save(tempOutfile);

            // Close the PDF document
            pdfDocument.Close(true);
            return tempOutfile;
        }
    }
}
