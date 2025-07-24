using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PDF_Merger.ViewModels;
using PDF_Merger.Views;
using Syncfusion.Windows.PdfViewer;

namespace PDF_Merger.Controls
{
    /// <summary>
    /// Interaction logic for SingleTab.xaml
    /// </summary>
    public partial class SingleTab : UserControl
    {
        public string FilePath { get; }

        public bool IsModified { get; private set; } = false;

        public SingleTab(PdfTabViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            if (File.Exists(vm.FilePath)) 
            {
                PdfViewer.Load(vm.FilePath);
            }
            FilePath = vm.FilePath;

            //Hook to all document change events
           
        }

        public PdfViewerControl GetPdfViewer() 
        {
            return PdfViewer;
        }

        private void PdfViewer_Loaded(object sender, RoutedEventArgs e)
        {
            DocumentToolbar toolbar = PdfViewer.Template.FindName("PART_Toolbar", PdfViewer) as DocumentToolbar;
            ToggleButton FileButton = (ToggleButton)toolbar.Template.FindName("PART_FileToggleButton", toolbar);
            ContextMenu FileContextMenu = FileButton.ContextMenu;
            foreach (MenuItem FileMenuItem in FileContextMenu.Items)
            {
                if (FileMenuItem.Name == "PART_OpenMenuItem")
                    FileMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
