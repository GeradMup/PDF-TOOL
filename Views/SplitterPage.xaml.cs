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
using Microsoft.Win32;

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
        }
    }
}
