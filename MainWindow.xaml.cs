using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace PDF_Merger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<String> documents;
        public MainWindow()
        {
            InitializeComponent();

            documents = new ObservableCollection<String>() 
            {
                "Document 1",
                "Document 2",
                "Document 3",
                "Document 4",
                "Document 5",
                "Document 6",
                "Document 7"
            };

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
    }
}
