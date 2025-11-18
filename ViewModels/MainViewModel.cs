using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System.IO;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System.Runtime.Versioning;
using Syncfusion.Pdf.Interactive;
using Syncfusion.Drawing;
using PDF_Merger.Models;

namespace PDF_Merger.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        internal bool ConvertToWord(string pathToFile = "")
        {
            /*  if (pathToFile == null) 
              {
                  // If no path is provided, open a file dialog to select a PDF
                  OpenFileDialog openFileDialog = new OpenFileDialog
                  {
                      Filter = "PDF Files (*.pdf)|*.pdf",
                      Title = "Select a PDF file to convert to Word"
                  };

                  if (openFileDialog.ShowDialog() == true)
                  {
                      pathToFile = openFileDialog.FileName;
                  }
                  else
                  {
                      return false; // User cancelled the dialog
                  }
              }

              try
              {
                  // Load the PDF document
                  using PdfLoadedDocument loadedDocument = new PdfLoadedDocument(pathToFile);
                  // Create a Word document from the PDF
                  using Syncfusion.DocIO.DLS.WordDocument wordDocument = loadedDocument.ExportAsWord();
                  // Save the Word document to a file
                  string wordFilePath = Path.ChangeExtension(pathToFile, ".docx");
                  wordDocument.Save(wordFilePath);
                  return true;
              }
              catch (Exception ex)
              {
                  throw new InvalidOperationException($"Error converting PDF to Word: {ex.Message}", ex);

              }
  */
            return false;  
        }
    }
}
