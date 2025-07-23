using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PDF_Merger.Controls;
using PDF_Merger.ViewModels;

namespace PDF_Merger.Views
{
    /// <summary>
    /// Interaction logic for PDFViewer.xaml
    /// </summary>
    public partial class PDFViewer : Page
    {
        readonly PDFViewModel viewModel = new();
        public PDFViewer()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        internal void LoadPdf(string mergeFilePath)
        {
            //Load pdf file into the viewer
            viewModel.LoadPdf(mergeFilePath);
        }

        private void PdfTab_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is SingleTab pdfTab)
            {
                //pdfTab.LoadPdf(viewModel.FilePath());
            }
        }
    }
}
