using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Path = System.IO.Path;
using System.Windows.Interop;
using Microsoft.Win32;


namespace PDF_Merger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<String> documents;
        List<DocObject> docObjects;
        
        public MainWindow()
        {
            InitializeComponent();

            documents = new ObservableCollection<String>() 
            {
            };

            docObjects = new List<DocObject>();

            DocumentsList.ItemsSource = documents;
        }

        private void ListViewItem_Drop(object sender, DragEventArgs e)
        {
            try
            {
                ListViewItem targetItem = (ListViewItem)sender;
                ListViewItem sourceItem = (ListViewItem)e.Data.GetData("System.Windows.Controls.ListViewItem");

                string targetItemString = targetItem.Content.ToString();
                string sourceItemString = sourceItem.Content.ToString();

                int targetItemIndex = documents.IndexOf(targetItemString);
                int sourceItemIndex = documents.IndexOf(sourceItemString);

                Rectangle topRectangle = (Rectangle)targetItem.Template.FindName("topRectangle", targetItem);
                Rectangle bottomRectangle = (Rectangle)targetItem.Template.FindName("bottomRectangle", targetItem);

                topRectangle.Visibility = Visibility.Collapsed;
                bottomRectangle.Visibility = Visibility.Collapsed;

                //Here we do the re-ordering
                documents.Move(sourceItemIndex, targetItemIndex);
            }
            catch(Exception) 
            {
                throw;
            }
        }

        private void ListViewItem_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                ListViewItem targetItem = (ListViewItem)sender;

                Rectangle topRectangle = (Rectangle)targetItem.Template.FindName("topRectangle", targetItem);
                Rectangle bottomRectangle = (Rectangle)targetItem.Template.FindName("bottomRectangle", targetItem);

                topRectangle.Visibility = Visibility.Collapsed;
                bottomRectangle.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ListViewItem_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                //Not Used
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ListViewItem_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                ListViewItem targetItem = (ListViewItem)sender;
                ListViewItem sourceItem = (ListViewItem)e.Data.GetData("System.Windows.Controls.ListViewItem");

                //We can't move an item into the same position it is already in     
                if (sourceItem == targetItem) return;

                string targetItemString = targetItem.Content.ToString();
                string sourceItemString = sourceItem.Content.ToString();

                int targetItemIndex = documents.IndexOf(targetItemString);
                int sourceItemIndex = documents.IndexOf(sourceItemString);

                Rectangle topRectangle = (Rectangle)targetItem.Template.FindName("topRectangle", targetItem);
                Rectangle bottomRectangle = (Rectangle)targetItem.Template.FindName("bottomRectangle", targetItem);

                if (targetItemIndex < sourceItemIndex)
                {
                    topRectangle.Visibility = Visibility.Visible;
                }
                else if (targetItemIndex > sourceItemIndex) 
                {
                    bottomRectangle.Visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ListViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if(e.LeftButton == MouseButtonState.Pressed) 
                {
                    ListViewItem sourceItem = (ListViewItem)sender;
                    DragDrop.DoDragDrop(DocumentsList, sourceItem, DragDropEffects.Move);
                }
            }
            catch (Exception)
            {
                throw;      
            }
        }

        private void dropFilePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) 
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string filePath  = files[0];

                string fileName = Path.GetFileName(filePath);


                //Check if the user has selected files with the correct extensions
                string fileExtension = Path.GetExtension(files[0]);

                if (fileExtension != ".pdf") return;

                //Check if this file already exists in the collection on not before adding it
                if (documents.Contains(fileName) == false) 
                {
                    documents.Add(fileName);

                    docObjects.Add(new DocObject(fileName, filePath));    
                }
            }
        }

        private void DeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsList.SelectedItem == null) return;

            string selectedItem = DocumentsList.SelectedItem.ToString();
            documents.Remove(selectedItem);

            docObjects.Remove(docObjects.Where(obj => obj.documentName == selectedItem).Single());
        }

        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PdfDocument mergedDocs = new PdfDocument();

                if (documents.Count() == 0) throw new Exception();

                foreach (String docName in documents)
                {
                    string filePath = docObjects.First(obj => obj.documentName == docName).filePath;

                    //Open each document that needs to be ended to the combined document at the end of the day
                    PdfDocument inputDoc = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);

                    //Iterate through every single page in the doc
                    int count = inputDoc.PageCount;
                    for (int pageNumber = 0; pageNumber < count; pageNumber++)
                    {
                        PdfPage page = inputDoc.Pages[pageNumber];
                        mergedDocs.Pages.Add(page);
                    }
                }

                //Save the final merged document
                string mergedDocumentPath = getFilePath();
                mergedDocs.Save(mergedDocumentPath);

                //Show that the process was completed
                MessageBox.Show("Documents have been merged successfully!", "SUCCESS!", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception) 
            {
                MessageBox.Show("Failed to merge PDF files", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string getFilePath() 
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Pdf Files|*.pdf";
            fileDialog.Title = "Insert file name";

            string fileName = "";
            if (fileDialog.ShowDialog() == true) 
            {
                if (fileDialog.FileName.Trim() == "") throw new Exception("Failed!");

                fileName = fileDialog.FileName;
            }

            return fileName;
        }
    }
}
