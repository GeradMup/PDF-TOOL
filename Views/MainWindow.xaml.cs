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

        private void PdfMerge_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(mergePage);
        }

        private void MergeCompleted(string mergeFilePath)
        {
            pdfViewer.LoadPdf(mergeFilePath);
            MainFrame.Navigate(pdfViewer);
        }
    }
}
