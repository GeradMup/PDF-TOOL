using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Xml.Serialization;
using PDF_Merger.ViewModels;
using PDF_Merger.Views;
using static PDF_Merger.Services.Delegates;
using Path = System.IO.Path;


namespace PDF_Merger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        readonly MainViewModel viewModel;

        private readonly MergePage mergePage;
        private readonly PDFViewer pdfViewer = new();
        private readonly SplitterPage splitterPage;

        readonly OnMergeComplete onMergeComplete;
        readonly OnSplitComplete onSplitComplete;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new();
            onMergeComplete = MergeCompleted;
            onSplitComplete = SplitCompleted;

            mergePage = new(onMergeComplete);
            splitterPage = new(onSplitComplete);

            DataContext = viewModel;
            MainFrame.Navigate(pdfViewer);
        }

        private void Pdfpage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(pdfViewer);
        }

        [SupportedOSPlatform("windows6.1")]
        private void PdfMerge_Click(object sender, RoutedEventArgs e)
        {
            // Before attempting to merge, check if we are currently displaying any docs. 
            // If yes, ask if the user wants to merge those?
            if (pdfViewer.DocObjects.Count > 1)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to merge the Documents in the Viewer?", "Merge Documents", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    mergePage.AddDocuments(pdfViewer.DocObjects);
                }
            }
            MainFrame.Navigate(mergePage);
        }

        private void MergeCompleted(string mergeFilePath)
        {
            pdfViewer.LoadPdf(mergeFilePath);
            MainFrame.Navigate(pdfViewer);
        }

        private void SplitCompleted(string splitFilePath)
        {
            pdfViewer.LoadPdf(splitFilePath);
            MainFrame.Navigate(pdfViewer);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            pdfViewer.LoadNewDoc();
            MainFrame.Navigate(pdfViewer);
        }

        [SupportedOSPlatform("windows6.1")]
        private void PdfSplitter_Click(object sender, RoutedEventArgs e)
        {
            // Ask the user if they want to split the currently loaded PDF in the viewer
            if (pdfViewer.DocObjects.Count > 0)
            {
                // If there are documents in the viewer, we can load the splitter page with the current PDF.
                // Ask the user if they want to split the currently loaded PDF.
                MessageBoxResult result = MessageBox.Show("Do you want to split the currently loaded PDF?", "Split PDF", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Load the PDF from the viewer into the splitter page
                    splitterPage.LoadPdfThumbnails(pdfViewer.PathOfActivePdf());
                }
                else
                {
                    // If no, just navigate to the splitter page without loading a PDF
                    // This will open a file dialog for the user to select a PDF to split
                    splitterPage.LoadPdf();
                }
            }
            else 
            {
                splitterPage.LoadPdf();
            }

            MainFrame.Navigate(splitterPage);   
        }
    }
}
