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
using PDF_Merger.Controls;
using PDF_Merger.ViewModels;
using Syncfusion.Pdf;
using Syncfusion.Windows.Tools.Controls;

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

        private void OpenPdfButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.PDF",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                AddTab(dialog.FileName);
            }
        }

        private void AddTab(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            var vm = new PdfTabViewModel { FilePath = filePath };
            SingleTab pdfTab = new(vm);

            var tabItem = new TabItemExt
            {
                Header = vm.Header,
                Content = pdfTab,
                ToolTip = vm.ToolTip
            };

            TabControl.Items.Add(tabItem);
            TabControl.SelectedItem = tabItem;
        }
    }
}
