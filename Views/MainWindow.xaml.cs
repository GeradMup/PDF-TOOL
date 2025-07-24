using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
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
        private readonly SplitterPage splitterPage = new();

        readonly OnMergeComplete onMergeComplete;

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new();
            onMergeComplete = MergeCompleted;
            mergePage = new(onMergeComplete);

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

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            pdfViewer.LoadNewDoc();
            MainFrame.Navigate(pdfViewer);
        }

        private void PdfSplitter_Click(object sender, RoutedEventArgs e)
        {
            splitterPage.LoadPdf();
            MainFrame.Navigate(splitterPage);   
        }
    }
}
