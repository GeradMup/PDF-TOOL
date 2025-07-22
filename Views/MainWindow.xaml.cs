using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using PDF_Merger.ViewModels;
using PDF_Merger.Views;
using Path = System.IO.Path;


namespace PDF_Merger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        readonly MainViewModel viewModel;

        private readonly MergePage mergePage = new();
        private readonly PDFViewer pdfViewer = new();
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new();
            DataContext = viewModel;
            MainFrame.Navigate(mergePage);
        }
    }
}
