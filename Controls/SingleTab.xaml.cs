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
using PDF_Merger.ViewModels;

namespace PDF_Merger.Controls
{
    /// <summary>
    /// Interaction logic for SingleTab.xaml
    /// </summary>
    public partial class SingleTab : UserControl
    {
        public SingleTab()
        {
            InitializeComponent();
            
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

        private void PdfViewer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is PdfTab tabItem)
            {
                pdfViewer.Load(tabItem.FilePath);
            }
        }
    }
}
