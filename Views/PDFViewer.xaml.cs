using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
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
using Microsoft.Win32;
using PDF_Merger.Controls;
using PDF_Merger.ViewModels;
using Syncfusion.Pdf;
using Syncfusion.Windows.PdfViewer;
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
            AddTab(mergeFilePath);
        }

        private void OpenPdfButton_Click(object sender, RoutedEventArgs e)
        {
            LoadNewDoc();
        }

        private void AddTab(string filePath)
        {
            if (!File.Exists(filePath))
                 return;

            // Check if the file is already open

            foreach (TabItemExt tab in TabControl.Items.OfType<TabItemExt>())
            {
                if (tab.Content is SingleTab singleTab)
                {
                    if (singleTab.FilePath == filePath)
                    {
                        TabControl.SelectedItem = tab;
                        return;
                    }
                }
            }

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

        private void TabControl_NewButtonClick(object sender, EventArgs e)
        {
            LoadNewDoc();
        }

        public void LoadNewDoc() 
        {
            var dialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.PDF",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                AddTab(dialog.FileName);
            }
        }

        private void TabControl_OnCloseButtonClick(object sender, CloseTabEventArgs e)
        {
            // Get the current tab item
            TabItemExt tabItem = (sender as TabControl).SelectedItem as TabItemExt;

            // Get the SingleTab from the tab item's content
            SingleTab singleTab = tabItem.Content as SingleTab;

            // Get the PdfViewerControl from the tab item's content
            PdfViewerControl pdfViewer = singleTab.GetPdfViewer();

            // Check if the document has been modified
            if (pdfViewer.IsDocumentEdited)
            {
                // Prompt the user to save
                MessageBoxResult result = MessageBox.Show("Do you want to save changes?", "Save?", MessageBoxButton.YesNoCancel);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        // Save the document
                        pdfViewer.Save(singleTab.FilePath);
                        break;
                    case MessageBoxResult.No:
                        // Don't save, just close
                        break;
                    case MessageBoxResult.Cancel:
                        // Cancel closing the tab
                        e.Cancel = true;
                        break;
                }
            }
        }
    }
}
