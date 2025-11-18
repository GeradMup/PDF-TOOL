using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media.Imaging;
using Microsoft.Win32;
using PdfiumViewer;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf;
using static PDF_Merger.Services.Delegates;
using PDF_Merger.Models;

namespace PDF_Merger.Views
{
    public class PdfPageThumbnail
    {
        public BitmapImage Image { get; set; }
        public int PageNumber { get; set; } // 1-based page number
    }


    /// <summary>
    /// Interaction logic for SplitterPage.xaml
    /// </summary>
    public partial class SplitterPage : Page
    {

        private string currentFilePath = string.Empty;
        private readonly OnSplitComplete SplitComplete;
        public SplitterPage(OnSplitComplete onSplitComplete)
        {
            InitializeComponent();
            SplitComplete = onSplitComplete;
        }

        [SupportedOSPlatform("windows6.1")]
        public void LoadPdf()
        {
            OpenFileDialog dialog = new()
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Select a PDF file"
            };

            if (dialog.ShowDialog() == true)
            {
                string pdfPath = dialog.FileName;
                LoadPdfThumbnails(pdfPath);
            }
        }

        [SupportedOSPlatform("windows6.1")]
        public void LoadPdfThumbnails(string pdfPath)
        {
            if (!File.Exists(pdfPath))
            {
                MessageBox.Show("PDF file not found.");
                return;
            }

            currentFilePath = pdfPath;
            var thumbnails = new List<PdfPageThumbnail>();

            using var document = PdfiumViewer.PdfDocument.Load(pdfPath);
            for (int i = 0; i < document.PageCount; i++)
            {
                using var image = document.Render(i, 150, 200, 96, 96, false);
                using var memory = new MemoryStream();
                image.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = memory;
                bitmap.EndInit();
                bitmap.Freeze();

                thumbnails.Add(new PdfPageThumbnail
                {
                    Image = bitmap,
                    PageNumber = i + 1 // Human-friendly index
                });
            }

            ThumbnailGrid.ItemsSource = thumbnails;
        }


        private void SplitButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Save Split PDF"
            };

            if (dialog.ShowDialog() == true)
            {
                try
                {
                   var splitRanges = GetSplitRange(FirstPage.Text);
                    
                    SplitRange(currentFilePath, dialog.FileName, splitRanges);
                    MessageBox.Show("PDF split successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearInputs();
                    SplitComplete(dialog.FileName);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Please enter valid page numbers.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private static List<RangeObject> GetSplitRange(string input) 
        {
            // Split the given input by comma and trim whitespace
            string[] parts = [.. input.Split(',').Select(p => p.Trim())];

            List<RangeObject> ranges = [];
            foreach (string part in parts) 
            {
                string[] subParts = part.Split('-');
                if (subParts.Length == 1)
                {
                    RangeObject singleRange = new(int.Parse(part), int.Parse(part));
                    ranges.Add(singleRange);
                }
                else 
                {
                    RangeObject range = new(int.Parse(subParts[0]), int.Parse(subParts[1]));
                    ranges.Add(range);
                }
            }
            return ranges;
        }

        private void ClearInputs()
        {
            FirstPage.Text = string.Empty;
            //LastPage.Text = string.Empty;
        }

        public static void SplitRange(string inputPath, string outputPath, List<RangeObject> splitRanges)
        {
            using PdfLoadedDocument loadedDocument = new(inputPath);


            using Syncfusion.Pdf.PdfDocument newDocument = new();
/*
            for (int i = fromPage - 1; i < toPage; i++)  // 0-based indexing
            {
                newDocument.ImportPage(loadedDocument, i);
            }
*/
            foreach (RangeObject range in splitRanges)
            {

                if (range.Start < 1 || range.End > loadedDocument.Pages.Count || range.Start > range.End)
                    throw new Exception("Invalid page range.");

                for (int i = range.Start - 1; i < range.End && i < loadedDocument.Pages.Count; i++)
                {
                    newDocument.ImportPage(loadedDocument, i);
                }
            }

            using FileStream outputFileStream = new(outputPath, FileMode.Create, FileAccess.Write);
            newDocument.Save(outputFileStream);
        }
    }
}