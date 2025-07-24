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
        public SingleTab(PdfTabViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            if (File.Exists(vm.FilePath)) 
            {
                PdfViewer.Load(vm.FilePath);
            }
            FilePath = vm.FilePath;
        }


        public void LoadPdf(string filePath)
        {
            try
            {
                PdfViewer.Load(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Handles the Loaded event of the PdfViewer control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PdfViewer_Loaded(object sender, RoutedEventArgs e)
        {
            //Get the instance of the toolbar using its template name.
            DocumentToolbar toolbar = PdfViewer.Template.FindName("PART_Toolbar", PdfViewer) as DocumentToolbar;

            //Get the instance of the file menu button using its template name.
            ToggleButton FileButton = (ToggleButton)toolbar.Template.FindName("PART_FileToggleButton", toolbar);

            //Get the instance of the file menu button context menu and the item collection.
            ContextMenu FileContextMenu = FileButton.ContextMenu;

            foreach (MenuItem FileMenuItem in FileContextMenu.Items)
            {
                //Get the instance of the open menu item using its template name and disable its visibility.
                if (FileMenuItem.Name == "PART_OpenMenuItem")
                    FileMenuItem.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
