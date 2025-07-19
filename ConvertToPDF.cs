using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace PDF_Merger
{
    internal class ConvertToPDF
    {
        public static List<string> tempFiles = new List<string>();

        public static string Convert(string inputFilePath)
        {
            string sofficePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "soffice.exe");

            if (!File.Exists(sofficePath))
                throw new FileNotFoundException("LibreOffice executable not found at expected location.", sofficePath);

            if (!File.Exists(inputFilePath))
                throw new FileNotFoundException("Input file does not exist.", inputFilePath);

            string tempOutputDir = Path.Combine(Path.GetTempPath(), "MyAppPdfTemp");
            Directory.CreateDirectory(tempOutputDir);

            var process = new Process();
            process.StartInfo.FileName = sofficePath;
            process.StartInfo.Arguments = $"--headless --convert-to pdf \"{inputFilePath}\" --outdir \"{tempOutputDir}\"";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();
            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception($"LibreOffice conversion failed:\n{stderr}");

            string outputPdf = Path.Combine(tempOutputDir, Path.GetFileNameWithoutExtension(inputFilePath) + ".pdf");

            if (!File.Exists(outputPdf))
                throw new FileNotFoundException("Expected PDF output was not created.", outputPdf);

            tempFiles.Add(outputPdf);
            return outputPdf;
        }

        public static string ConvertImageToPdf(string imagePath)
        {
            using var document = new PdfDocument();
            var page = document.AddPage();
            page.Size = PdfSharp.PageSize.A4;

            using var gfx = XGraphics.FromPdfPage(page);
            using var img = XImage.FromFile(imagePath);

            // Convert image pixel size to points (72 DPI)
            double imgWidthPoints = img.PixelWidth * 72 / img.HorizontalResolution;
            double imgHeightPoints = img.PixelHeight * 72 / img.VerticalResolution;

            double maxWidth = page.Width.Point;
            double maxHeight = page.Height.Point;

            double finalWidth = imgWidthPoints;
            double finalHeight = imgHeightPoints;

            // Scale down if necessary (but never scale up)
            if (imgWidthPoints > maxWidth || imgHeightPoints > maxHeight)
            {
                double widthScale = maxWidth / imgWidthPoints;
                double heightScale = maxHeight / imgHeightPoints;
                double scale = Math.Min(widthScale, heightScale);

                finalWidth = imgWidthPoints * scale;
                finalHeight = imgHeightPoints * scale;
            }

            // Center the image on the page
            double x = (page.Width.Point - finalWidth) / 2;
            double y = (page.Height.Point - finalHeight) / 2;

            gfx.DrawImage(img, x, y, finalWidth, finalHeight);

            string tempOutputDir = Path.Combine(Path.GetTempPath(), "ActomPDF");
            Directory.CreateDirectory(tempOutputDir);

            string imageName = Path.GetFileNameWithoutExtension(imagePath);
            string tempOutfile = Path.Combine(tempOutputDir, imageName + ".pdf");

            document.Save(tempOutfile);
            document.Close();
            return tempOutfile;
        }
    }
}
