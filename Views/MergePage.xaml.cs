using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
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
using System.IO;
using Path = System.IO.Path;
using static PDF_Merger.Services.Delegates;

namespace PDF_Merger.Views
{
    /// <summary>
    /// Interaction logic for MergePage.xaml
    /// </summary>
    public partial class MergePage : Page
    {

        private readonly MergeViewModel viewModel = new();
        private readonly OnMergeComplete OnMergeComplete;
        public MergePage(OnMergeComplete onMergeComplete_)
        {
            InitializeComponent();
            DataContext = viewModel;
            OnMergeComplete = onMergeComplete_;
        }

        private void ListViewItem_Drop(object sender, DragEventArgs e)
        {
            try
            {
                ListViewItem targetItem = (ListViewItem)sender;
                ListViewItem sourceItem = (ListViewItem)e.Data.GetData("System.Windows.Controls.ListViewItem");

                string targetItemString = targetItem.Content.ToString();
                string sourceItemString = sourceItem.Content.ToString();

                int targetItemIndex = viewModel.GetIndex(targetItemString);
                int sourceItemIndex = viewModel.GetIndex(sourceItemString);

                Rectangle topRectangle = (Rectangle)targetItem.Template.FindName("topRectangle", targetItem);
                Rectangle bottomRectangle = (Rectangle)targetItem.Template.FindName("bottomRectangle", targetItem);

                topRectangle.Visibility = Visibility.Collapsed;
                bottomRectangle.Visibility = Visibility.Collapsed;

                //Here we do the re-ordering
                viewModel.ReOrder(sourceItemIndex, targetItemIndex);
            }
            catch (Exception)
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

                int targetItemIndex = viewModel.GetIndex(targetItemString);
                int sourceItemIndex = viewModel.GetIndex(sourceItemString);

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
                if (e.LeftButton == MouseButtonState.Pressed)
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

        [SupportedOSPlatform("windows6.1")]
        private void DropFilePanel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                //string filePath = files[0];

                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);


                    //Check if the user has selected files with the correct extensions
                    string fileExtension = Path.GetExtension(filePath);

                    if (MergeViewModel.ExtensionAllowed(fileExtension) == false) return;

                    //Check if this file already exists in the collection on not before adding it
                    viewModel.AddDocument(fileName, filePath);
                }

            }
        }

        private void DeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            if (DocumentsList.SelectedItem == null) return;

            string selectedItem = DocumentsList.SelectedItem.ToString();
            viewModel.RemoveDocument(selectedItem);
        }

        private void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel.MergeFiles();
                OnMergeComplete(viewModel.GetMergedFilePath());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while merging files: {ex.Message}", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
