using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using GroupDocs.Conversion.Contracts;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using Microsoft.Win32;
using System.IO;
using GroupDocs.Conversion.Options.Convert;
using GroupDocs.Conversion;

namespace PDF_Merger
{
    public partial class MainViewModel : ObservableObject
    {

        [ObservableProperty]
        public ObservableCollection<string> documents = [];

        readonly List<DocObject> DocObjects = [];

        readonly List<string> TempFilePaths = [];

        public int GetIndex(string fileName)
        {
            return Documents.IndexOf(fileName);
        }

        public void ReOrder(int sourceItemIndex, int targetItemIndex)
        {
            Documents.Move(sourceItemIndex, targetItemIndex);
        }

        public void AddDocument(string fileName, string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);


            //Check if the input file is an image
            //Check if the input file is an image
            string[] imageExtensions = [".BMP", ".JPEG", ".JPG", ".PNG", ".GIF", ".TIFF", ".SVG"];
            bool isImageFile = imageExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));


            if (isImageFile) 
            {
                filePath = ConvertToPDF.ConvertImageToPdf(filePath);
            }




            if (!Documents.Contains(fileName))
            {
                Documents.Add(fileName);
                DocObjects.Add(new DocObject(fileName, filePath));
            }
        }



        public void RemoveDocument(string fileName)
        {
            // Directly attempt to remove the item without checking Contains
            if (Documents.Remove(fileName))
            {
                DocObjects.Remove(DocObjects.Where(obj => obj.DocumentName == fileName).Single());
            }
        }

        public void MergeFiles()
        {
            try
            {
                PdfDocument mergedDocs = new();

                if (Documents.Count == 0) throw new Exception();

                int globalPageNumber = 0;

                foreach (String docName in Documents)
                {
                    string filePath = DocObjects.First(obj => obj.DocumentName == docName).FilePath;

                    bool indexAdded = false;
                    //Open each document that needs to be ended to the combined document at the end of the day
                    PdfDocument inputDoc = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);

                    //Iterate through every single page in the doc
                    int count = inputDoc.PageCount;
                    for (int pageNumber = 0; pageNumber < count; pageNumber++)
                    {
                        PdfPage page = inputDoc.Pages[pageNumber];
                        mergedDocs.Pages.Add(page);
                        if (indexAdded == false)
                        {
                            mergedDocs.Outlines.Add(docName, mergedDocs.Pages[globalPageNumber]);
                            indexAdded = true;
                        }
                        globalPageNumber++;
                    }
                }

                //Save the final merged document
                string mergedDocumentPath = GetFilePath();
                mergedDocs.Save(mergedDocumentPath);

                //Show that the process was completed
                MessageBox.Show("Documents have been merged successfully!", "SUCCESS!", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                throw new Exception("Failed to merge documents! Please check if the files are valid PDF files.", ex);
            }
        }

        private static string GetFilePath()
        {
            SaveFileDialog fileDialog = new()
            {
                Filter = "Pdf Files|*.pdf",
                Title = "Insert file name"
            };

            string fileName = "";
            if (fileDialog.ShowDialog() == true)
            {
                if (fileDialog.FileName.Trim() == "") throw new Exception("Failed!");

                fileName = fileDialog.FileName;
            }

            return fileName;
        }

        public static bool ExtensionAllowed(string fileExtension)
        {
            string[] allowedExtensions =
            {
               ".PDF", ".XPS", ".TEX",
               ".DOC", ".DOCX", ".DOCM", ".DOT", ".DOTX", ".DOTM", ".RTF", ".TXT",
               ".PPT", ".PPTX", ".PPS", ".PPSX", ".ODP", ".OTP",
               ".XLS", ".XLSX", ".XLSM", ".XLSB", ".XLSX", ".CSV",
               ".BMP", ".JPEG", ".PNG", ".GIF", ".TIFF", ".SVG", ".PS"
           };

            // Use LINQ's Any method with StringComparison.OrdinalIgnoreCase for case-insensitive comparison  
            if (!allowedExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            return true;
        }
    }
}
