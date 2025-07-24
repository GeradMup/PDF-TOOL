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
        public SplitterPage()
        {
            InitializeComponent();

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
        private void LoadPdfThumbnails(string pdfPath)
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
                    int fromPage = int.Parse(FirstPage.Text);
                    int toPage = int.Parse(LastPage.Text);
                    SplitRange(currentFilePath, dialog.FileName, fromPage, toPage);
                    MessageBox.Show("PDF split successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

        public static void SplitRange(string inputPath, string outputPath, int fromPage, int toPage)
        {
            using PdfLoadedDocument loadedDocument = new(inputPath);

            if (fromPage < 1 || toPage > loadedDocument.Pages.Count || fromPage > toPage)
                throw new ArgumentOutOfRangeException("Invalid page range.");

            using Syncfusion.Pdf.PdfDocument newDocument = new();

            for (int i = fromPage - 1; i < toPage; i++)  // 0-based indexing
            {
                newDocument.ImportPage(loadedDocument, i);
            }

            using FileStream outputFileStream = new(outputPath, FileMode.Create, FileAccess.Write);
            newDocument.Save(outputFileStream);
        }
    }
}