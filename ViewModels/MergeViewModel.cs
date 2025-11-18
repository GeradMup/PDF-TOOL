using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf;
using System.Windows;
using System.IO;
using Syncfusion.Drawing;
using PDF_Merger.Models;
using PDF_Merger.Services;
using System.Drawing;

namespace PDF_Merger.ViewModels
{
    public partial class MergeViewModel : ObservableObject
    {
        [ObservableProperty]
        public ObservableCollection<string> documents = [];

        readonly List<DocObject> DocObjects = [];

        readonly List<string> TempFilePaths = [];

        private string mergedFilePath = string.Empty;

        public int GetIndex(string fileName)
        {
            return Documents.IndexOf(fileName);
        }

        public void ReOrder(int sourceItemIndex, int targetItemIndex)
        {
            Documents.Move(sourceItemIndex, targetItemIndex);
        }

        [SupportedOSPlatform("windows6.1")]
        public void AddDocument(string fileName, string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);


            //Check if the input file is an image
            //Check if the input file is an image
            string[] imageExtensions = [".BMP", ".JPEG", ".JPG", ".PNG", ".GIF", ".TIFF", ".SVG"];
            bool isImageFile = imageExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));

            //Check for word documents and convert them to PDF
            string[] wordExtensions = [".DOC", ".DOCX", ".DOCM", ".DOT", ".DOTX", ".DOTM", ".RTF", ".TXT"];
            bool isWordFile = wordExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));

            //Check for powerpoint documents and convert them to PDF
            string[] pptExtensions = [".PPT", ".PPTX", ".PPS", ".PPSX", ".ODP", ".OTP"];
            bool isPptFile = pptExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));

            //Check for excel documents and convert them to PDF
            string[] excelExtensions = [".XLS", ".XLSX", ".XLSM", ".XLSB", ".CSV"];
            bool isExcelFile = excelExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase));


            if (isImageFile)
            {
                filePath = ConvertToPDF.ConvertImageToPdf(filePath);
            }
            else if (isWordFile)
            {
                // Convert Word document to PDF
                filePath = ConvertToPDF.ConvertWordToPdf(filePath);
            }
            else if (isPptFile)
            {
                // Convert PowerPoint document to PDF
                filePath = ConvertToPDF.ConvertPowerPointToPdf(filePath);
            }
            else if (isExcelFile)
            {
                // Convert Excel document to PDF
                filePath = ConvertToPDF.ConvertExcelToPdf(filePath);
            }

            if (!Documents.Contains(fileName))
            {
                Documents.Add(fileName);
                DocObjects.Add(new DocObject(filePath));
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

                int currentPageIndex = 0;
                foreach (String docName in Documents)
                {
                    string filePath = DocObjects.First(obj => obj.DocumentName == docName).FilePath;
                    PdfLoadedDocument loadedDoc = new(filePath);

                    // Import pages
                    mergedDocs.ImportPageRange(loadedDoc, 0, loadedDoc.Pages.Count - 1);

                    // Add a bookmark to the first page of this document
                    PdfBookmark bookmark = mergedDocs.Bookmarks.Add(System.IO.Path.GetFileName(filePath));
                    bookmark.Destination = new PdfDestination(mergedDocs.Pages[currentPageIndex])
                    {
                        Location = new PointF(0, 0)
                    };

                    currentPageIndex += loadedDoc.Pages.Count;
                }

                //Avoid compression to maximize quality
                mergedDocs.Compression = PdfCompressionLevel.None;
                
                //Before saving, make sure the current file is not open in the very same project we are working on

                //Save the final merged document
                string mergedDocumentPath = GetFilePath();
                mergedDocs.Save(mergedDocumentPath);
                mergedDocs.Close();

                mergedFilePath = mergedDocumentPath;
                //Show that the process was completed
                MessageBox.Show("Documents have been merged successfully!", "SUCCESS!", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to merge documents! {ex.Message}");
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
                if (fileDialog.FileName.Trim() == "") throw new Exception("Failed to get file name for merging save!");

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
               ".BMP", ".JPEG", ".JPG", ".PNG", ".GIF", ".TIFF", ".SVG", ".PS"
           };

            // Use LINQ's Any method with StringComparison.OrdinalIgnoreCase for case-insensitive comparison  
            if (!allowedExtensions.Any(ext => ext.Equals(fileExtension, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            return true;
        }

        internal string GetMergedFilePath()
        {
            return mergedFilePath;
        }
    }
}
