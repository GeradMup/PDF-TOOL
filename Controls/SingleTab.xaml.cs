using System;
using System.Collections.Generic;
using System.IO;
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
using PDF_Merger.ViewModels;
using PDF_Merger.Views;

namespace PDF_Merger.Controls
{
    /// <summary>
    /// Interaction logic for SingleTab.xaml
    /// </summary>
    public partial class SingleTab : UserControl
    {

        private string loadedFilePath = string.Empty;
        private bool isPdfLoaded = false;
        public SingleTab(PdfTabViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            if (File.Exists(vm.FilePath)) 
            {
                pdfViewer.Load(vm.FilePath);
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TryLoadPdf();
        }

        public void TryLoadPdf()
        {
            if (DataContext is PdfTabViewModel vm && File.Exists(vm.FilePath))
            {
                pdfViewer.Load(vm.FilePath);
                isPdfLoaded = true;
            }
        }

        public void LoadPdf(string filePath)
        {
            try
            {
                pdfViewer.Load(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
    }
}
