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
using System.Windows.Threading;
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

            PdfViewer.ShowToolbar = true;
           
        }

        public PdfViewerControl GetPdfViewer() 
        {
            return PdfViewer;
        }

        private void PdfViewer_DocumentLoaded(object sender, EventArgs args)
        {
            // Wait until the visual tree and templates are ready
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (PdfViewer.Template?.FindName("PART_Toolbar", PdfViewer) is not DocumentToolbar toolbar) return;

                if (toolbar.Template?.FindName("PART_FileToggleButton", toolbar) is not ToggleButton fileButton) return;

                // Optional: Do something with the context menu
                ContextMenu FileContextMenu = fileButton.ContextMenu;

                if (FileContextMenu == null) return;
                foreach (MenuItem FileMenuItem in FileContextMenu.Items)
                {
                    if (FileMenuItem.Name == "PART_OpenMenuItem")
                        FileMenuItem.Visibility = System.Windows.Visibility.Collapsed;
                }

            }), DispatcherPriority.Loaded);
        }
    }
}
