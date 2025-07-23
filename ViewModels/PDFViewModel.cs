using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PDF_Merger.Controls;
using Syncfusion.DocIO.DLS;

namespace PDF_Merger.ViewModels
{

    public partial class PdfTabViewModel : ObservableObject
    {
        
        public string FilePath { get; set; } = string.Empty;

        public string Header => Path.GetFileNameWithoutExtension(FilePath);

        public string ToolTip => FilePath;

        public override string ToString() => FilePath;
    }

    public partial class PDFViewModel : ObservableObject
    {

        [ObservableProperty]
        public ObservableCollection<PdfTabViewModel> pdfTabs = [];

        [ObservableProperty]
        public PdfTabViewModel selectedTab;

        [ObservableProperty]
        private int selectedIndex;

        //public IRelayCommand<string> AddTabCommand { get; }
        //public IRelayCommand<PdfTab> CloseTabCommand { get; }
        private string SelectedFilePath { get; set; } = string.Empty;


        public PDFViewModel() 
        {
            //AddTabCommand = new RelayCommand<string>(AddTab);
            //CloseTabCommand = new RelayCommand<PdfTab>(tab => pdfTabs.Remove(tab));
        }

        internal void LoadPdf(string mergeFilePath)
        {
            try
            {
                AddTab(mergeFilePath);
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found, access denied)
                Console.WriteLine($"Error loading PDF: {ex.Message}");
            }
        }

        [RelayCommand]
        private void AddTab(string pathToPdf)
        {
            if (!File.Exists(pathToPdf)) return;

            PdfTabViewModel pdfItem = new() { FilePath = pathToPdf };
            PdfTabs.Add(pdfItem);
            SelectedTab = pdfItem; 
        }

        [RelayCommand]
        private void OpenPdf()
        {
            OpenFileDialog dialog = new()
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                AddTab(dialog.FileName);
                SelectedFilePath = dialog.FileName;
            }
            else 
            {
                SelectedFilePath = string.Empty;
            }
        }

        [RelayCommand]
        public void CloseTab(PdfTabViewModel tab)
        {
            PdfTabs.Remove(tab);
        }

        /// <summary>
        /// Open a FileDialog to select a PDF file and return its path.
        /// </summary>
        /// <returns>string</returns>
        internal string FilePath()
        {
            //Open a FileDialog to select a PDF file and get its path
            return SelectedFilePath;
        }
    }
}
