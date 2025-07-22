using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Syncfusion.DocIO.DLS;

namespace PDF_Merger.ViewModels
{

    public partial class PdfTab : ObservableObject
    {
        [ObservableProperty]
        public string title;

        [ObservableProperty]
        public Stream documentStream;
    }

    public partial class PDFViewModel : ObservableObject
    {

        [ObservableProperty]
        public ObservableCollection<PdfTab> pdfTabs = [];

        [ObservableProperty]
        public PdfTab selectedTab;

        public IRelayCommand<string> AddTabCommand { get; }
        public IRelayCommand<PdfTab> CloseTabCommand { get; }



        public PDFViewModel() 
        {
            AddTabCommand = new RelayCommand<string>(AddTab);
            CloseTabCommand = new RelayCommand<PdfTab>(tab => pdfTabs.Remove(tab));
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

        private void AddTab(string pathToPdf)
        {
            var doc = new PdfTab
            {
                Title = Path.GetFileNameWithoutExtension(pathToPdf),
                DocumentStream = File.OpenRead(pathToPdf)
            };

            PdfTabs.Add(doc);
            SelectedTab = doc;
        }
    }
}
