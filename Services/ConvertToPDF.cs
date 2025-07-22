using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using System.Runtime.Versioning;
using Syncfusion.XlsIO;
using Syncfusion.Presentation;
using Syncfusion.DocIO.DLS;
using Syncfusion.ExcelToPdfConverter;
using Syncfusion.OfficeChartToImageConverter;
using Syncfusion.PresentationToPdfConverter;
using Syncfusion.DocToPDFConverter;

namespace PDF_Merger.Services
{
    internal class ConvertToPDF
    {
        [SupportedOSPlatform("windows6.1")]
        public static string ConvertImageToPdf(string imagePath)
        {
            // Fully qualify the PdfDocument type to resolve ambiguity  
            PdfDocument document = new();
            PdfPage page = document.Pages.Add();
            // Use fully qualified name for System.Drawing.Image  
            Image image = Image.FromFile(imagePath);

            // Get page size in points (1 point = 1/72 inch)  
            SizeF pageSize = page.GetClientSize(); // Updated to use Syncfusion.Drawing.SizeF  

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

            string imageName = Path.GetFileNameWithoutExtension(imagePath);
            string tempOutfile = Path.Combine(TempDir(), imageName + ".pdf");

            document.Save(tempOutfile);

            // Dispose objects  
            image.Dispose();
            document.Close(true);

            return tempOutfile;
        }

        internal static string ConvertExcelToPdf(string filePath)
        {
            using ExcelEngine excelEngine = new();
            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Xlsx;
            using FileStream excelStream = new(filePath, FileMode.Open, FileAccess.Read);
            IWorkbook workbook = application.Workbooks.Open(excelStream);

            // Set smaller margins for each worksheet
            foreach (IWorksheet sheet in workbook.Worksheets)
            {
                sheet.PageSetup.LeftMargin = 0.7;   // Inches
                sheet.PageSetup.RightMargin = 0.7;
                sheet.PageSetup.TopMargin = 0.7;
                sheet.PageSetup.BottomMargin = 0.7;
            }

            foreach (IWorksheet sheet in workbook.Worksheets)
            {
                // Clear header and footer text
                sheet.PageSetup.LeftHeader = string.Empty;
                sheet.PageSetup.CenterHeader = string.Empty;
                sheet.PageSetup.RightHeader = string.Empty;
                sheet.PageSetup.LeftFooter = string.Empty;
                sheet.PageSetup.CenterFooter = string.Empty;
                sheet.PageSetup.RightFooter = string.Empty;
            }


            ExcelToPdfConverter converter = new(workbook);
            // Set converter settings
            var settings = new ExcelToPdfConverterSettings
            {
                LayoutOptions = LayoutOptions.FitAllColumnsOnOnePage,
                EmbedFonts = true, // Embed fonts in the PDF
                ExportQualityImage = true,
                
                // Set the page size to A4
                // Optional: DisplayGridLines = GridLinesDisplayStyle.Visible
            };


            //Convert Excel document into PDF document 
            PdfDocument pdfDocument = converter.Convert(settings);

            string excelFileName = Path.GetFileNameWithoutExtension(filePath);
            string tempOutfile = Path.Combine(TempDir(), excelFileName + ".pdf");
            pdfDocument.Save(tempOutfile);
            return tempOutfile;
        }

        internal static string ConvertPowerPointToPdf(string filePath)
        {
            // Open the PowerPoint presentation  
            using IPresentation presentation = Presentation.Open(filePath);
            // Assign PresentationRenderer to enable PDF conversion  
            presentation.ChartToImageConverter = new ChartToImageConverter();

            PresentationToPdfConverterSettings settings = new()
            {
                ImageResolution = 300, // High resolution
                ImageQuality = 100, // Max quality
                EmbedFonts = true, // Embed fonts in the PDF
                EmbedCompleteFonts = true, // Embed complete fonts
                OptimizeIdenticalImages = true, // Optimize identical images
                AutoTag = true // Enable accessibility features
            };

            // Convert the presentation to PDF  
            using PdfDocument pdfDocument = PresentationToPdfConverter.Convert(presentation, settings);
            // Save the PDF document  

            string pptFileName = Path.GetFileNameWithoutExtension(filePath);
            string tempOutfile = Path.Combine(TempDir(), pptFileName + ".pdf");

            pdfDocument.Save(tempOutfile);
            return tempOutfile;
        }

        internal static string ConvertWordToPdf(string filePath)
        {
            // Open the Word document  
            using WordDocument wordDocument = new(filePath, Syncfusion.DocIO.FormatType.Automatic);
            // Initialize the DocIORenderer for Word-to-PDF conversion  
            
            wordDocument.ChartToImageConverter = new ChartToImageConverter(); // Set the ChartToImageConverter for rendering charts

            using DocToPDFConverter converter = new();
            converter.Settings.EmbedFonts = true;
            converter.Settings.ImageQuality = 100;
            converter.Settings.ImageResolution = 300; // DPI
            converter.Settings.OptimizeIdenticalImages = true; // Optimize identical images
            converter.Settings.AutoTag = true; // for accessibility

            // 4. Convert to PDF
            using PdfDocument pdfDocument = converter.ConvertToPDF(wordDocument);

            string docName = Path.GetFileNameWithoutExtension(filePath);
            string tempOutfile = Path.Combine(TempDir(), docName + ".pdf");

            // Save the converted PDF to file  
            pdfDocument.Save(tempOutfile);

            // Close the PDF document  
            pdfDocument.Close(true);
            return tempOutfile;
        }

        private static string TempDir()
        {
            // Save the document  
            string tempOutputDir = Path.Combine(Path.GetTempPath(), "Actom PDF Merger");
            Directory.CreateDirectory(tempOutputDir);
            return tempOutputDir;
        }
    }
}
