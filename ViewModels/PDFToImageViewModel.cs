using Microsoft.Win32;
using Syncfusion.DocIO.DLS;
using Syncfusion.Pdf.Parsing;
using Syncfusion.PdfToImageConverter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDF_Merger.ViewModels
{
    public partial class PDFToImageViewModel
    {
        public static void ConvertToImage(string path) 
        {
            SaveFileDialog dialog = new()
            {
                Filter = "PNG Files|*.png",
                Title = "Insert file name"
            };

            string filePath = "";
            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName == string.Empty) throw new Exception("Failed to get file name for saving Image!");
                filePath = dialog.FileName;

            }
            
            int pages = GetPageCount(path);

            for (int page = 0; page < pages; page++) 
            {
                string outputPath = AddToFileName(filePath, page.ToString());
                Convert(path, page, outputPath);
            }
        }

        public static string AddToFileName(string originalFilePath, string suffix)
        {
            // Get the directory name
            string directory = Path.GetDirectoryName(originalFilePath);

            // Get the file name without extension
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);

            // Get the file extension
            string extension = Path.GetExtension(originalFilePath);

            // Construct the new file name with the added suffix
            string newFileName = $"{fileNameWithoutExtension}{suffix}{extension}";

            // Combine the directory and the new file name to get the full new path
            string newFilePath = Path.Combine(directory, newFileName);

            return newFilePath;
        }

        public static int GetPageCount(string path)
        {
            FileStream inputStream = new(Path.GetFullPath(path), FileMode.Open, FileAccess.ReadWrite);

            // Load the PDF document from the input stream
            PdfLoadedDocument loadedDocument = new PdfLoadedDocument(inputStream);

            // Get the total page count
            int pageCount = loadedDocument.Pages.Count;

            // Close the document
            loadedDocument.Close(true);

            return pageCount;
        }

        public static void Convert(string path, int pageNumber, string outputPath) 
        {

            //Initialize PDF to Image converter.
            PdfToImageConverter imageConverter = new();

            //Load the PDF document as a stream
            FileStream inputStream = new(Path.GetFullPath(path), FileMode.Open, FileAccess.ReadWrite);

            imageConverter.Load(inputStream);

            //Convert PDF to Image.
            Stream outputStream = imageConverter.Convert(pageNumber, 600, 600, false, false);

            //Rewind the stream position to the beginning before copying.
            outputStream.Position = 0;


            //Create file stream.
            using (FileStream outputFileStream = new(Path.GetFullPath(outputPath), FileMode.Create, FileAccess.ReadWrite))
            {
                //Save the image to file stream.
                outputStream.CopyTo(outputFileStream);
            }
            //Dispose the imageConverter
            imageConverter.Dispose();
        }
    }
}
