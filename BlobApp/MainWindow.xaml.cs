using Azure.Storage.Blobs.Models;
using BlobApp.ViewModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace BlobApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly VMItems vmItems;
        public MainWindow()
        {
            InitializeComponent();
            vmItems = new VMItems();
            Init();
        }

        private void Init()
        {
            cbDirectories.ItemsSource = vmItems.Directories;
            lbItems.ItemsSource = vmItems.Items;
        }

        private void LbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataContext = lbItems.SelectedItem as BlobItem;

        }

        private void CbDirectories_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                vmItems.Directory = cbDirectories.Text.Trim();
                cbDirectories.Text = vmItems.Directory;
            }

        }

        private void CbDirectories_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (vmItems.Directories.Contains(cbDirectories.Text))
            {
                vmItems.Directory = cbDirectories.Text;
                cbDirectories.SelectedItem = vmItems.Directory;
            }
        }

        private async void BtnUpload_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                await vmItems.UploadAsync(dialog.FileName, cbDirectories.Text);
            }
        }
        private async void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (lbItems.SelectedItem is not BlobItem item)
            {
                return;
            }
            var dialog = new SaveFileDialog
            {
                FileName = item.Name[(item.Name.LastIndexOf(VMItems.ForwardSlash) + 1)..]
            };
            if (dialog.ShowDialog() == true)
            {
                await vmItems.DownloadAsync(item, dialog.FileName);
            }
            cbDirectories.Text = vmItems.Directory;
        }
        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lbItems.SelectedItem is not BlobItem item)
            {
                return;
            }
            await vmItems.DeleteAsync(item);
            cbDirectories.Text = vmItems.Directory;
        }
    }
}
