using Azure.Storage.Blobs.Models;
using BlobApp.Dal;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlobApp.ViewModel
{
    internal class VMItems
    {
        public const string ForwardSlash = "/";
        private string? directory;

        public ObservableCollection<BlobItem> Items { get; }
        public ObservableCollection<string> Directories { get; }
        public string? Directory
        {
            get => directory;
            set
            {
                directory = value;
                Refresh();
            }
        }
        public VMItems()
        {
            Items = new ObservableCollection<BlobItem>();
            Directories = new ObservableCollection<string>();
            Refresh();
        }
        private void Refresh()
        {
            Directories.Clear();
            Items.Clear();
            Repository.Container.GetBlobs().ToList().ForEach(blob =>
            {
                if (blob.Name.Contains(ForwardSlash))
                {
                    var dir = blob.Name[..blob.Name.LastIndexOf(ForwardSlash)];
                    if (!Directories.Contains(dir))
                    {
                        Directories.Add(dir);
                    }
                }

                if (string.IsNullOrEmpty(Directory) && !blob.Name.Contains(ForwardSlash))
                {
                    Items.Add(blob);
                }
                else if (!string.IsNullOrEmpty(Directory) && blob.Name.StartsWith($"{Directory}{ForwardSlash}"))
                {
                    Items.Add(blob);
                }
            });
        }

        public async Task UploadAsync(string path, string directory)
        {
            var filename = path[(path.LastIndexOf(Path.DirectorySeparatorChar) + 1)..];
            if (!string.IsNullOrEmpty(directory))
            {
                filename = $"{directory}{ForwardSlash}{filename}";

            }
            using var fs = File.OpenRead(path);
            await Repository.Container.GetBlobClient(filename).UploadAsync(fs,true);
            Refresh();
        }
        
        public async Task DownloadAsync(BlobItem item, string path)
        {
            using var fs = File.OpenWrite(path);
            await Repository.Container.GetBlobClient(item.Name).DownloadToAsync(fs);
        }

        public async Task DeleteAsync(BlobItem item)
        {
            await Repository.Container.GetBlobClient(item.Name).DeleteAsync();
            Refresh();
        }
    }
}
