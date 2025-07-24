using System;
using System.Collections.Generic;
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
using Microsoft.Win32;
using Syncfusion.Windows.PdfViewer;

namespace PDF_Merger.Views
{
    /// <summary>
    /// Interaction logic for SplitterPage.xaml
    /// </summary>
    public partial class SplitterPage : Page
    {
        public SplitterPage()
        {
            InitializeComponent();
        }

        public void LoadPdf() 
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Open PDF File"
            };

            if (dialog.ShowDialog() == true) 
            {
                SplitterPdf.Load(dialog.FileName);
            }

            // Hides the thumbnail icon. 
            SplitterPdf.ThumbnailSettings.IsVisible = false;
            // Hides the bookmark icon. 
            SplitterPdf.IsBookmarkEnabled = false;
            // Hides the layer icon. 
            SplitterPdf.EnableLayers = false;
            // Hides the organize page icon. 
            SplitterPdf.PageOrganizerSettings.IsIconVisible = false;
            // Hides the redaction icon. 
            SplitterPdf.EnableRedactionTool = false;
            // Hides the form icon. 
            SplitterPdf.FormSettings.IsIconVisible = false;
            SplitterPdf.ShowToolbar = false;
        }

        private void SplitterPdf_DocumentLoaded(object sender, EventArgs args)
        {
            // Wait until the visual tree and templates are ready
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (SplitterPdf.Template?.FindName("PART_Toolbar", SplitterPdf) is DocumentToolbar toolbar)
                {
                    if (toolbar.Template?.FindName("PART_ThumbnailToggleButton", toolbar) is ToggleButton thumbnailButton)
                    {
                        // You found the thumbnail toggle button!
                        thumbnailButton.Visibility = Visibility.Collapsed; // or attach handler
                    }
                }

            }), DispatcherPriority.Loaded);
        }
    }
}
